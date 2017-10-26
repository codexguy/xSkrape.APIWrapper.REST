using System;
using System.Collections.Generic;
using System.Text;

namespace xSkrape.APIWrapper
{
    internal class AvailableMailboxesResult
    {
        public IList<AvailableMailbox> Mailboxes;
        public bool Success;
        public string Source;
        public string Message;
    }

    internal class AvailableMailbox
    {
        public string Moniker;
        public string Title;
        public string Description;
    }

    internal class MultiValueResult
    {
        public IDictionary<string, string> v;
        public IDictionary<string, string> dt;
        public bool Success;
        public string Message;
        public string Source;
        public bool NoData;
        public bool RobotsTxtWarning;
        public bool Truncated;
    }

    internal class SingleValueResult
    {
        public string v;
        public string dt;
        public bool Success;
        public string Message;
        public string Source;
        public bool NoData;
        public bool RobotsTxtWarning;
        public bool Truncated;
    }

    internal class TableValueResult
    {
        public IList<TableValueColumn> c;
        public IList<TableValueRow> r;
        public string Message;
        public string Source;
        public bool NoData;
        public bool Success;
        public bool HasHeader;
        public bool RobotsTxtWarning;
        public bool Truncated;
    }

    internal class TableValueColumn
    {
        public string cn;
        public string dt;
        public string fmt;
    }

    internal class TableValueRow
    {
        public IList<string> v;
        public IList<string> n;
    }

    internal class SendMessageResult
    {
        public bool Success;
        public string Message;
        public string Source;
    }

    internal class SendRSSResult
    {
        public bool Success;
        public string Message;
        public string Source;
    }

    internal class CreateDataResult
    {
        public bool Success;
        public string Message;
        public string Source;
        public string[] Data;
    }
}
