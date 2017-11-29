using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public interface IDatabaseQuery:IQuery
	{
		string DatabaseName
		{
			get;
		}

	}
}
