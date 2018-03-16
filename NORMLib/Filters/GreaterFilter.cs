using NORMLib.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public class GreaterFilter:Filter
	{
		private IColumn column;
		public IColumn Column
		{
			get { return column; }
		}

		private object value;
		public object Value
		{
			get { return value; }
		}


		public GreaterFilter(IColumn Column, object Value)
		{
			this.column = Column; this.value = Value;//this.index = 0;
		}

		public override string ToString()
		{
			return $"{column} > {value}";
		}


	}
}
