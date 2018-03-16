using NORMLib.Columns;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public abstract class ColumnsQuery<RowType> : TableQuery<RowType>, IColumnsQuery<RowType>
	{
		private IColumn[] columns;
		public IEnumerable<IColumn> Columns
		{
			get { return columns; }
		}


		public ColumnsQuery(params IColumn[] Columns)
		{
			if ((Columns == null) || (Columns.Count()==0)) columns = Table<RowType>.Columns.ToArray();
			else this.columns = Columns;

		}




	}
}
