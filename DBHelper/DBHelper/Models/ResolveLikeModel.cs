using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUtil
{
    public class ResolveLikeModel
    {
        public string Sql { get; set; }

        public object Value { get; set; }

        public ResolveLikeModel(string sql, object value)
        {
            Sql = sql;
            Value = value;
        }
    }
}
