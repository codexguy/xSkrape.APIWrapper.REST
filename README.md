# xSkrape.APIWrapper.REST
xSkrape provides data parsing for structured, semi and non-structured data sources. Extract tabular and discrete data from sources with minimal coding. Interact with HTML, JSON, XML, CSV, Excel and other sources over http/https using simple directives. Pull data from Google Docs, shape data from web API's, merge data over multiple requests, and more.
<br/><br/>
This assembly interacts with Web API services offered at www.xskrape.com. Most functionality requires a client key that can be obtained by creating a free account at xskrape.com, confirming your email address, and visiting the Queries pages under My Account. Note that you receive free credits each month, so the service can be used for free for most light to moderate usage. Heavier usage that would go past your free credit limit can be covered by purchasing additional usage credits. More details can be found here: https://www.xskrape.com/home/products. Examples of usage can be found here: https://github.com/codexguy/xSkrape.APIWrapper.REST/blob/master/xSkrape.APIWrapper.REST.Sample/RESTExamples.cs.
<br/><br/>
One example is for pulling tabular data from an HTML source, in this case a spreadsheet published in Google Docs:
<br/><br/>
<pre><code>var r = await xSkrapeREST.GetDataTable(CLIENT_KEY, "https://docs.google.com/spreadsheets/d/1r_gYGu8nawdIk7wpUrbL1evCqE0eygC-TZwVD9ViS-o/edit?usp=sharing", "columnname=Name");
</pre></code>
Of note, <i>one line of code</i> is all that's needed here to fully express <i>where</i> the data is, and a hint is provided about what it looks like (a column titled "Name"). As a second example:
<br/><br/>
<pre><code>var url = @"http://www.ndbc.noaa.gov/data/latest_obs/46042.rss";
Dictionary<string, string> queries = new Dictionary<string, string>()
{
{ "name", "firstelement=title" },
  { "windspeed", @"numberfollowsnear=Wind\ Speed" },
  { "winddir", @"followinginnertext=Wind\ Direction" },
  { "pubdate", @"xpath=/rss[1]/channel[1]/pubDate[1]/text()" }
};
var r = await xSkrapeREST.GetMultiple(CLIENT_KEY, url, queries);
</code></pre>
Here we're pulling four discrete values from a single page source using four different approaches. The last approach of using an xpath expression works for virtually all pages: even ill-formed HTML. However, the simplest approaches are to use simple matching terms like "numberfollowsnear" - very easy to understand and use. We even have a tool that can make suggestions about how to extract values and tables - see https://www.xskrape.com/Home/XSPageExplorer.
<br/><br/>
Looking for a feature or have a cool idea? Drop us a line, admin@codexframework.com.
