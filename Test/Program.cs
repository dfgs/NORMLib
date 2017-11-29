using NORMLib;
using NORMLib.Sql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
	class Program
	{
		static void Main(string[] args)
		{
			ICommandFactory commandFactory = new SqlCommandFactory();
			System.Data.IDbConnection connection = new SqlConnection("Server=127.0.0.1;Database=ePlanifDatabase;Trusted_Connection=True;");
			IServer db;

			ActivityType activityType;
			activityType = new ActivityType() { BackgroundColor = "Red", IsDisabled = false, LayerID = 1, MinEmployees = 2, Name = "test", TextColor = "Red" };
		
			try
			{
				IQuery q = new Select<ActivityType>(ActivityType.ActivityTypeIDColumn, ActivityType.NameColumn).Where( ActivityType.ActivityTypeIDColumn.IsEqualToThan(2)  );
					
				
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			//Console.ReadLine();

		}
	}
}
