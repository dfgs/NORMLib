using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace NORMLib
{
    public class Session:ISession
    {

		private IDbConnection connection;
		private ICommandFactory commandFactory;

			
		public Session(IDbConnection Connection,ICommandFactory CommandFactory)
		{
			this.connection = Connection;
			this.commandFactory = CommandFactory;
			this.connection.Open();
		}
		public void Dispose()
		{

			this.connection.Close();
		}
				

		public void Insert<DataType>(DataType Item)
		{
			DbCommand command;
			object result,key;

			command = commandFactory.CreateInsertCommand(Item);
			command.Connection = (DbConnection)connection;
			command.ExecuteNonQuery();

			command = commandFactory.CreateIdentityCommand(Item);
			command.Connection = (DbConnection)connection;
			result=command.ExecuteScalar() ;
			key = Convert.ChangeType(result, Schema<DataType>.IdentityColumn.ColumnType);
			Schema<DataType>.IdentityColumn.SetValue(Item,key);
		}

		public void Update<DataType>(DataType Item)
		{
			DbCommand command;

			command = commandFactory.CreateUpdateCommand(Item);
			command.Connection = (DbConnection)connection;
			command.ExecuteNonQuery();
		}

		public void Delete<DataType>(DataType Item)
		{
			DbCommand command;

			command = commandFactory.CreateDeleteCommand(Item);
			command.Connection = (DbConnection)connection;
			command.ExecuteNonQuery();
		}

		public IEnumerable<DataType> Select<DataType>()
			where DataType:new()
		{
			DbCommand command;
			DbDataReader reader;
			DataType item;
			object value;

			command = commandFactory.CreateSelectCommand<DataType>();
			command.Connection = (DbConnection)connection;
			reader=command.ExecuteReader();
			while(reader.Read())
			{
				item = new DataType();
				foreach(IColumn<DataType> column in Schema<DataType>.Columns)
				{
					value = commandFactory.ConvertFromDbValue(column, reader[column.Name]);
					column.SetValue(item, value);
				}
				yield return item;
			}
		}

	}
}
