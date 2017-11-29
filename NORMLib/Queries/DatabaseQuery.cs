using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public abstract class DatabaseQuery : Query,IDatabaseQuery
	{

		private string databaseName;
		public string DatabaseName
		{
			get { return databaseName; }
		}

		public DatabaseQuery(string DatabaseName)
		{
			this.databaseName = DatabaseName;
		}


	}
}
