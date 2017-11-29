using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	[AttributeUsage(AttributeTargets.Class)]
	public class DatabaseAttribute:Attribute
	{
		private string name;
		public string Name
		{
			get { return name; }
		}

		public DatabaseAttribute(string Name)
		{
			this.name = Name;
		}
	}



}
