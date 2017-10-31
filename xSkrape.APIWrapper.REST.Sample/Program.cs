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
            // Credit usage is explained more here: https://www.xskrape.com/home/products - usage is generally FREE up until a certain level of usage, or in the case of the in-process libraries (fixed charge per machine per month), your first month per machine can often be covered by free monthly credits as a "trial" period
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
            RESTExamples.Example7_ExtractMultipleElementsFromSinglePage().Wait();
            RESTExamples.Example8_IsolateTabularDataOfInterestUsingTableIdentificationExpression().Wait();
            RESTExamples.Example9_ExtractAndFlattenJSONOnHtmlPage().Wait();
            RESTExamples.Example10_ExtractDataFromAnExcelFileLocatedInAZipFileStoredOnDropBox().Wait();
            RESTExamples.Example11_TabularDataFromGoogleDocsSpreadsheet().Wait();
            RESTExamples.Example12_PositionalBasedTabularDataInsidePreElement().Wait();
        }
    }
}
