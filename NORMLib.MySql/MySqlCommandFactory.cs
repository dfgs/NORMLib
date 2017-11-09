using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib.MySql
{
	public class MySqlCommandFactory : CommandFactory<MySqlCommand>
	{

		protected override string OnFormatColumnName(IColumn Column)
		{
			return $"`{Column.Name}`";
		}
		protected override string OnFormatTableName(string TableName)
		{
			return $"`{TableName}`";
		}
		protected override MySqlCommand OnCreateIdentityCommand<DataType>(DataType Item)
		{
			return new MySqlCommand("SELECT LAST_INSERT_ID()");
		}

		protected override void OnSetParameter<DataType>(MySqlCommand Command, string Name, object Value)
		{
			Command.Parameters.AddWithValue(Name, Value);
		}
	}
}
