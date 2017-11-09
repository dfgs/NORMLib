using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public abstract class ValueTypeColumn<ModelType,ValueType> : BaseColumn<ModelType, ValueType?>
		where ValueType:struct
	{
		public override Type ColumnType
		{
			get { return typeof(ValueType); }
		}

		public ValueTypeColumn(string Name = null) : base(Name)
		{

		}
	}

}
