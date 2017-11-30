using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Reflection;


namespace NORMLib
{
    public static class Table<RowType>
    {
		
		private static IColumn primaryKey;
		public static IColumn PrimaryKey
		{
			get { return primaryKey; }
		}
		

		private static IColumn identityColumn;
		public static IColumn IdentityColumn
		{
			get { return identityColumn; }
		}
		

		private static string name;
		public static string Name
		{
			get { return name; }
		}

		

		private static List<IColumn> columns;
		public static IEnumerable<IColumn> Columns
		{
			get { return columns; }
		}
		
		
		

		static Table()
		{
			FieldInfo[] fis;
			TableAttribute tableAttribute;
			IColumn column;
			Type dataType;


			dataType = typeof(RowType);

			columns = new List< IColumn>();

			tableAttribute = dataType.GetCustomAttribute<TableAttribute>(true);
			name = tableAttribute?.Name ?? dataType.Name;

		
			fis = dataType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);
			foreach (FieldInfo fi in fis)
			{
				column = fi.GetValue(null) as IColumn;
				if (column == null) continue;
				if (column.IsPrimaryKey) primaryKey = column;
				if (column.IsIdentity) identityColumn = column;

				columns.Add(column);
			}
			
			if (primaryKey == null)
				throw (new NotSupportedException("Missing primary key"));

		}

		
		




		public static void Clone(RowType Source, RowType Destination)
		{
			object value;
			foreach(IColumn column in columns)
			{
				if (column.IsIdentity)  continue;
				value = column.GetValue(Source);
				column.SetValue(Destination, value);
			}
		}

		public static bool AreEquals(RowType Source, RowType Destination)
		{
			object value1,value2;
			foreach (IColumn column in columns)
			{
				if (column.IsIdentity) continue;
				value1 = column.GetValue(Source);
				value2 = column.GetValue(Destination);
				if (!System.ValueType.Equals(value1, value2)) return false;
			}
			return true;
		}

		

	



	}
}
