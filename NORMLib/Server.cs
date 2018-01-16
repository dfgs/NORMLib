using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace NORMLib
{
    public class Server:IServer
    {

		private IConnectionFactory connectionFactory;
		private ICommandFactory commandFactory;

		public string name;
		public string Name
		{
			get { return name; }
		}

		public Server(IConnectionFactory ConnectionFactory,ICommandFactory CommandFactory)
		{
			this.connectionFactory = ConnectionFactory;
			this.commandFactory = CommandFactory;
		}

		public void Dispose()
		{

		}
		public bool DatabaseExists()
		{
			DbCommand command;

			using (DbConnection connection = connectionFactory.CreateConnectionToServer())
			{
				connection.Open();
				command = new DatabaseExists(connectionFactory.DatabaseName).CreateCommand(commandFactory);
				command.Connection = connection;
				return command.ExecuteScalar()!=null;
			}
		}

		public void CreateDatabase()
		{
			DbCommand command;

			using (DbConnection connection = connectionFactory.CreateConnectionToServer())
			{
				connection.Open();
				command = new CreateDatabase(connectionFactory.DatabaseName).CreateCommand(commandFactory);
				command.Connection = connection;
				command.ExecuteNonQuery();
			}
		}

		public object ExecuteTransaction(params IQuery[] Queries)
		{
			DbCommand command;
			DbTransaction transaction;
			object result=null;

			using (DbConnection connection = connectionFactory.CreateConnectionToDatabase())
			{
				connection.Open();
				transaction=connection.BeginTransaction();
				try
				{
					foreach (IQuery query in Queries)
					{
						command = query.CreateCommand(commandFactory);
						command.Connection = connection;
						command.Transaction = transaction;
						result= command.ExecuteScalar();
					}
					transaction.Commit();
					return result;
				}
				catch(Exception ex)
				{
					transaction.Rollback();
					throw (ex);
				}
			}
		}

		public int ExecuteNonQuery(IQuery Query)
		{
			DbCommand command;

			using (DbConnection connection = connectionFactory.CreateConnectionToDatabase())
			{
				connection.Open();
				command = Query.CreateCommand(commandFactory);
				command.Connection = connection;
				return command.ExecuteNonQuery();
			}
		}
		public object ExecuteScalar(IQuery Query)
		{
			DbCommand command;

			using (DbConnection connection = connectionFactory.CreateConnectionToDatabase())
			{
				connection.Open();
				command = Query.CreateCommand(commandFactory);
				command.Connection = connection;
				return command.ExecuteScalar();
			}
		}

		public IEnumerable<RowType> Execute<RowType>(ISelect<RowType> Query)
			where RowType : new()
		{
			DbCommand command;
			DbDataReader reader;
			RowType item;
			object value;
			List<RowType> results;

			results = new List<RowType>();

			using (DbConnection connection = connectionFactory.CreateConnectionToDatabase())
			{
				connection.Open();
				command = commandFactory.CreateCommand(Query);
				command.Connection = connection;
				reader = command.ExecuteReader();
				while (reader.Read())
				{
					item = new RowType();
					foreach (IColumn column in Query.Columns)
					{
						value = commandFactory.ConvertFromDbValue(column, reader[column.Name]);
						column.SetValue(item, value);
					}
					results.Add(item);
				}
				reader.Close();
			}

			return results;
		}

		
	}
}
