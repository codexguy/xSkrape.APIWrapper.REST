using System;
using System.Data;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Dynamic;
using System.Collections;

namespace xSkrape.APIWrapper
{
    public static class xSkrapeREST
    {
        public async static Task<(bool success, string message, string source)> SendMessageDirect
            (string clientCode,
            IEnumerable<string> sendTo,
            string fromAddress,
            string subject,
            string body,
            MessageProvider defaultProvider = MessageProvider.EMAIL)
        {
            if (string.IsNullOrEmpty(clientCode))
                throw new ArgumentNullException("clientCode");
            if (string.IsNullOrEmpty(fromAddress))
                throw new ArgumentNullException("fromAddress");
            if (string.IsNullOrEmpty(subject))
                throw new ArgumentNullException("subject");
            if (string.IsNullOrEmpty(body))
                throw new ArgumentNullException("body");
            if (sendTo == null || !sendTo.Any())
                throw new ArgumentNullException("sendTo");

            using (var hc = new HttpClient())
            {
                string sendToText = WebUtility.UrlEncode(string.Join(";", sendTo.ToArray()));
                StringBuilder xsURL = new StringBuilder($"https://www.xs.codexframework.com/api/SendMessageDirect?cc={WebUtility.UrlEncode(clientCode)}&from={WebUtility.UrlEncode(fromAddress)}&body={WebUtility.UrlEncode(body)}&subject={WebUtility.UrlEncode(subject)}&sendto={sendToText}&defprov={defaultProvider.ToString()}");

                var rawResult = await hc.GetAsync(xsURL.ToString());

                if (!rawResult.IsSuccessStatusCode)
                {
                    return (false, $"{rawResult.StatusCode.ToString()} return value.", "Request");
                }

                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<SendMessageResult>(await rawResult.Content.ReadAsStringAsync());
                return (result.Success, result.Message, result.Source);
            }
        }

        public async static Task<(IDictionary<string, object> data, bool success, string message, string source, bool nodata, bool robotswarning, bool truncated)> GetMultiple(
            string clientCode,
            string url,
            IDictionary<string, string> queries,
            string dateTreatment = null)
        {
            if (string.IsNullOrEmpty(clientCode))
                throw new ArgumentNullException("clientCode");
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");
            if (queries == null || !queries.Any())
                throw new ArgumentNullException("query");

            using (var hc = new HttpClient())
            {
                var queryText = Newtonsoft.Json.JsonConvert.SerializeObject(queries);

                StringBuilder xsURL = new StringBuilder($"https://www.xs.codexframework.com/api/WebGetMultiple?cc={WebUtility.UrlEncode(clientCode)}&url={WebUtility.UrlEncode(url)}&queriesJSON={WebUtility.UrlEncode(queryText)}");

                if (!string.IsNullOrWhiteSpace(dateTreatment))
                {
                    xsURL.Append($"&sdt={WebUtility.UrlEncode(dateTreatment)}");
                }

                var rawResult = await hc.GetAsync(xsURL.ToString());

                if (!rawResult.IsSuccessStatusCode)
                {
                    return (null, false, $"{rawResult.StatusCode.ToString()} return value.", "Request", false, false, false);
                }

                var body = await rawResult.Content.ReadAsStringAsync();
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<MultiValueResult>(body);

                Dictionary<string, object> vals = new Dictionary<string, object>();

                if (result.Success && result.v != null)
                {
                    foreach (var kvp in result.v)
                    {
                        object data = null;
                        var dt = result.dt[kvp.Key];
                        
                        if (string.IsNullOrEmpty(dt))
                        {
                            data = kvp.Value;
                        }
                        else
                        {
                            if (string.Compare(dt, "DateTime", true) == 0)
                            {
                                if (long.TryParse(kvp.Value, out long jsval))
                                {
                                    data = new DateTime((jsval * 10000L) + 621355968000000000L);
                                }
                            }
                            else
                            {
                                data = Convert.ChangeType(kvp.Value, Type.GetType($"System.{dt}"));
                            }
                        }

                        vals[kvp.Key] = data;
                    }
                }

                return (vals, result.Success, result.Message, result.Source, result.NoData, result.RobotsTxtWarning, result.Truncated);
            }
        }

        public async static Task<(object data, bool success, string message, string source)> GetSingle(
            string clientCode,
            string url,
            string query,
            string dateTreatment = null)
        {
            if (string.IsNullOrEmpty(clientCode))
                throw new ArgumentNullException("clientCode");
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");
            if (string.IsNullOrEmpty(query))
                throw new ArgumentNullException("query");

            using (var hc = new HttpClient())
            {
                StringBuilder xsURL = new StringBuilder($"https://www.xs.codexframework.com/api/WebGetSingle?cc={WebUtility.UrlEncode(clientCode)}&url={WebUtility.UrlEncode(url)}&query={WebUtility.UrlEncode(query)}");

                if (!string.IsNullOrWhiteSpace(dateTreatment))
                {
                    xsURL.Append($"&sdt={WebUtility.UrlEncode(dateTreatment)}");
                }

                var rawResult = await hc.GetAsync(xsURL.ToString());

                if (!rawResult.IsSuccessStatusCode)
                {
                    return (null, false, $"{rawResult.StatusCode.ToString()} return value.", "Request");
                }

                var body = await rawResult.Content.ReadAsStringAsync();
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<SingleValueResult>(body);

                object data = null;

                if (result.Success)
                {
                    if (string.IsNullOrEmpty(result.dt))
                    {
                        data = result.v;
                    }
                    else
                    {
                        if (string.Compare(result.dt, "DateTime", true) == 0)
                        {
                            if (long.TryParse(result.v, out long jsval))
                            {
                                data = new DateTime((jsval * 10000L) + 621355968000000000L);
                            }
                        }
                        else
                        {
                            data = Convert.ChangeType(result.v, Type.GetType($"System.{result.dt}"));
                        }
                    }
                }

                return (data, result.Success, result.Message, result.Source);
            }
        }

        public async static Task<(IEnumerable<string> data, bool success, string message, string source)> GetRandomData(
            string clientCode, 
            string pattern, 
            int count, 
            int? uniquePercent = null)
        {
            using (var hc = new HttpClient())
            {
                StringBuilder xsURL = new StringBuilder($"https://www.xs.codexframework.com/api/GetRandomData?cc={WebUtility.UrlEncode(clientCode)}&pattern={WebUtility.UrlEncode(pattern)}&count={count}");

                if (uniquePercent.HasValue)
                {
                    xsURL.Append($"&areUniquePct={uniquePercent.Value}");
                }

                var rawResult = await hc.GetAsync(xsURL.ToString());

                if (!rawResult.IsSuccessStatusCode)
                {
                    return (null, false, $"{rawResult.StatusCode.ToString()} return value.", "Request");
                }

                var body = await rawResult.Content.ReadAsStringAsync();
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<CreateDataResult>(body);

                return (result.Data, result.Success, result.Message, result.Source);
            }
        }

        public async static Task<(string data, bool success, string message, string source, bool nodata, bool robotswarning, bool truncated)> GetSingleFromTable(
            string clientCode,
            string url,
            string tableColumnName,
            string tableQuery = null,
            string tableValueFilter = null,
            string tableValueSort = null,
            string dateTreatment = null)
        {
            if (string.IsNullOrEmpty(clientCode))
                throw new ArgumentNullException("clientCode");
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");
            if (string.IsNullOrEmpty(tableColumnName))
                throw new ArgumentNullException("tableColumnName");

            using (var hc = new HttpClient())
            {
                StringBuilder xsURL = new StringBuilder($"https://www.xs.codexframework.com/api/WebGetSingleFromTable?cc={WebUtility.UrlEncode(clientCode)}&url={WebUtility.UrlEncode(url)}&tableColumnName={WebUtility.UrlEncode(tableColumnName)}");

                if (!string.IsNullOrWhiteSpace(tableQuery))
                {
                    xsURL.Append($"&query={WebUtility.UrlEncode(tableQuery)}");
                }

                if (!string.IsNullOrWhiteSpace(tableValueFilter))
                {
                    xsURL.Append($"&tableValueFilter={WebUtility.UrlEncode(tableValueFilter)}");
                }

                if (!string.IsNullOrWhiteSpace(tableValueSort))
                {
                    xsURL.Append($"&tableValueSort={WebUtility.UrlEncode(tableValueSort)}");
                }

                if (!string.IsNullOrWhiteSpace(dateTreatment))
                {
                    xsURL.Append($"&sdt={WebUtility.UrlEncode(dateTreatment)}");
                }

                var rawResult = await hc.GetAsync(xsURL.ToString());

                if (!rawResult.IsSuccessStatusCode)
                {
                    return (null, false, $"{rawResult.StatusCode.ToString()} return value.", "Request", false, false, false);
                }

                var body = await rawResult.Content.ReadAsStringAsync();
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<SingleValueResult>(body);
                var data = result.v;

                if (string.Compare(result.dt, "DateTime", true) == 0)
                {
                    if (long.TryParse(data, out long jsval))
                    {
                        data = new DateTime((jsval * 10000L) + 621355968000000000L).ToString("O");
                    }
                }

                return (data, result.Success, result.Message, result.Source, result.NoData, result.RobotsTxtWarning, result.Truncated);
            }
        }

        public async static Task<(DataTable data, bool success, string message, string source, bool nodata, bool robotswarning, bool truncated, bool hasheader)> GetDataTable(
            string clientCode, 
            string url, 
            string tableQuery = null, 
            bool includeHeader = false, 
            string tableValueFilter = null, 
            string tableValueSort = null, 
            int? maxRows = null, 
            int? maxColumns = null, 
            string dateTreatment = null,
            IList<(string oldname, string newname)> headerRenaming = null)
        {
            if (string.IsNullOrEmpty(clientCode))
                throw new ArgumentNullException("clientCode");
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");

            using (var hc = new HttpClient())
            {
                StringBuilder xsURL = new StringBuilder($"https://www.xs.codexframework.com/api/WebGetTable?cc={WebUtility.UrlEncode(clientCode)}&url={WebUtility.UrlEncode(url)}&includeHeader={includeHeader}");

                if (!string.IsNullOrWhiteSpace(tableQuery))
                {
                    xsURL.Append($"&query={WebUtility.UrlEncode(tableQuery)}");
                }

                if (!string.IsNullOrWhiteSpace(tableValueFilter))
                {
                    xsURL.Append($"&tableValueFilter={WebUtility.UrlEncode(tableValueFilter)}");
                }

                if (!string.IsNullOrWhiteSpace(tableValueSort))
                {
                    xsURL.Append($"&tableValueSort={WebUtility.UrlEncode(tableValueSort)}");
                }

                if (!string.IsNullOrWhiteSpace(dateTreatment))
                {
                    xsURL.Append($"&sdt={WebUtility.UrlEncode(dateTreatment)}");
                }

                if (maxRows.HasValue)
                {
                    xsURL.Append($"&maxrow={maxRows.Value}");
                }

                if (maxColumns.HasValue)
                {
                    xsURL.Append($"&maxcol={maxColumns.Value}");
                }

                if (headerRenaming != null)
                {
                    StringBuilder renameExpr = new StringBuilder();

                    foreach (var re in headerRenaming)
                    {
                        if (renameExpr.Length > 0)
                        {
                            renameExpr.Append(" && ");
                        }

                        renameExpr.Append($"{re.oldname} becomes {re.newname}");
                    }

                    xsURL.Append($"&headerRenaming={WebUtility.UrlEncode(renameExpr.ToString())}");
                }

                var rawResult = await hc.GetAsync(xsURL.ToString());

                if (!rawResult.IsSuccessStatusCode)
                {
                    return (null, false, $"{rawResult.StatusCode.ToString()} return value.", "Request", false, false, false, false);
                }

                var body = await rawResult.Content.ReadAsStringAsync();
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<TableValueResult>(body);

                var dt = new DataTable();

                if (result.c != null && result.c.Count > 0)
                {
                    foreach (var c in result.c)
                    {
                        dt.Columns.Add(c.cn, Type.GetType($"System.{c.dt}"));
                    }

                    if (result.r != null && result.r.Count > 0)
                    {
                        dt.BeginLoadData();

                        foreach (var r in result.r)
                        {
                            var dr = dt.NewRow();

                            for (int ci = 0; ci < r.v.Count; ++ci)
                            {
                                if (Convert.ToInt32(r.n[ci]) == 0)
                                {
                                    if (string.Compare(result.c[ci].dt, "DateTime", true) == 0)
                                    {
                                        if (long.TryParse(r.v[ci], out long jsval))
                                        {
                                            dr[ci] = new DateTime((jsval * 10000L) + 621355968000000000L);
                                        }
                                    }
                                    else
                                    {
                                        dr[ci] = r.v[ci];
                                    }
                                }
                            }

                            dt.Rows.Add(dr);
                        }

                        dt.EndLoadData();
                    }
                }

                return (dt, result.Success, result.Message, result.Source, result.NoData, result.RobotsTxtWarning, result.Truncated, result.HasHeader);
            }
        }

    }
}
