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
		private SortedList<int, Commit> commits;
		private string databaseName;
		private IConnectionFactory connectionFactory;
		private ICommandFactory commandFactory;

		private DbConnection connection;
		private DbTransaction transaction;


		public VersionController(string DatabaseName, IConnectionFactory ConnectionFactory, ICommandFactory CommandFactory)
		{
			Type[] types;
			ConstructorInfo ci;
			RevisionAttribute revisionAttribute;
			Commit commit;
			Assembly assembly;

			this.databaseName = DatabaseName;
			this.connectionFactory = ConnectionFactory;
			this.commandFactory = CommandFactory;

			assembly = Assembly.GetEntryAssembly();
			commits = new SortedList<int, Commit>();
			
			types = assembly.GetTypes();
			foreach(Type type in types)
			{
				if (!type.IsSubclassOf(typeof(Commit))) continue;
				revisionAttribute = type.GetCustomAttribute<RevisionAttribute>();
				if (revisionAttribute == null) throw (new Exception($"Commit {type.Name} has no revision number defined"));
				if (commits.ContainsKey(revisionAttribute.Value)) throw (new Exception($"Commit {type.Name} has duplicate revision number ({revisionAttribute.Value})"));


				ci = type.GetConstructor(Type.EmptyTypes);
				if (ci==null) throw (new Exception($"Commit {type.Name} has no default public constructor"));
				commit = (Commit)ci.Invoke(null);
				commits.Add(revisionAttribute.Value, commit);
			}
		}

		public void DropDatabase()
		{
			DbCommand command;

			using (DbConnection connection = connectionFactory.CreateConnectionToServer())
			{
				connection.Open();

				command = commandFactory.CreateDropDatabaseCommand(databaseName);
				command.Connection = connection;

				command.ExecuteNonQuery();
			}
		}

				


		public void Run()
		{
			int currentRevision;
			DbCommand command;
			bool exists;
			DbDataReader reader;


			using (connection = connectionFactory.CreateConnectionToServer())
			{
				connection.Open();

				#region check if exists
				command = commandFactory.CreateSelectDatabaseCommand(databaseName);
				command.Connection = connection;
				reader = command.ExecuteReader();
				exists = reader.HasRows;
				reader.Close();
				#endregion

				#region create if not exists
				if (!exists)
				{
					command = commandFactory.CreateCreateDatabaseCommand(databaseName);
					command.Connection = connection;
					command.ExecuteNonQuery();
				}
				#endregion
			}
				
			


			using (connection = connectionFactory.CreateConnectionToDatabase())
			{
				connection.Open();

				#region check if table Revision exists
				command = commandFactory.CreateSelectTableCommand<Revision>();
				command.Connection = connection;
				reader = command.ExecuteReader();
				exists = reader.HasRows;
				reader.Close();
				#endregion

				#region create revision table if not exists
				if (!exists)
				{
					command = commandFactory.CreateCreateTableCommand<Revision>(Revision.RevisionIDColumn, Revision.DateColumn, Revision.ValueColumn);
					command.Connection = connection;
					command.ExecuteNonQuery();
				}
				#endregion

				currentRevision = Select<Revision>().Max(item => item.Value)??0;

				foreach (KeyValuePair<int,Commit> keyValuePair in commits.Where(item=>item.Key>currentRevision))
				{
					transaction = connection.BeginTransaction();
					
					try
					{
						keyValuePair.Value.Execute(this);
						Insert(new Revision() { Date=DateTime.Now, Value=keyValuePair.Key });
						transaction.Commit();
					}
					catch (Exception ex)
					{
						transaction.Rollback();
						throw (new Exception($"Error while upgrading to revision {keyValuePair.Key} ({ex.Message})"));
					}
				}
	
			}

				
		}

		public List<RowType> Select<RowType>(Filter Filter = null)
			where RowType : new()
		{
			DbCommand command;
			DbDataReader reader;
			RowType item;
			object value;
			List<RowType> results;

			results = new List<RowType>();

			command = commandFactory.CreateSelectCommand<RowType>(Filter);
			command.Connection = connection;
			command.Transaction = transaction;
			reader = command.ExecuteReader();
			while (reader.Read())
			{
				item = new RowType();
				foreach (IColumn column in Schema<RowType>.Columns)
				{
					value = commandFactory.ConvertFromDbValue(column, reader[column.Name]);
					column.SetValue(item, value);
				}
				results.Add(item);
			}
			reader.Close();

			return results;
		}

		public void Insert<RowType>(RowType Item)
		{
			DbCommand command;
			object result, key;

			command = commandFactory.CreateInsertCommand(Item);
			command.Connection = connection;
			command.Transaction = transaction;
			command.ExecuteNonQuery();

			command = commandFactory.CreateIdentityCommand(Item);
			command.Connection = connection;
			result = command.ExecuteScalar();

			key = Convert.ChangeType(result, Schema<RowType>.IdentityColumn.ColumnType);
			Schema<RowType>.IdentityColumn.SetValue(Item, key);
		}

		public void Update<RowType>(RowType Item)
		{
			DbCommand command;

			command = commandFactory.CreateUpdateCommand(Item);
			command.Connection = connection;
			command.Transaction = transaction;
			command.ExecuteNonQuery();
		}

		public void Delete<RowType>(RowType Item)
		{
			DbCommand command;

			command = commandFactory.CreateDeleteCommand(Item);
			command.Connection = connection;
			command.Transaction = transaction;
			command.ExecuteNonQuery();

		}

		public void CreateTable<RowType>(params IColumn[] Columns)
		{
			DbCommand command;

			command = commandFactory.CreateCreateTableCommand<RowType>(Columns);
			command.Connection = connection;
			command.Transaction = transaction;
			command.ExecuteNonQuery();
		}

		public void CreateColumn(IColumn Column)
		{
			DbCommand command;

			command = commandFactory.CreateCreateColumnCommand(Column);
			command.Connection = connection;
			command.Transaction = transaction;
			command.ExecuteNonQuery();
		}

	}
}
