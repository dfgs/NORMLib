using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public interface ISelect<RowType>:IFilterQuery<RowType>
	{
		

		IEnumerable<IColumn> Orders
		{
			get;
		}

		IQuery OrderBy(params IColumn[] Columns);
	}
}
