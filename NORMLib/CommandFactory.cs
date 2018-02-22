using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public abstract class CommandFactory<CommandType> : ICommandFactory
		where CommandType : DbCommand, new()
	{

		#region filters
		protected virtual string OnCreateEqualFilter(EqualFilter Filter, List<Tuple<string, object>> Parameters,ref int Index)
		{
			Tuple<string, object> parameter;
			parameter = new Tuple<string, object>(OnCreateParameterName(Filter.Column, ref Index), Filter.Value); Parameters.Add(parameter);
			if (Filter.Value == null) return $"{OnFormatTableName(Filter.Column.TableName)}.{OnFormatColumnName(Filter.Column)} is null";
			else return $"{OnFormatTableName(Filter.Column.TableName)}.{OnFormatColumnName(Filter.Column)} = {parameter.Item1}";
		}
		protected virtual string OnCreateGreaterFilter(GreaterFilter Filter, List<Tuple<string, object>> Parameters, ref int Index)
		{
			Tuple<string, object> parameter;
			parameter = new Tuple<string, object>(OnCreateParameterName(Filter.Column, ref Index), Filter.Value); Parameters.Add(parameter);
			if (Filter.Value == null) throw (new ArgumentNullException("Value"));
			else return $"{OnFormatTableName(Filter.Column.TableName)}.{OnFormatColumnName(Filter.Column)} > {parameter.Item1}";
		}
		protected virtual string OnCreateLowerFilter(LowerFilter Filter, List<Tuple<string, object>> Parameters, ref int Index)
		{
			Tuple<string, object> parameter;
			parameter = new Tuple<string, object>(OnCreateParameterName(Filter.Column, ref Index), Filter.Value); Parameters.Add(parameter);
			if (Filter.Value == null) throw (new ArgumentNullException("Value"));
			else return $"{OnFormatTableName(Filter.Column.TableName)}.{OnFormatColumnName(Filter.Column)} < {parameter.Item1}";
		}
		protected virtual string OnCreateGreaterOrEqualFilter(GreaterOrEqualFilter Filter, List<Tuple<string, object>> Parameters, ref int Index)
		{
			Tuple<string, object> parameter;
			parameter = new Tuple<string, object>(OnCreateParameterName(Filter.Column, ref Index), Filter.Value); Parameters.Add(parameter);
			if (Filter.Value == null) throw (new ArgumentNullException("Value"));
			else return $"{OnFormatTableName(Filter.Column.TableName)}.{OnFormatColumnName(Filter.Column)} >= {parameter.Item1}";
		}
		protected virtual string OnCreateLowerOrEqualFilter(LowerOrEqualFilter Filter, List<Tuple<string, object>> Parameters, ref int Index)
		{
			Tuple<string, object> parameter;
			parameter = new Tuple<string, object>(OnCreateParameterName(Filter.Column, ref Index), Filter.Value); Parameters.Add(parameter);
			if (Filter.Value == null) throw (new ArgumentNullException("Value"));
			else return $"{OnFormatTableName(Filter.Column.TableName)}.{OnFormatColumnName(Filter.Column)} <= {parameter.Item1}";
		}
		protected virtual string OnCreateAndFilter(AndFilter Filter, List<Tuple<string, object>> Parameters, ref int Index)
		{
			string result;

			if ((Filter.Filters == null) || (Filter.Filters.Length == 0)) throw (new ArgumentNullException("Filters")); ;

			result = "(" + OnCreateFilter(Filter.Filters[0], Parameters,ref Index) + ")";
			for (int t = 1; t < Filter.Filters.Length; t++)
			{
				result += " AND (" + OnCreateFilter(Filter.Filters[t], Parameters,ref Index) + ")";
			}

			return result;
		}
		protected virtual string OnCreateOrFilter(OrFilter Filter, List<Tuple<string, object>> Parameters, ref int Index)
		{
			string result;

			if ((Filter.Filters == null) || (Filter.Filters.Length == 0)) throw (new ArgumentNullException("Filters")); ;

			result = "(" + OnCreateFilter(Filter.Filters[0], Parameters,ref Index) + ")";
			for (int t = 1; t < Filter.Filters.Length; t++)
			{
				result += " OR (" + OnCreateFilter(Filter.Filters[t], Parameters,ref Index) + ")";
			}

			return result;
		}

		private string OnCreateFilter(Filter Filter, List<Tuple<string, object>> Parameters,ref int Index)
		{

			if (Filter is EqualFilter) return OnCreateEqualFilter((EqualFilter)Filter, Parameters, ref Index);
			else if (Filter is GreaterFilter) return OnCreateGreaterFilter((GreaterFilter)Filter, Parameters, ref Index);
			else if (Filter is LowerFilter) return OnCreateLowerFilter((LowerFilter)Filter, Parameters, ref Index);
			else if (Filter is GreaterOrEqualFilter) return OnCreateGreaterOrEqualFilter((GreaterOrEqualFilter)Filter, Parameters, ref Index);
			else if (Filter is LowerOrEqualFilter) return OnCreateLowerOrEqualFilter((LowerOrEqualFilter)Filter, Parameters, ref Index);
			else if (Filter is AndFilter) return OnCreateAndFilter((AndFilter)Filter, Parameters, ref Index);
			else if (Filter is OrFilter) return OnCreateOrFilter((OrFilter)Filter, Parameters, ref Index);

			else throw (new InvalidOperationException("Unsupported filter"));
		}
		#endregion


		protected virtual string OnFormatColumnName(IColumn Column)
		{
			return $"[{Column.Name}]";
		}

		protected virtual string OnFormatTableName(string TableName)
		{
			return $"[{TableName}]";
		}

		protected virtual string OnCreateParameterName(IColumn Column, ref int Index)
		{
			return $"@_{Column.Name}{Index++}";
		}

		protected virtual object OnConvertToDbValue(IColumn Column, object Component)
		{
			object value;

			value = Column.GetValue(Component);
			if (value == null) return DBNull.Value;

			return value;
		}

		protected virtual object OnConvertFromDbValue(IColumn Column, object Value)
		{
			if (Column.ColumnType.IsEnum) return Enum.ToObject(Column.ColumnType, Value);
			if (Value == DBNull.Value) return null;
			return Value;
		}


		public object ConvertToDbValue(IColumn Column, object Component)
		{
			return OnConvertToDbValue(Column, Component);
		}

		public object ConvertFromDbValue(IColumn Column, object Value)
		{
			return OnConvertFromDbValue(Column, Value);
		}


		protected abstract void OnSetParameter(CommandType Command, string Name, object Value);

		


		public DbCommand CreateCommand<RowType>(ISelect<RowType> Query)
		{
			string sql;
			CommandType command;
			List<Tuple<string, object>> parameters;
			int index = 0;
			IEnumerable<IGrouping<string, IJoin>> joinGoups;

			parameters = new List<Tuple<string, object>>();

			sql = "select " + string.Join(",", Query.Columns.Select(item => $"{OnFormatTableName(item.TableName)}.{OnFormatColumnName(item)}"));
			sql += " from " + OnFormatTableName(Query.TableName);

			if (Query.Joins.FirstOrDefault()!=null)
			{
				joinGoups=Query.Joins.GroupBy<IJoin, string>( item=> item.JoinedTableName );
				foreach(IGrouping<string,IJoin> group in joinGoups)
				{
					sql += $" inner join {group.Key} ON ";
					sql += string.Join(" AND ", group.Select( item=> $"{OnFormatTableName(item.JoinedTableName)}.{OnFormatColumnName(item.JoinedColumn)} = {OnFormatTableName(item.TargetTableName)}.{OnFormatColumnName(item.TargetColumn)}"));
				}
			}

			if (Query.Filter != null)
			{
				sql += " where " + OnCreateFilter(Query.Filter, parameters,ref index);
			}

			if (Query.Orders != null)
			{
				sql += " order by " + string.Join(",", Query.Orders.Select(item => OnFormatColumnName(item)));
			}

			command = new CommandType();
			command.CommandText = sql;

			foreach (Tuple<string, object> parameter in parameters)
			{
				OnSetParameter(command, parameter.Item1, parameter.Item2);
			}

			return command;
		}

		public DbCommand CreateCommand<RowType>(IInsert<RowType> Query)
		{
			string sql;
			CommandType command;
			int index = 0;

			sql = "insert into " + OnFormatTableName(Query.TableName);
			sql += " (" + string.Join(",",Query.Columns.Where(item=>!item.IsIdentity).Select(item => OnFormatColumnName(item))) + ") values (";
			sql += string.Join(",", Query.Columns.Where(item => !item.IsIdentity).Select(item => OnCreateParameterName(item, ref index))) + ")";

			command = new CommandType();
			command.CommandText = sql;
			index = 0;
			foreach (IColumn column in Query.Columns.Where(item => !item.IsIdentity))
			{
				OnSetParameter(command, OnCreateParameterName(column, ref index), OnConvertToDbValue(column, Query.Item));
			}

			return command;
		}

		public DbCommand CreateCommand<RowType>(IUpdate<RowType> Query)
		{
			string sql;
			CommandType command;
			List<Tuple<string, object>> parameters;
			int index=0;

			parameters = new List<Tuple<string, object>>();

			sql = "update " + OnFormatTableName(Query.TableName) + " set ";
			sql += string.Join(",", Query.Columns.Where(item=>!item.IsIdentity).Select(item => OnFormatColumnName(item) + "=" + OnCreateParameterName(item, ref index)));

			if (Query.Filter != null)
			{
				sql += " where " + OnCreateFilter(Query.Filter, parameters,ref index);
			}

			command = new CommandType();
			command.CommandText = sql;

			index = 0;
			foreach (IColumn column in Query.Columns.Where(item => !item.IsIdentity))
			{
				OnSetParameter(command, OnCreateParameterName(column, ref index), OnConvertToDbValue(column, Query.Item));
			}

			foreach (Tuple<string, object> parameter in parameters)
			{
				OnSetParameter(command, parameter.Item1, parameter.Item2);
			}

			return command;
		}

		public DbCommand CreateCommand<RowType>(IDelete<RowType> Query)
		{
			string sql;
			CommandType command;
			List<Tuple<string, object>> parameters;
			int index = 0;

			parameters = new List<Tuple<string, object>>();

			sql = "delete from " + OnFormatTableName(Query.TableName);

			if (Query.Filter != null)
			{
				sql += " where " + OnCreateFilter(Query.Filter, parameters,ref index);
			}

			command = new CommandType();
			command.CommandText = sql;

			index = 0;
			foreach (Tuple<string, object> parameter in parameters)
			{
				OnSetParameter(command, parameter.Item1, parameter.Item2);
			}

			return command;
		}

		public abstract DbCommand CreateCommand(IDatabaseExists Query);
		public abstract DbCommand CreateCommand(ICreateDatabase Query);
		public abstract DbCommand CreateCommand(ISelectIdentity Query);
		public abstract DbCommand CreateCommand<RowType>(ITableExists<RowType> Query);
		public abstract DbCommand CreateCommand<RowType>(ICreateTable<RowType> Query);
		public abstract DbCommand CreateCommand<RowType>(ICreateColumn<RowType> Query);
		public abstract DbCommand CreateCommand<PrimaryRowType, ForeignRowType, ValueType>(ICreateRelation<PrimaryRowType, ForeignRowType,ValueType> Query);
	}
}

	
