using NORMLib;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public interface IConnectionFactory
	{
		string DatabaseName
		{
			get;
		}
		DbConnection CreateConnectionToServer();
		DbConnection CreateConnectionToDatabase();
	}
}
