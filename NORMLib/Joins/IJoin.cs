using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public interface IJoin
	{
		string JoinedTableName
		{
			get;
		}

		IColumn JoinedColumn
		{
			get;
		}

		string TargetTableName
		{
			get;
		}

		IColumn TargetColumn
		{
			get;
		}

	}
}
