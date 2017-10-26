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
        public static IEnumerable<dynamic> AsDynamic(this DataTable dt)
        {
            foreach (DataRowView row in dt.DefaultView) yield return row.AsDynamic();
        }

        public static IEnumerable<dynamic> AsDynamic(this DataView dv)
        {
            foreach (DataRowView row in dv) yield return row.AsDynamic();
        }

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

    public enum MessageProvider
    {
        EMAIL = 0,
        ALLTEL = 1,
        ATT = 2,
        ATTENTPAGING = 3,
        BELLMOB = 4,
        BOOSTMOB = 5,
        CRICKET = 6,
        FIDO = 7,
        HELIO = 8,
        IRIDIUM = 9,
        METROPCS = 10,
        MOBIPCSHI = 11,
        NEXTEL = 12,
        ROGERS = 13,
        SPRINT = 14,
        TELUSMOB = 15,
        THUMB = 16,
        TMOBILEUK = 17,
        TMOBILE = 18,
        UNICEL = 19,
        USCELLULAR = 20,
        VERIZON = 21,
        VIRGINMOB = 22,
        VODACOMZA = 23,
        VODAFONEIT = 24
    }

}
