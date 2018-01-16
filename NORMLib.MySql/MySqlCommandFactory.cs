using MySql.Data.MySqlClient;
using MySql.Data.Types;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib.MySql
{
	public class MySqlCommandFactory : CommandFactory<MySqlCommand>
	{

		protected override string OnFormatColumnName(IColumn Column)
		{
			return $"`{Column.Name}`";
		}
		protected override string OnFormatTableName(string TableName)
		{
			return $"`{TableName}`";
		}

		protected override void OnSetParameter(MySqlCommand Command, string Name, object Value)
		{
			Command.Parameters.AddWithValue(Name, Value);
		}

		
		private string GetTypeName(IColumn Column)
		{
			string result;
			Type type;

			type = Column.ColumnType;
			if (type.IsGenericType) type = type.GenericTypeArguments[0];

			switch (type.Name)
			{
				case "String":
					result = "longtext";
					break;
				case "Byte":
					result = "tinyint unsigned";
					break;
				case "Single":
					result = "float";
					break;
				case "Int32":
					result = "int";
					break;
				case "Int64":
					result = "bigint";
					break;
				case "Boolean":
					result = "bit";
					break;
				case "DateTime":
					result = "DateTime";
					break;
				case "TimeSpan":
					result = "bigint";
					break;
				default: throw (new NotImplementedException("Cannot convert CLR type " + Column.ColumnType.Name));

			}
			return result;
		}



		
		public override DbCommand CreateCommand(IDatabaseExists Query)
		{
			MySqlCommand command=new MySqlCommand("SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @Name");
			command.Parameters.AddWithValue("@Name", Query.DatabaseName);
			return command;
		}

		public override DbCommand CreateCommand(ICreateDatabase Query)
		{
			return new MySqlCommand($"CREATE DATABASE {Query.DatabaseName}");
		}

		public override DbCommand CreateCommand<RowType>(ITableExists<RowType> Query)
		{
			MySqlCommand command = new MySqlCommand("SELECT table_name FROM information_schema.tables where table_name=@Name and table_schema=DATABASE()");
			command.Parameters.AddWithValue("@Name", Query.TableName);
			return command;
		}

		public override DbCommand CreateCommand<RowType>(ICreateTable<RowType> Query)
		{
			string sql;

			sql = "CREATE TABLE " + OnFormatTableName(Query.TableName) + " (" +string.Join( ",", Query.Columns.Select(  column=>
			{
				return $"{OnFormatColumnName(column)} {GetTypeName(column)}{(column.IsNullable ? " NULL" : " NOT NULL")}{(column.IsIdentity? " AUTO_INCREMENT":"")}{(column.DefaultValue==null?"":$" default {column.DefaultValue}")}";
			})) + $",PRIMARY KEY ({Query.PrimaryKey.Name})) ENGINE=INNODB";


			return new MySqlCommand(sql);
		}

		public override DbCommand CreateCommand<RowType>(ICreateColumn<RowType> Query)
		{
			return new MySqlCommand($"ALTER TABLE {Query.TableName} ADD COLUMN ({OnFormatColumnName(Query.Column)} {GetTypeName(Query.Column)} {(Query.Column.IsNullable ? " NULL" : " NOT NULL")} {(Query.Column.IsIdentity ? " AUTO_INCREMENT" : "")}{(Query.Column.DefaultValue == null ? "" : $" default {Query.Column.DefaultValue}")})");
		}

		public override DbCommand CreateCommand<PrimaryRowType, ForeignRowType, ValueType>(ICreateRelation<PrimaryRowType, ForeignRowType, ValueType> Query)
		{
			return new MySqlCommand("ALTER TABLE " + Query.ForeignTableName+ " ADD FOREIGN KEY (" + Query.ForeignColumn.Name + ") REFERENCES " + Query.PrimaryTableName + "(" + Query.PrimaryColumn.Name + ")");
		}

		public override DbCommand CreateCommand(ISelectIdentity Query)
		{
			return new MySqlCommand("select @@identity");
		}


	}
}
