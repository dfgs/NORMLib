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
	public abstract class VersionController:IVersionController
	{
		private IServer server;

		private string databaseName;

		//private int revision=-1;

		private List<Tuple<int, MethodInfo>> methods;


		public VersionController(IServer Server,string DatabaseName)
		{
			RevisionAttribute revisionAttribute;
			MethodInfo[] mis;
			ParameterInfo[] pis;
			Type type;
			int revision;


			this.server = Server;
			this.databaseName = DatabaseName;
			methods = new List<Tuple<int, MethodInfo>>();

			type = GetType();

			mis = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);
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
			foreach(int value in methods.Select(item => item.Item1))
			{
				maxRevision=Math.Max(maxRevision, value);
			}
			return maxRevision;
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
			List<IQuery> queries;


			exists = server.DatabaseExists();
			if (!exists) server.CreateDatabase();

			exists = server.ExecuteScalar(new TableExists<Revision>())!=null;
			if (!exists) server.ExecuteNonQuery(new CreateTable<Revision>());

			currentRevision = server.Execute(new Select<Revision>()).Max(item => item.Value) ?? -1;
			maxRevision = GetMaxRevision();

			queries = new List<IQuery>();
			for(int revision=currentRevision+1;revision<=maxRevision;revision++)
			{
				queries.Clear();

				foreach (MethodInfo mi in GetMethods(revision, revision))
				{
					foreach (IQuery query in (IEnumerable<IQuery>)mi.Invoke(null, null)) queries.Add(query);
				}

				queries.Add(new Insert<Revision>(new Revision() { Date = DateTime.Now, Value = revision }));

				server.ExecuteTransaction(queries.ToArray());
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
