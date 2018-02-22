using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib.SqlCE
{
	public class SqlCEConnectionFactory : IConnectionFactory
	{
		private string fileName;

		public string DatabaseName
		{
			get { return System.IO.Path.GetFileNameWithoutExtension(fileName); }
		}

		public SqlCEConnectionFactory(string FileName)
		{
			this.fileName = FileName;
		}

		

		public DbConnection CreateConnectionToDatabase()
		{
			return new SqlCeConnection($"Data Source={fileName};Persist Security Info=False;");
		}

		public DbConnection CreateConnectionToServer()
		{
			return new SqlCeConnection($"Data Source={fileName};Persist Security Info=False;");
		}


	}
}
