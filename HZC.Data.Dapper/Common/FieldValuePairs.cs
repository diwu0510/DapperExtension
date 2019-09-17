using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace HZC.Data.Dapper.Common
{
    public class FieldValuePairs : List<FieldValuePair>
    {
        public static FieldValuePairs New()
        {
            return new FieldValuePairs();
        }

        public FieldValuePairs Add(string field, object value)
        {
            Add(new FieldValuePair {Field = field, Value = value, HasValue = true});
            return this;
        }

        public FieldValuePairs Add(string claus)
        {
            Add(new FieldValuePair {Field = claus, Value = null, HasValue = false});
            return this;
        }

        public StringParameterPair Invoke(string prefix = "@")
        {
            if (!this.Any())
            {
                return null;
            }

            var clauses = new List<string>();
            var parameters = new DynamicParameters();
            foreach (var pair in this)
            {
                if (pair.HasValue)
                {
                    clauses.Add($"{pair.Field}={prefix}{pair.Field}");
                    parameters.Add(pair.Field, pair.Value);
                }
                clauses.Add(pair.Field);
            }

            return new StringParameterPair { Sql = string.Join(",", clauses), Parameters = parameters };
        }
    }
}
