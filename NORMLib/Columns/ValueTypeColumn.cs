using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib.Columns
{
	public  class ValueTypeColumn<RowType,ValType>:Column<RowType,ValType?>
		where ValType:struct
	{
	}
}
