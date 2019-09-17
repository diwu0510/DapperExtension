using System;
using System.Collections.Generic;
using System.Text;
using HZC.Data.Dapper;

namespace Test
{
    public class MyDbContext : DapperContext<int>
    {
        public MyDbContext(string connectionString, Func<int> getUserKeyFunc) : base(connectionString, getUserKeyFunc)
        {
        }
    }
}
