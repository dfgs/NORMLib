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
		private IServer server;

		private string databaseName;

		//private int revision=-1;

		private List<Tuple<int, ITable>> tables;
		private List<Tuple<int, IRelation>> relations;
		private List<Tuple<int, MethodInfo>> methods;


		public VersionController(IServer Server,Type DatabaseType)
		{
			DatabaseAttribute databaseAttribute;
			RevisionAttribute revisionAttribute;
			FieldInfo[] fis;
			MethodInfo[] mis;
			ParameterInfo[] pis;
			
			int revision;
			object data;

			this.server = Server;
			tables = new List<Tuple<int, ITable>>();
			relations = new List<Tuple<int, IRelation>>();
			methods = new List<Tuple<int, MethodInfo>>();

			databaseAttribute = DatabaseType.GetCustomAttribute<DatabaseAttribute>();
			databaseName = databaseAttribute?.Name??DatabaseType.Name;

			fis = DatabaseType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);
			foreach (FieldInfo fi in fis)
			{
				revisionAttribute = fi.GetCustomAttribute<RevisionAttribute>(true);
				revision = revisionAttribute?.Value ?? 0;
				data = fi.GetValue(null);

				if (data is ITable) tables.Add(new Tuple<int, ITable>(revision, (ITable)data));
				else if (data is IRelation) relations.Add(new Tuple<int, IRelation>(revision, (IRelation)data));
			}

			mis = DatabaseType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);
			foreach(MethodInfo mi in mis)
			{
				revisionAttribute = mi.GetCustomAttribute<RevisionAttribute>(true);
				revision = revisionAttribute?.Value ?? 0;
				if (mi.ReturnParameter.ParameterType!=typeof(IEnumerable<IQuery>)) continue;

				pis = mi.GetParameters();
				if (pis.Length != 0) continue;
				methods.Add(new Tuple<int, MethodInfo>(revision, mi));
			}

		}

		private int GetMaxRevision()
		{
			int maxRevision = 0;
			foreach(int value in tables.Select(item=>item.Item1).Union(relations.Select(item => item.Item1)).Union(methods.Select(item => item.Item1)))
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
		private IEnumerable<MethodInfo> GetMethods(int MinRevision, int MaxRevision = int.MaxValue)
		{
			return methods.Where(item => (item.Item1 >= MinRevision) && (item.Item1 <= MaxRevision)).Select(item => item.Item2);
		}


		public void Run()
		{
			int currentRevision;
			bool exists;
			int maxRevision;
			ITable revisionTable;
			List<IQuery> queries;

			revisionTable = new Table<Revision>();

			exists = server.Execute(new DatabaseExists(databaseName));
			if (!exists) server.Execute(new CreateDatabase(databaseName));

			exists = server.Execute(new TableExists<Revision>());
			if (!exists) server.Execute(new CreateTable<Revision>());

			currentRevision = server.Execute(new Select<Revision>()).Max(item => item.Value) ?? -1;
			maxRevision = GetMaxRevision();

			queries = new List<IQuery>();
			for(int revision=currentRevision+1;revision<=maxRevision;revision++)
			{
				queries.Clear();

				foreach (ITable table in GetTables(revision,revision)) queries.Add(table.GetCreateQuery( table.GetColumns(0,revision).ToArray() ));

				foreach (ITable table in GetTables(0, revision - 1))
				{
					foreach(IColumn column in table.GetColumns(revision,revision)) queries.Add(table.GetCreateQuery(column));
				}
				foreach (IRelation relation in GetRelations(revision, revision)) queries.Add(relation.GetCreateQuery());
			
				foreach (MethodInfo mi in GetMethods(revision, revision))
				{
					foreach (IQuery query in (IEnumerable<IQuery>)mi.Invoke(null, null)) queries.Add(query);
				}

				queries.Add(new Insert<Revision>(new Revision() { Date = DateTime.Now, Value = revision }));
				//transaction.Commit();

				server.Execute(queries.ToArray());
			}

				
		}

		/*
		public List<RowType> Select<RowType>(Filter Filter)
			where RowType : new()
		{
			DbCommand command;
			DbDataReader reader;
			RowType item;
			object value;
			List<RowType> results;

			results = new List<RowType>();

			command = commandFactory.CreateSelectCommand<RowType>(Table<RowType>.GetColumns(0,revision), Filter);
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

		public void Insert<RowType>(RowType Item)
		{
			DbCommand command;
			object result, key;

			command = commandFactory.CreateInsertCommand(Item, Table<RowType>.GetColumns(0,revision));
			command.Connection = connection;
			command.Transaction = transaction;
			command.ExecuteNonQuery();

			command = commandFactory.CreateIdentityCommand<RowType>();
			command.Connection = connection;
			result = command.ExecuteScalar();

			key = Convert.ToInt32(result);
			NORMLib.Table<RowType>.IdentityColumn.SetValue(Item, key);
		}

		public void Update<RowType>(RowType Item)
		{
			DbCommand command;

			command = commandFactory.CreateUpdateCommand(Item, Table<RowType>.GetColumns(0,revision));
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
		//*/


	}
}
