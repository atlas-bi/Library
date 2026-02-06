using System.Text;
using System.Xml;
using System.Xml.Linq;
using Atlas_Web.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlas_Web.Pages.Requests
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _config;

        public IndexModel(IConfiguration config)
        {
            _config = config;
        }

        public void OnGet()
        {
            // api is never directly accessed
        }

        public class ApiResponse
        {
            public ApiResponse(string xml)
            {
                // Constructor takes an xml string and converts to ApiResponse obj
                var xdoc = XDocument.Parse(xml);
                var result = xdoc.Descendants("result");
                foreach (var r in result.Elements())
                {
                    if (r.Name.LocalName.Equals("status"))
                    {
                        this.Status = r.Value;
                    }
                    else if (r.Name.LocalName.Equals("message"))
                    {
                        this.Message = r.Value;
                    }
                }
                var dictList = new List<Dictionary<string, string>>();
                if (xdoc.Descendants("operation").First().Attribute("name").Value != "GET_REQUESTS")
                // (note the plural)
                {
                    // a "Details" request will only have one dict to pass back, and doesn't have a "record" element
                    dictList.Add(
                        xdoc.Descendants("parameter")
                            .ToDictionary(
                                d => d.Element("name").Value,
                                d => d.Element("value").Value
                            )
                    );
                    this.Details = dictList;
                }
                else
                {
                    foreach (var record in xdoc.Descendants("record"))
                    {
                        var dict = record
                            .Descendants("parameter")
                            .ToDictionary(
                                d => d.Element("name").Value,
                                d => d.Element("value").Value
                            );

                        dict.Add("URL", record.Attribute("URI").Value);
                        dictList.Add(dict);
                        this.Details = dictList;
                    }
                }
            }

            public string Status { get; set; }
            public string Message { get; set; }
            public List<Dictionary<string, string>> Details { get; set; }
        }

        public class Utf8StringWriter : StringWriter
        {
            // it's possible the default UTF-16 encoding from the .NET stringwriter is screwing up the XML parser on ME???
            // use UTF-8 instead, which is the default for network stuff
            // http://stackoverflow.com/questions/25730816/how-to-return-xml-as-utf-8-instead-of-utf-16
            // Use UTF8 encoding but write no BOM to the wire
            public override Encoding Encoding
            {
                get { return new UTF8Encoding(false); }
            }
        }

        public async Task<ActionResult> OnPostAccessRequest(
            string reportName,
            string reportUrl,
            string directorName
        )
        {
            string Result; // <--- Make sure this matches below
            string xmlString;

            // Build XML payload
            using (var stringWriter = new Utf8StringWriter())
            {
                using var xmlWriter = new XmlTextWriter(stringWriter)
                {
                    Formatting = Formatting.Indented,
                };
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("Operation");
                xmlWriter.WriteStartElement("Details");

                void WriteParameter(string name, string value)
                {
                    xmlWriter.WriteStartElement("parameter");
                    xmlWriter.WriteElementString("name", name);
                    xmlWriter.WriteElementString("value", value);
                    xmlWriter.WriteEndElement();
                }

                WriteParameter("requesttemplate", "WebAPI");
                WriteParameter("subject", "Atlas Access Request");
                WriteParameter(
                    "description",
                    $"I would like access to the report '{reportName}' from {reportUrl}"
                );
                WriteParameter("Atlas Link", reportUrl);
                WriteParameter("category", "Epic Request");
                WriteParameter("requester", User.GetUserName());
                WriteParameter("item", "Request Access");
                WriteParameter("subcategory", "Atlas");
                WriteParameter("report_name", reportName);
                WriteParameter("requesteremail", User.GetUserEmail());
                WriteParameter("director", directorName);

                xmlWriter.WriteEndElement(); // Details
                xmlWriter.WriteEndElement(); // Operation
                xmlWriter.WriteEndDocument();

                xmlString = stringWriter.ToString();
            }

            // HttpClient ignoring SSL errors
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,
            };

            using var client = new HttpClient(handler);
            var values = new Dictionary<string, string>
            {
                { "OPERATION_NAME", "ADD_REQUEST" },
                { "TECHNICIAN_KEY", _config["AppSettings:manage_engine_tech_key"] },
                { "INPUT_DATA", xmlString },
            };

            var httpRequest = new FormUrlEncodedContent(values);

            HttpResponseMessage httpResponse;
            try
            {
                httpResponse = await client.PostAsync(
                    _config["AppSettings:manage_engine_server"] + "/sdpapi/request/",
                    httpRequest
                );
            }
            catch (Exception ex)
            {
                return Content($"HTTP request failed: {ex.Message}");
            }

            string responseText = await httpResponse.Content.ReadAsStringAsync();

            // DEBUG: log the response
            Console.WriteLine("ManageEngine Response:");
            Console.WriteLine(responseText);

            // Try parse XML safely
            try
            {
                if (responseText.TrimStart().StartsWith("<"))
                {
                    ApiResponse response = new(responseText);
                    Result = $"{response.Status} - {response.Message}";
                }
                else
                {
                    Result = $"Server returned non-XML response:\n{responseText}";
                }
            }
            catch (XmlException ex)
            {
                Result = $"Invalid XML received: {ex.Message}\n{responseText}";
            }

            return Content(Result);
        }

        public async Task<ActionResult> OnPostShareFeedback(
            string reportName,
            string reportUrl,
            string description
        )
        {
            string Result;
            string xmlString;

            using (var stringWriter = new Utf8StringWriter())
            {
                using var xmlWriter = new XmlTextWriter(stringWriter);
                xmlWriter.Formatting = System.Xml.Formatting.Indented;
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("Operation");
                xmlWriter.WriteStartElement("Details");
                xmlWriter.WriteStartElement("parameter");
                xmlWriter.WriteElementString("name", "requesttemplate");
                xmlWriter.WriteElementString("value", "WebAPI");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("parameter");
                xmlWriter.WriteElementString("name", "subject");
                xmlWriter.WriteElementString("value", "Atlas Feedback");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("parameter");
                xmlWriter.WriteElementString("name", "purpose");
                xmlWriter.WriteElementString(
                    "value",
                    "Atlas feedback reported on '" + reportName + "' from " + reportUrl
                );
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("parameter");
                xmlWriter.WriteElementString("name", "Atlas Link");
                xmlWriter.WriteElementString("value", reportUrl);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("parameter");
                xmlWriter.WriteElementString("name", "category");
                xmlWriter.WriteElementString("value", "Epic Request");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("parameter");
                xmlWriter.WriteElementString("name", "description");
                xmlWriter.WriteElementString("value", description);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("parameter");
                xmlWriter.WriteElementString("name", "requester");
                xmlWriter.WriteElementString("value", User.GetUserName());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("parameter");
                xmlWriter.WriteElementString("name", "item");
                xmlWriter.WriteElementString("value", "Problem/Issue");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("parameter");
                xmlWriter.WriteElementString("name", "subcategory");
                xmlWriter.WriteElementString("value", "Atlas");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("parameter");
                xmlWriter.WriteElementString("name", "report_name");
                xmlWriter.WriteElementString("value", reportName);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("parameter");
                xmlWriter.WriteElementString("name", "responsibility");
                xmlWriter.WriteElementString("value", "YES");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("parameter");
                xmlWriter.WriteElementString("name", "requesteremail");
                xmlWriter.WriteElementString("value", User.GetUserEmail());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("parameter");
                xmlWriter.WriteElementString("name", "director");
                xmlWriter.WriteElementString("value", "dryan@rhc.net");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("parameter");
                xmlWriter.WriteElementString("name", "responsible_person");
                xmlWriter.WriteElementString("value", "dryan@rhc.net");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                // Spit out the XML document
                xmlString = stringWriter.ToString();
            }

            using (var client = new HttpClient())
            {
                // Compose the http POST request following the API guidelines
                // https://www.manageengine.com/products/service-desk/help/adminguide/api/request-operations.html#add
                var values = new Dictionary<string, string>
                {
                    { "OPERATION_NAME", "ADD_REQUEST" },
                    { "TECHNICIAN_KEY", _config["AppSettings:manage_engine_tech_key"] },
                    { "INPUT_DATA", xmlString },
                };

                var httpRequest = new FormUrlEncodedContent(values);
                // send the request asynchronously
                var httpResponse = await client.PostAsync(
                    _config["AppSettings:manage_engine_server"] + "/sdpapi/request/",
                    httpRequest
                );
                // notify the user of the response from the server http://stackoverflow.com/a/13550295/3900824
                ApiResponse response = new(await httpResponse.Content.ReadAsStringAsync());
                Result = response.Status + " - " + response.Message;
            }
            return Content(Result);
        }
    }
}
