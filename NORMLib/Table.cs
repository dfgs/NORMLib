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
    public class Table<RowType>:ITable
    {
		private static IColumn primaryKey;
		public static IColumn PrimaryKey
		{
			get { return primaryKey; }
		}
		IColumn ITable.PrimaryKey
		{
			get { return primaryKey; }
		}

		private static IColumn identityColumn;
		public static IColumn IdentityColumn
		{
			get { return identityColumn; }
		}
		IColumn ITable.IdentityColumn
		{
			get { return identityColumn; }
		}

		private static string name;
		public static string Name
		{
			get { return name; }
		}

		string ITable.Name
		{
			get { return name; }
		}

		private static List<Tuple<int,IColumn>> columns;
		public static IEnumerable<IColumn> Columns
		{
			get { return columns.Select(item=>item.Item2); }
		}
		IEnumerable<IColumn> ITable.Columns
		{
			get { return columns.Select(item => item.Item2); }
		}


		private static int maxColumnRevision;
		public static int MaxColumnRevision
		{
			get { return maxColumnRevision; }
		}
		int ITable.MaxColumnRevision
		{
			get { return maxColumnRevision; }
		}

		static Table()
		{
			FieldInfo[] fis;
			TableAttribute tableAttribute;
			RevisionAttribute revisionAttribute;
			IColumn column;
			Type dataType;
			int revision;

			maxColumnRevision = 0;

			dataType = typeof(RowType);

			columns = new List<Tuple<int, IColumn>>();

			tableAttribute = dataType.GetCustomAttribute<TableAttribute>(true);
			name = tableAttribute?.Name ?? dataType.Name;

		
			fis = dataType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);
			foreach (FieldInfo fi in fis)
			{
				column = fi.GetValue(null) as IColumn;
				if (column == null) continue;
				if (column.IsPrimaryKey) primaryKey = column;
				if (column.IsIdentity) identityColumn = column;


				revisionAttribute = fi.FieldType.GetCustomAttribute<RevisionAttribute>(true);
				revision = revisionAttribute?.Value ?? 0;
				if (revision > maxColumnRevision) maxColumnRevision = revision;

				columns.Add(new Tuple<int,IColumn>(revision,column));
			}
			
			if (primaryKey == null)
				throw (new NotSupportedException("Missing primary key"));

		}

		public static IEnumerable<IColumn> GetColumns(int MinRevision, int MaxRevision = int.MaxValue)
		{
			return columns.Where(item => (item.Item1 >= MinRevision) && (item.Item1<=MaxRevision) ).Select(item => item.Item2);
		}
		IEnumerable<IColumn> ITable.GetColumns(int MinRevision, int MaxRevision)
		{
			return GetColumns(MinRevision, MaxRevision);
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
