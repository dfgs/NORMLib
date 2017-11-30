using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public interface IQuery
	{
		DbCommand CreateCommand(ICommandFactory CommandFactory);

	}
}
