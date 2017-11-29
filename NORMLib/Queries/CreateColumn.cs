using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public class CreateColumn<RowType> : TableQuery<RowType>, ICreateColumn<RowType>
	{

		private IColumn column;
		public IColumn Column
		{
			get { return column; }
		}

		public CreateColumn(IColumn Column)
		{
			this.column = Column;
		}


	}

}
