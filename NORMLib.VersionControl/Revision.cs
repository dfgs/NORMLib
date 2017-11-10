using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NORMLib;

namespace NORMLib.VersionControl
{
	public class Revision
	{

		public static readonly Column<int?> RevisionIDColumn = new Column<int?>() { IsPrimaryKey=true, IsIdentity=true };
		public int? RevisionID
		{
			get { return RevisionIDColumn.GetValue(this); }
			set { RevisionIDColumn.SetValue(this, value); }
		}

		public static readonly Column<DateTime?> DateColumn = new Column<DateTime?>() ;
		public DateTime? Date
		{
			get { return DateColumn.GetValue(this); }
			set { DateColumn.SetValue(this, value); }
		}

		public static readonly Column<int?> ValueColumn = new Column<int?>() ;
		public int? Value
		{
			get { return ValueColumn.GetValue(this); }
			set { ValueColumn.SetValue(this, value); }
		}

	}
}
