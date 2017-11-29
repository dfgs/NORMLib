using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public class CreateDatabase : DatabaseQuery, ICreateDatabase
	{



		public CreateDatabase(string DatabaseName):base(DatabaseName)
		{
		}


	}
}
