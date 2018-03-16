using NORMLib.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NORMLib
{
	public class Join<JoinedRowType,ValueType> :IJoin
	{
		public string JoinedTableName
		{
			get { return Table<JoinedRowType>.Name; }
		}

		private IColumn<JoinedRowType,ValueType> joinedColumn;
		public IColumn<JoinedRowType, ValueType> JoinedColumn
		{
			get { return joinedColumn; }
		}
		IColumn IJoin.JoinedColumn
		{
			get { return joinedColumn; }
		}

		public string TargetTableName
		{
			get { return targetColumn?.TableName; }
		}

		private IColumn<ValueType> targetColumn;
		public IColumn<ValueType> TargetColumn
		{
			get { return targetColumn; }
		}
		IColumn IJoin.TargetColumn
		{
			get { return targetColumn; }
		}

		public Join(IColumn<JoinedRowType,ValueType> JoinedColumn, IColumn<ValueType> TargetColumn)
		{
			this.joinedColumn = JoinedColumn; this.targetColumn = TargetColumn;
		}

		


	}

}

