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

    public static class xSkrapeREST
    {
        /// <summary>
        /// Send a message to SMS or email. Only available in REST package.
        /// </summary>
        /// <param name="clientCode">Obtained from the Queries page after logging into your xskrape.com account.</param>
        /// <param name="sendTo">Phone number or email address</param>
        /// <param name="fromAddress">Your email address (used to track results, etc.)</param>
        /// <param name="subject">Required subject line</param>
        /// <param name="body">Required message body</param>
        /// <param name="defaultProvider">Default is email, for SMS specify cell provider from list available at https://www.xskrape.com/Home/MsgProviderList</param>
        /// <returns></returns>
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

        /// <summary>
        /// Generate random data (e.g. names, addresses, etc.) that meet input criteria specs. Only available in REST package.
        /// </summary>
        /// <param name="clientCode">Obtained from the Queries page after logging into your xskrape.com account.</param>
        /// <param name="pattern">A pattern string that's described here: https://www.xskrape.com/Home/ObfuscationPatterns</param>
        /// <param name="count">Number of elements to return.</param>
        /// <param name="uniquePercent">An optional number from 0 to 100, where 100 implies all records must be unique.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Retrieve multiple discrete data values from a data source, returned in a bag.
        /// </summary>
        /// <param name="clientCode">Obtained from the Queries page after logging into your xskrape.com account.</param>
        /// <param name="url">URL spec as documented here http://www.codexframework.com/Documentation/XSFRUrlSpec</param>
        /// <param name="queries">Data query spec as documented here http://www.codexframework.com/Documentation/XSFRGetValueQuerySpec</param>
        /// <param name="dateTreatment">Valid values include "fromdate" which implies there is no date conversion undertaken (or an embedded explicit time zone within a matched date is used), or names a specific time zone which is assumed to apply to any matched date.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Retrieve a single, discrete value from a data source.
        /// </summary>
        /// <param name="clientCode">Obtained from the Queries page after logging into your xskrape.com account.</param>
        /// <param name="url">URL spec as documented here http://www.codexframework.com/Documentation/XSFRUrlSpec</param>
        /// <param name="query">Data query spec as documented here http://www.codexframework.com/Documentation/XSFRGetValueQuerySpec</param>
        /// <param name="dateTreatment">Valid values include "fromdate" which implies there is no date conversion undertaken (or an embedded explicit time zone within a matched date is used), or names a specific time zone which is assumed to apply to any matched date.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Retrieve a single value from a tabular data source. This operation is performed on the server.
        /// </summary>
        /// <param name="clientCode">Obtained from the Queries page after logging into your xskrape.com account.</param>
        /// <param name="url">URL spec as documented here http://www.codexframework.com/Documentation/XSFRUrlSpec</param>
        /// <param name="tableColumnName">Either the resolved column name from the matched table (as show for example in Page Explorer), or the column number (position-based, starting at zero). One can optionally apply one of the following aggregates against the column: SUM, AVG, MIN, MAX, COUNT, FIRST, LAST.</param>
        /// <param name="tableQuery">Table identification expression as documented http://www.codexframework.com/Documentation/XSFRGetTableQuerySpec</param>
        /// <param name="tableValueFilter">An optional filter expression that can be applied to the table to help isolate a candidate row or rows of interest.</param>
        /// <param name="tableValueSort">An optional string that contains the column name followed by "ASC" (ascending) or "DESC" (descending).</param>
        /// <param name="dateTreatment">Valid values include "fromdate" which implies there is no date conversion undertaken (or an embedded explicit time zone within a matched date is used), or names a specific time zone which is assumed to apply to any matched date.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Retrieve tabular data from a data source. Where tabular data might not be identified automatically, 'hints' can be provided to control how tabular data is found.
        /// </summary>
        /// <param name="clientCode">Obtained from the Queries page after logging into your xskrape.com account.</param>
        /// <param name="url">URL spec as documented here http://www.codexframework.com/Documentation/XSFRUrlSpec</param>
        /// <param name="tableQuery">Table identification expression as documented http://www.codexframework.com/Documentation/XSFRGetTableQuerySpec</param>
        /// <param name="includeHeader">If True, table header names are included in the returned data feed.</param>
        /// <param name="tableValueFilter">An optional filter expression that can be applied to the table to help isolate a candidate row or rows of interest.</param>
        /// <param name="tableValueSort">An optional string that contains the column name followed by "ASC" (ascending) or "DESC" (descending).</param>
        /// <param name="maxRows">Optionally restrict to a maximum number of rows.</param>
        /// <param name="maxColumns">Optionally restrict to a maximum number of columns.</param>
        /// <param name="dateTreatment">Valid values include "fromdate" which implies there is no date conversion undertaken (or an embedded explicit time zone within a matched date is used), or names a specific time zone which is assumed to apply to any matched date.</param>
        /// <param name="headerRenaming">An optional set of column renaming expressions of the format "OldColumnName becomes NewColumnName", delimited by "&&" if there are multiple. </param>
        /// <returns></returns>
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
