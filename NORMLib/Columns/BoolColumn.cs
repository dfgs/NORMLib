using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NORMLib
{
	public class BoolColumn<ModelType>:ValueTypeColumn<ModelType,bool>
	{

		public BoolColumn([CallerMemberName]string Name=null):base(Name)
		{
		
		}
	}

	



}
