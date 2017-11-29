using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public class Update<RowType>:FilterQuery<RowType>, IUpdate<RowType>
	{
		private RowType item;
		public RowType Item
		{
			get { return item; }
		}
		
		public Update(RowType Item, params IColumn[] Columns):base(Columns)
		{
			this.item = Item;
		}


	}

}
