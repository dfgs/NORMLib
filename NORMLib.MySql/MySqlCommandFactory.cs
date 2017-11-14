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

		protected override void OnSetParameter<DataType>(MySqlCommand Command, string Name, object Value)
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



		public override DbCommand CreateIdentityCommand<RowType>()
		{
			return new MySqlCommand("select  @@identity");
		}
		
		public override DbCommand CreateSelectDatabaseCommand(string DatabaseName)
		{
			MySqlCommand command=new MySqlCommand("SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @Name");
			command.Parameters.AddWithValue("@Name", DatabaseName);
			return command;
		}

		public override DbCommand CreateCreateDatabaseCommand(string DatabaseName)
		{
			return new MySqlCommand($"CREATE DATABASE {DatabaseName}");
		}

		public override DbCommand CreateDropDatabaseCommand(string DatabaseName)
		{
			return new MySqlCommand($"DROP DATABASE {DatabaseName}");
		}

		public override DbCommand CreateSelectTableCommand(ITable Table)
		{
			MySqlCommand command = new MySqlCommand("SELECT table_name FROM information_schema.tables where table_name=@Name and table_schema=DATABASE()");
			command.Parameters.AddWithValue("@Name", Table.Name);
			return command;

		}

		public override DbCommand CreateCreateTableCommand(ITable Table,IEnumerable<IColumn> Columns)
		{
			string sql;

			sql = "CREATE TABLE " + OnFormatTableName(Table.Name) + " (" +string.Join( ",", Columns.Select(  column=>
			{
				return $"{OnFormatColumnName(column)} {GetTypeName(column)}{(column.IsNullable ? " NULL" : " NOT NULL")}{(column.IsIdentity? " AUTO_INCREMENT":"")}";
			})) + $",PRIMARY KEY ({Table.PrimaryKey.Name})) ENGINE=INNODB";


			return new MySqlCommand(sql);
		}

		public override DbCommand CreateCreateColumnCommand(ITable Table,IColumn Column)
		{
			throw new NotImplementedException();
		}

		public override DbCommand CreateCreateRelationCommand(IRelation Relation)
		{
			return new MySqlCommand("ALTER TABLE " + Relation.ForeignTable + " ADD FOREIGN KEY (" + Relation.ForeignColumn.Name + ") REFERENCES " + Relation.PrimaryTable + "(" + Relation.PrimaryColumn.Name + ")");
		}





	}
}
