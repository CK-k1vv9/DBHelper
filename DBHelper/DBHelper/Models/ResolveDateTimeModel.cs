using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUtil
{
    public class ResolveDateTimeModel
    {
        public string Sql { get; set; }

        public object Value { get; set; }

        public ResolveDateTimeModel(string sql, object value)
        {
            Sql = sql;
            Value = value;
        }

    }
}
