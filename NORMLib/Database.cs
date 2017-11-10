using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace NORMLib
{
    public class Database:IDatabase
    {

		private IConnectionFactory connectionFactory;
		private ICommandFactory commandFactory;


		public Database(IConnectionFactory ConnectionFactory,ICommandFactory CommandFactory)
		{
			this.connectionFactory = ConnectionFactory;
			this.commandFactory = CommandFactory;
		}
		public void Dispose()
		{

		}
				

		public void Insert<RowType>(RowType Item)
		{
			DbCommand command;
			object result,key;

			using (DbConnection connection = connectionFactory.CreateConnectionToDatabase())
			{
				connection.Open();
				command = commandFactory.CreateInsertCommand(Item);
				command.Connection = connection;
				command.ExecuteNonQuery();

				command = commandFactory.CreateIdentityCommand(Item);
				command.Connection = (DbConnection)connectionFactory;
				result = command.ExecuteScalar();
			}

			key = Convert.ChangeType(result, Schema<RowType>.IdentityColumn.ColumnType);
			Schema<RowType>.IdentityColumn.SetValue(Item,key);
		}

		public void Update<RowType>(RowType Item)
		{
			DbCommand command;

			using (DbConnection connection = connectionFactory.CreateConnectionToDatabase())
			{
				connection.Open();
				command = commandFactory.CreateUpdateCommand(Item);
				command.Connection = connection;
				command.ExecuteNonQuery();
			}
		}

		public void Delete<RowType>(RowType Item)
		{
			DbCommand command;

			using (DbConnection connection = connectionFactory.CreateConnectionToDatabase())
			{
				connection.Open();
				command = commandFactory.CreateDeleteCommand(Item);
				command.Connection = connection;
				command.ExecuteNonQuery();
			}
		}

		public List<RowType> Select<RowType>(Filter Filter = null)
			where RowType:new()
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
				command = commandFactory.CreateSelectCommand<RowType>(Filter);
				command.Connection = connection;
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
			}

			return results;
		}



	}
}
