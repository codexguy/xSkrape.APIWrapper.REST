using System;
using System.Data;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Dynamic;
using System.Collections;

namespace xSkrape.APIWrapper
{
    public static class Extensions
    {
        /// <summary>
        /// Allows for enumeration over a DataTable as an effective list of dynamic objects where columns can be accessed using .columnname
        /// </summary>
        /// <param name="dt">DataTable to transform</param>
        /// <returns></returns>
        public static IEnumerable<dynamic> AsDynamic(this DataTable dt)
        {
            foreach (DataRowView row in dt.DefaultView) yield return row.AsDynamic();
        }

        /// <summary>
        /// Allows for enumeration over a DataView as an effective list of dynamic objects where columns can be accessed using .columnname
        /// </summary>
        /// <param name="dv">DataView to transform</param>
        /// <returns></returns>
        public static IEnumerable<dynamic> AsDynamic(this DataView dv)
        {
            foreach (DataRowView row in dv) yield return row.AsDynamic();
        }

        /// <summary>
        /// Allows for access to columns of a DataRowView using .columnname notation
        /// </summary>
        /// <param name="row">DataRowView to access columns against</param>
        /// <returns></returns>
        public static dynamic AsDynamic(this DataRowView row)
        {
            return new DynamicDataRowView(row);
        }

        class DynamicDataRowView : DynamicObject
        {
            DataRowView _row;
            public DynamicDataRowView(DataRowView row) { _row = row; }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = _row[binder.Name];
                return true;
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                _row[binder.Name] = value;
                return true;
            }

            public override IEnumerable<string> GetDynamicMemberNames()
            {
                return _row.Row.Table.Columns.Cast<DataColumn>().Select(dc => dc.ColumnName);
            }
        }
    }
}
