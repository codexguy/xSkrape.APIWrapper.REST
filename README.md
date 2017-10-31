# xSkrape.APIWrapper.REST
xSkrape provides data parsing for structured, semi and non-structured data sources. Extract tabular and discrete data from sources with minimal coding. Interact with HTML, JSON, XML, CSV, Excel and other sources over http/https using simple directives. Pull data from Google Docs, shape data from web API's, merge data over multiple requests, and more.
<br/><br/>
This assembly interacts with Web API services offered at www.xskrape.com. Most functionality requires a client key that can be obtained by creating a free account at xskrape.com, confirming your email address, and visiting the Queries pages under My Account. Note that you receive free credits each month, so the service can be used for free for most light to moderate usage. Heavier usage that would go past your free credit limit can be covered by purchasing additional usage credits. More details can be found here: https://www.xskrape.com/home/products. Example usage can be found here: https://github.com/codexguy/xSkrape.APIWrapper.REST/blob/master/xSkrape.APIWrapper.REST.Sample/RESTExamples.cs.
<br/><br/>
One example usage is for pulling tabular data from an HTML source, in this case a spreadsheet published in Google Docs:
<br/><br/>
<code>
var r = await xSkrapeREST.GetDataTable(CLIENT_KEY, "https://docs.google.com/spreadsheets/d/1r_gYGu8nawdIk7wpUrbL1evCqE0eygC-TZwVD9ViS-o/edit?usp=sharing", "columnname=Name");
</code>
<br/><br/>
Of note, <i>one line</i> of code is all that's needed here to fully express <i>where</i> the data is, and a hint is provided about what it looks like (a column titled "Name").
<br/><br/>
Looking for a feature or have a cool idea? Drop me a line, admin@codexframework.com.
