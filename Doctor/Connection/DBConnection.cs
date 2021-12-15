using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Doctor.Connection
{
    class DBConnection
    {
        public static string myConn = $@"Data Source=DocAcc.db;Version=3;";
    }
}