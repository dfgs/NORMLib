using System;
using System.Collections.Generic;
using System.Data.Common;
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

		public override DbCommand CreateCommand(ICommandFactory CommandFactory)
		{
			return CommandFactory.CreateCommand(this);
		}


	}
}
