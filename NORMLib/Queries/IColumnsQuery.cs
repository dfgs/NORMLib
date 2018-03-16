using NORMLib.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public interface IColumnsQuery<RowType>:ITableQuery<RowType>
	{
		IEnumerable<IColumn> Columns
		{
			get;
		}

	}
}
