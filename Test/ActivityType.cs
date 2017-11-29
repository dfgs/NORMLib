using NORMLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
	public class ActivityType
	{
		public static readonly Column<int?> ActivityTypeIDColumn = new Column<int?>() { IsIdentity = true, IsPrimaryKey = true };
		public int? ActivityTypeID
		{
			get { return ActivityTypeIDColumn.GetValue(this); }
			set { ActivityTypeIDColumn.SetValue(this, value); }
		}
		public static readonly Column<string> NameColumn = new Column<string>();
		public string Name
		{
			get { return NameColumn.GetValue(this); }
			set { NameColumn.SetValue(this, value); }
		}
		public static readonly Column<string> BackgroundColorColumn = new Column<string>() ;
		public string BackgroundColor
		{
			get { return BackgroundColorColumn.GetValue(this); }
			set { BackgroundColorColumn.SetValue(this, value); }
		}
		public static readonly Column<string> TextColorColumn = new Column<string>();
		public string TextColor
		{
			get { return TextColorColumn.GetValue(this); }
			set { TextColorColumn.SetValue(this, value); }
		}
		public static readonly Column<int?> LayerIDColumn = new Column<int?>() ;
		public int? LayerID
		{
			get { return LayerIDColumn.GetValue(this); }
			set { LayerIDColumn.SetValue(this, value); }
		}
		public static readonly Column<bool?> IsDisabledColumn = new Column<bool?>();
		public bool? IsDisabled
		{
			get { return IsDisabledColumn.GetValue(this); }
			set { IsDisabledColumn.SetValue(this, value); }
		}
		public static readonly Column<int?> MinEmployeesColumn = new Column<int?>() { IsNullable=true };
		public int? MinEmployees
		{
			get { return MinEmployeesColumn.GetValue(this); }
			set { MinEmployeesColumn.SetValue(this, value); }
		}
    
      
      
	}
}
