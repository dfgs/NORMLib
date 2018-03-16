using NORMLib.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public interface ICreateColumn<RowType>:ITableQuery<RowType>
	{
		IColumn Column
		{
			get;
		}

	}
}
