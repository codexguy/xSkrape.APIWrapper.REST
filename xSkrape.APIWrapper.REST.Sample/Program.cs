using System;
using System.Data;
using System.Text;
using System.Net;
using System.Collections.Generic;
using xSkrape.APIWrapper;

namespace APIWrapperDemo
{
    static class Program
    {
        static void Main(string[] args)
        {
            // Substitute your own client key, obtained by visiting https://www.xskrape.com, registering and adding a client query end-point
            // Credit usage is explained more here: https://www.xskrape.com/addcredits - usage is generally FREE up until a certain level of usage, or in the case of the in-process libraries (fixed charge per machine per month), your first month per machine can often be covered by free monthly credits as a "trial" period
            // Some examples below require your own client keys, etc. and may not return successful results without these being substituted in the code below (we have tested all examples with our own keys and got passes)
            // Something on your wish list? Have your own unique use cases? Contact me, community@codexframework.com
            // Other useful links:
            //    http://www.codexframework.com/Documentation/XSRefGuide?ProdFilter=xsapi 
            //    https://www.xskrape.com/Home/XSPageExplorer
            //    https://www.xskrape.com/Home/XSSQL

            RESTExamples.Example1_TabularFromCSVHttp().Wait();
            RESTExamples.Example2_TabularMultiPagedHTMLGrid().Wait();
            RESTExamples.Example3_ClientSideAggregationOverTabularResults().Wait();
            RESTExamples.Example4_MergeRequestFromWebAPIReturningShapedJSON().Wait();
            RESTExamples.Example5_SingleValueFromRSSFeed().Wait();
            RESTExamples.Example6_MergeMultipleRSSRequestsExtractingByLineNumber().Wait();
            RESTExamples.Example7_ExtractMultipleElementsFromSinglePage().Wait();           // todo - good one to test date parsing further
            RESTExamples.Example8_IsolateTabularDataOfInterestUsingTableIdentificationExpression().Wait();
            RESTExamples.Example9_ExtractAndFlattenJSONOnHtmlPage().Wait();
            RESTExamples.Example10_ExtractDataFromAnExcelFileLocatedInAZipFileStoredOnDropBox().Wait();
            RESTExamples.Example11_TabularDataFromGoogleDocsSpreadsheet().Wait();
            RESTExamples.Example12_PositionalBasedTabularDataInsidePreElement().Wait();
        }

        public static string RenderDataTable(this DataTable dt)
        {
            int maxWidth = 20;

            StringBuilder sb = new StringBuilder();

            foreach (DataColumn dc in dt.Columns)
            {
                sb.Append(dc.ColumnName.Left(maxWidth) + "  ");
            }
            sb.Append(Environment.NewLine);

            foreach (DataRow dr in dt.Rows)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    sb.Append(dr[dc].ToString().Left(maxWidth) + "  ");
                }
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        public static string Left(this string s, int length)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            if (s.Length > length)
            {
                return s.Substring(0, length - 2) + "..";
            }

            return s.PadRight(length);
        }
    }
}
