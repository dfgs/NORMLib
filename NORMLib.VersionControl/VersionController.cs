using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib.VersionControl
{
	public class VersionController:IVersionController
	{
		private IConnectionFactory connectionFactory;
		private ICommandFactory commandFactory;

		private DbConnection connection;
		private DbTransaction transaction;

		private List<Tuple<int, ITable>> tables;
		private List<Tuple<int, IRelation>> relations;


		public VersionController(IConnectionFactory ConnectionFactory, ICommandFactory CommandFactory,Type DatabaseType)
		{
			RevisionAttribute revisionAttribute;
			FieldInfo[] fis;
			int revision;
			object data;

			this.connectionFactory = ConnectionFactory;
			this.commandFactory = CommandFactory;
			tables = new List<Tuple<int, ITable>>();
			relations = new List<Tuple<int, IRelation>>();	

			fis = DatabaseType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);
			foreach (FieldInfo fi in fis)
			{
				revisionAttribute = fi.GetCustomAttribute<RevisionAttribute>(true);
				revision = revisionAttribute?.Value ?? 0;
				data = fi.GetValue(null);

				if (data is ITable) tables.Add(new Tuple<int, ITable>(revision, (ITable)data));
				else if (data is IRelation) relations.Add(new Tuple<int, IRelation>(revision, (IRelation)data));
			}

		}

		public void DropDatabase()
		{
			DbCommand command;

			using (DbConnection connection = connectionFactory.CreateConnectionToServer())
			{
				connection.Open();

				command = commandFactory.CreateDropDatabaseCommand(connectionFactory.DatabaseName);
				command.Connection = connection;

				command.ExecuteNonQuery();
			}
		}

		private int GetMaxRevision()
		{
			int maxRevision = 0;
			foreach(int value in tables.Select(item=>item.Item1).Union(relations.Select(item=>item.Item1)))
			{
				maxRevision=Math.Max(maxRevision, value);
			}
			return maxRevision;
		}

		private IEnumerable<ITable> GetTables(int MinRevision, int MaxRevision = int.MaxValue)
		{
			return tables.Where(item => (item.Item1 >= MinRevision) && (item.Item1 <= MaxRevision)).Select(item => item.Item2);
		}
		private IEnumerable<IRelation> GetRelations(int MinRevision, int MaxRevision = int.MaxValue)
		{
			return relations.Where(item => (item.Item1 >= MinRevision) && (item.Item1 <= MaxRevision)).Select(item => item.Item2);
		}
		private IEnumerable<IColumn> GetColumns(int MinRevision, int MaxRevision = int.MaxValue)
		{
			return tables.SelectMany(item => item.Item2.GetColumns(MinRevision, MaxRevision));
		}


		public void Run()
		{
			int currentRevision;
			DbCommand command;
			bool exists;
			DbDataReader reader;
			int maxRevision;
			ITable revisionTable;

			revisionTable = new Table<Revision>();

			using (connection = connectionFactory.CreateConnectionToServer())
			{
				connection.Open();

				#region check if exists
				command = commandFactory.CreateSelectDatabaseCommand(connectionFactory.DatabaseName);
				command.Connection = connection;
				reader = command.ExecuteReader();
				exists = reader.HasRows;
				reader.Close();
				#endregion

				#region create if not exists
				if (!exists)
				{
					command = commandFactory.CreateCreateDatabaseCommand(connectionFactory.DatabaseName);
					command.Connection = connection;
					command.ExecuteNonQuery();
				}
				#endregion
			}
				
	
			using (connection = connectionFactory.CreateConnectionToDatabase())
			{
				connection.Open();

				#region check if table Revision exists
				command = commandFactory.CreateSelectTableCommand(revisionTable);
				command.Connection = connection;
				reader = command.ExecuteReader();
				exists = reader.HasRows;
				reader.Close();
				#endregion

				#region create revision table if not exists
				if (!exists)
				{
					
					command = commandFactory.CreateCreateTableCommand(revisionTable, revisionTable.Columns);
					command.Connection = connection;
					command.ExecuteNonQuery();
				}
				#endregion

				currentRevision = Select<Revision>( revisionTable.Columns,null ).Max(item => item.Value)??-1;
				maxRevision = GetMaxRevision();

				for(int revision=currentRevision+1;revision<=maxRevision;revision++)
				{
					transaction = connection.BeginTransaction();
					try
					{
						foreach (ITable table in GetTables(revision,revision))
						{
							CreateTable(table, table.GetColumns(0, revision));
						}
						foreach (ITable table in GetTables(0, revision - 1)) 
						{
							foreach(IColumn column in table.GetColumns(revision,revision))
							{
								CreateColumn(table,column);
							}
						}
						foreach (IRelation relation in GetRelations(revision, revision))
						{
							CreateRelation(relation);
						}

						Insert(new Revision() { Date = DateTime.Now, Value = revision },revisionTable.Columns);
						transaction.Commit();
					}
					catch (Exception ex)
					{
						transaction.Rollback();
						throw (new Exception($"Error while upgrading to revision {revision} ({ex.Message})"));
					}

				}
	
			}

				
		}

		private List<RowType> Select<RowType>(IEnumerable<IColumn> Columns,Filter Filter)
			where RowType : new()
		{
			DbCommand command;
			DbDataReader reader;
			RowType item;
			object value;
			List<RowType> results;

			results = new List<RowType>();

			command = commandFactory.CreateSelectCommand<RowType>(Columns, Filter);
			command.Connection = connection;
			command.Transaction = transaction;
			reader = command.ExecuteReader();
			while (reader.Read())
			{
				item = new RowType();
				foreach (IColumn column in NORMLib.Table<RowType>.Columns)
				{
					value = commandFactory.ConvertFromDbValue(column, reader[column.Name]);
					column.SetValue(item, value);
				}
				results.Add(item);
			}
			reader.Close();

			return results;
		}

		private void Insert<RowType>(RowType Item,IEnumerable<IColumn> Columns)
		{
			DbCommand command;
			object result, key;

			command = commandFactory.CreateInsertCommand(Item,Columns);
			command.Connection = connection;
			command.Transaction = transaction;
			command.ExecuteNonQuery();

			command = commandFactory.CreateIdentityCommand<RowType>();
			command.Connection = connection;
			result = command.ExecuteScalar();

			key = Convert.ToInt32(result);
			NORMLib.Table<RowType>.IdentityColumn.SetValue(Item, key);
		}

		private void Update<RowType>(RowType Item, IEnumerable<IColumn> Columns)
		{
			DbCommand command;

			command = commandFactory.CreateUpdateCommand(Item,Columns);
			command.Connection = connection;
			command.Transaction = transaction;
			command.ExecuteNonQuery();
		}

		private void Delete<RowType>(RowType Item)
		{
			DbCommand command;

			command = commandFactory.CreateDeleteCommand(Item);
			command.Connection = connection;
			command.Transaction = transaction;
			command.ExecuteNonQuery();

		}

		private void CreateTable(ITable Table, IEnumerable<IColumn> Columns)
		{
			DbCommand command;

			command = commandFactory.CreateCreateTableCommand(Table, Columns);
			command.Connection = connection;
			command.Transaction = transaction;
			command.ExecuteNonQuery();
		}

		private void CreateColumn(ITable Table,IColumn Column)
		{
			DbCommand command;

			command = commandFactory.CreateCreateColumnCommand(Table,Column);
			command.Connection = connection;
			command.Transaction = transaction;
			command.ExecuteNonQuery();
		}

		private void CreateRelation(IRelation Relation)
		{
			DbCommand command;

			command = commandFactory.CreateCreateRelationCommand(Relation);
			command.Connection = connection;
			command.Transaction = transaction;
			command.ExecuteNonQuery();
		}

	}
}
