using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public class Delete<RowType> : TableQuery<RowType>, IDelete<RowType>
	{
		/*private RowType item;
		public RowType Item
		{
			get { return item; }
		}*/

		private Filter filter;
		public Filter Filter
		{
			get { return filter; }
		}

		
		public Delete(RowType Item)
		{
			this.filter = new EqualFilter(Table<RowType>.PrimaryKey, Table<RowType>.PrimaryKey.GetValue(Item));
		}
		public Delete()
		{
		}


		public IDelete<RowType> Where(Filter Filter)
		{
			this.filter = Filter;
			return this;
		}

		public override DbCommand CreateCommand(ICommandFactory CommandFactory)
		{
			return CommandFactory.CreateCommand(this);
		}


	}

}
