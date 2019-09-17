using Dapper;

namespace HZC.Data.Dapper.Common
{
    public class StringParameterPair
    {
        public string Sql { get; set; }

        public DynamicParameters Parameters { get; set; }
    }
}
