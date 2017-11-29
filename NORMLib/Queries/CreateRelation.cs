using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public class CreateRelation<PrimaryRowType,ForeignRowType> : Query, ICreateRelation<PrimaryRowType,ForeignRowType>
	{

		private IColumn primaryColumn;
		public IColumn PrimaryColumn
		{
			get { return primaryColumn; }
		}
		private IColumn foreignColumn;
		public IColumn ForeignColumn
		{
			get { return foreignColumn; }
		}

		public string PrimaryTableName
		{
			get { return Table<PrimaryRowType>.Name; }
		}

		public string ForeignTableName
		{
			get { return Table<ForeignRowType>.Name; }
		}

		public CreateRelation(IColumn PrimaryColumn, IColumn ForeignColumn)
		{
			this.primaryColumn = PrimaryColumn;this.foreignColumn = ForeignColumn;
		}


	}

}
