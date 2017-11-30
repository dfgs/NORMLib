using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public abstract class Query : IQuery
	{

		public abstract DbCommand CreateCommand(ICommandFactory CommandFactory);


	}
}
