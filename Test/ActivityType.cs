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
		public static readonly IntColumn<ActivityType> ActivityTypeIDColumn = new IntColumn<ActivityType>() { IsIdentity = true, IsPrimaryKey = true };
		public int? ActivityTypeID
		{
			get { return ActivityTypeIDColumn.GetValue(this); }
			set { ActivityTypeIDColumn.SetValue(this, value); }
		}
		public static readonly StringColumn<ActivityType> NameColumn = new StringColumn<ActivityType>();
		public string Name
		{
			get { return NameColumn.GetValue(this); }
			set { NameColumn.SetValue(this, value); }
		}
		public static readonly StringColumn<ActivityType> BackgroundColorColumn = new StringColumn<ActivityType>() { ForeignKey=ActivityType.ActivityTypeIDColumn };
		public string BackgroundColor
		{
			get { return BackgroundColorColumn.GetValue(this); }
			set { BackgroundColorColumn.SetValue(this, value); }
		}
		public static readonly StringColumn<ActivityType> TextColorColumn = new StringColumn<ActivityType>();
		public string TextColor
		{
			get { return TextColorColumn.GetValue(this); }
			set { TextColorColumn.SetValue(this, value); }
		}
		public static readonly IntColumn<ActivityType> LayerIDColumn = new IntColumn<ActivityType>() ;
		public int? LayerID
		{
			get { return LayerIDColumn.GetValue(this); }
			set { LayerIDColumn.SetValue(this, value); }
		}
		public static readonly BoolColumn<ActivityType> IsDisabledColumn = new BoolColumn<ActivityType>();
		public bool? IsDisabled
		{
			get { return IsDisabledColumn.GetValue(this); }
			set { IsDisabledColumn.SetValue(this, value); }
		}
		public static readonly IntColumn<ActivityType> MinEmployeesColumn = new IntColumn<ActivityType>() { IsNullable=true };
		public int? MinEmployees
		{
			get { return MinEmployeesColumn.GetValue(this); }
			set { MinEmployeesColumn.SetValue(this, value); }
		}
    
      
      
	}
}
