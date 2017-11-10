using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib.VersionControl
{
	public abstract class Commit
	{
		public abstract void Execute(IVersionController VersionController);
	}
}
