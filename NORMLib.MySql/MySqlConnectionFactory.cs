using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib.MySql
{
	public class MySqlConnectionFactory : IConnectionFactory
	{
		private string hostname;
		private string databaseName;
		public string DatabaseName
		{
			get { return databaseName; }
		}

		private string user;
		private string password;

		public MySqlConnectionFactory(string Hostname, string DatabaseName, string User, string Password)
		{
			this.hostname = Hostname; this.databaseName = DatabaseName; this.user = User; this.password = Password;
		}

		public DbConnection CreateConnectionToDatabase()
		{
			return new MySqlConnection($"Server={hostname};Database={databaseName};Uid={user};Pwd={password};");
		}

		public DbConnection CreateConnectionToServer()
		{
			return new MySqlConnection($"Server={hostname};Uid={user};Pwd={password};");
		}
	}
}
