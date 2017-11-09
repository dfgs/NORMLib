using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public class StringColumn<ModelType> : BaseColumn<ModelType, string>
	{
		public override Type ColumnType
		{
			get { return typeof(string); }
		}

		public StringColumn([CallerMemberName]string Name = null) : base(Name)
		{

		}
	}

}
