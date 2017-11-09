using NORMLib;
using NORMLib.Sql;
using System;
using System.Collections.Generic;
using System.Data;
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
			IDbConnection connection = new SqlConnection("Server=127.0.0.1;Database=ePlanifDatabase;Trusted_Connection=True;");

			ActivityType activityType;
			activityType = new ActivityType() { BackgroundColor = "Red", IsDisabled = false, LayerID = 1, MinEmployees = 2, Name = "test", TextColor = "Red" };

			try
			{
				using (Session session = new Session(connection, commandFactory))
				{
					session.Insert(activityType);
					activityType.Name = "toto";
					session.Update(activityType);
					session.Delete(activityType);

					foreach(ActivityType a in session.Select<ActivityType>())
					{
						Console.WriteLine(a.Name);
					}
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			//Console.ReadLine();

		}
	}
}
