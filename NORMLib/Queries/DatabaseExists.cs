using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public class DatabaseExists : DatabaseQuery,IDatabaseExists
	{



		public DatabaseExists(string DatabaseName):base(DatabaseName)
		{
		}


	}
}
