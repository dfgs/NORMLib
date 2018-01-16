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

		public static readonly Column<Revision,int?> RevisionIDColumn = new Column<Revision, int?>() { IsPrimaryKey=true, IsIdentity=true };
		public int? RevisionID
		{
			get { return RevisionIDColumn.GetValue(this); }
			set { RevisionIDColumn.SetValue(this, value); }
		}

		public static readonly Column<Revision, DateTime?> DateColumn = new Column<Revision, DateTime?>() ;
		public DateTime? Date
		{
			get { return DateColumn.GetValue(this); }
			set { DateColumn.SetValue(this, value); }
		}

		public static readonly Column<Revision, int?> ValueColumn = new Column<Revision, int?>() ;
		public int? Value
		{
			get { return ValueColumn.GetValue(this); }
			set { ValueColumn.SetValue(this, value); }
		}

	}
}
