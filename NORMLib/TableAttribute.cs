using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	[AttributeUsage(AttributeTargets.Class)]
	public class TableAttribute:Attribute
	{
		private string name;
		public string Name
		{
			get { return name; }
		}

		public TableAttribute(string Name)
		{
			this.name = Name;
		}
	}



}
