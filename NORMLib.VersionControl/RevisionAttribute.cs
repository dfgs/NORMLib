using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib.VersionControl
{
	[AttributeUsage(AttributeTargets.Method)]
	public class RevisionAttribute:Attribute
	{
		public int Value
		{
			get;
			private set;
		}

		public RevisionAttribute(int Value)
		{
			this.Value = Value;
		}
	}
}
