using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib.Sql
{
	public class SqlCommandFactory : CommandFactory<SqlCommand>
	{
		protected override SqlCommand OnCreateIdentityCommand<DataType>(DataType Item)
		{
			return new SqlCommand("SELECT @@identity");
		}

		protected override void OnSetParameter<DataType>(SqlCommand Command, string Name, object Value)
		{
			Command.Parameters.AddWithValue(Name, Value);
		}
	}
}
