using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public class LowerOrEqualFilter<DataType>:Filter<DataType>
	{
		private IColumn<DataType> column;
		public IColumn<DataType> Column
		{
			get { return column; }
		}

		private object value;
		public object Value
		{
			get { return value; }
		}


		public LowerOrEqualFilter(IColumn<DataType> Column, object Value)
		{
			this.column = Column; this.value = Value;//this.index = 0;
		}

		public override string ToString()
		{
			return $"{column} <= {value}";
		}


	}
}
