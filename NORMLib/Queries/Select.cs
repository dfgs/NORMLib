﻿using NORMLib.Columns;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public class Select<RowType> : ColumnsQuery<RowType>, ISelect<RowType>
	{
		
		private IColumn[] orders;
		public IEnumerable<IColumn> Orders
		{
			get { return orders; }
		}

		private Filter filter;
		public Filter Filter
		{
			get { return filter; }
		}

		private List<IJoin> joins;
		public IEnumerable<IJoin> Joins
		{
			get { return joins; }
		}

		public Select(params IColumn[] Columns):base(Columns)
		{
			joins = new List<IJoin>();
		}

		public override DbCommand CreateCommand(ICommandFactory CommandFactory)
		{
			return CommandFactory.CreateCommand(this);
		}

		public Select<RowType> Join<JoinedRowType, ValueType>(IColumn<JoinedRowType, ValueType> JoinedColumn, IColumn<ValueType> TargetColumn)
		{
			joins.Add(new Join<JoinedRowType,ValueType>(JoinedColumn, TargetColumn));
			return this;
		}

		public Select<RowType> OrderBy(params IColumn[] Columns)
		{
			this.orders = Columns;
			return this;
		}

		public Select<RowType> Where(Filter Filter)
		{
			this.filter = Filter;
			return this;
		}

		

		
	}

}
