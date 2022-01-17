/*
    Atlas of Information Management business intelligence library and documentation database.
    Copyright (C) 2020  Riverside Healthcare, Kankakee, IL

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Atlas_Web.Models;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Data;
using System.Data.Odbc;
using Newtonsoft.Json;
using Atlas_Web.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Pages.Requests
{
    public class IndexModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IConfiguration _config;
        private IMemoryCache _cache;

        public IndexModel(Atlas_WebContext context, IConfiguration config, IMemoryCache cache)
        {
            _context = context;
            _config = config;
            _cache = cache;
        }

        public void OnGet() { }

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
                        this.status = r.Value;
                    }
                    else if (r.Name.LocalName.Equals("message"))
                    {
                        this.message = r.Value;
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
                                d => (string)d.Element("name").Value,
                                d => (string)d.Element("value").Value
                            )
                    );
                    this.details = dictList;
                }
                else
                {
                    foreach (var record in xdoc.Descendants("record"))
                    {
                        var dict = new Dictionary<string, string>();
                        dict = record
                            .Descendants("parameter")
                            .ToDictionary(
                                d => (string)d.Element("name").Value,
                                d => (string)d.Element("value").Value
                            );
                        dict.Add("URL", record.Attribute("URI").Value);
                        dictList.Add(dict);
                        this.details = dictList;
                    }
                }
            }

            public string status { get; set; }
            public string message { get; set; }
            public List<Dictionary<string, string>> details { get; set; }
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
            string Result;
            string xmlString;

            using (var stringWriter = new Utf8StringWriter())
            {
                using (var xmlWriter = new XmlTextWriter(stringWriter))
                {
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
                    xmlWriter.WriteElementString("value", "Atlas Access Request");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("parameter");
                    xmlWriter.WriteElementString("name", "description");
                    xmlWriter.WriteElementString(
                        "value",
                        "I would like access to the report '" + reportName + "' from " + reportUrl
                    );
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("parameter");
                    xmlWriter.WriteElementString("name", "purpose");
                    xmlWriter.WriteElementString(
                        "value",
                        "Atlas Access Request for '" + reportName + "'"
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
                    xmlWriter.WriteElementString("name", "requester");
                    xmlWriter.WriteElementString(
                        "value",
                        UserHelpers.GetUser(_cache, _context, User.Identity.Name).Fullname_Cust
                    );
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("parameter");
                    xmlWriter.WriteElementString("name", "item");
                    xmlWriter.WriteElementString("value", "Request Access");
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
                    xmlWriter.WriteElementString(
                        "value",
                        UserHelpers.GetUser(_cache, _context, User.Identity.Name).Email
                    );
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("parameter");
                    xmlWriter.WriteElementString("name", "director");
                    xmlWriter.WriteElementString("value", directorName);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndDocument();
                    // Spit out the XML document
                    xmlString = stringWriter.ToString();
                }
            }

            using (var client = new HttpClient())
            {
                // Compose the http POST request following the API guidelines
                // https://www.manageengine.com/products/service-desk/help/adminguide/api/request-operations.html#add
                var values = new Dictionary<string, string>
                {
                    { "OPERATION_NAME", "ADD_REQUEST" },
                    { "TECHNICIAN_KEY", _config["AppSettings:manage_engine_tech_key"] },
                    { "INPUT_DATA", xmlString }
                };

                var httpRequest = new FormUrlEncodedContent(values);
                // send the request asynchronously
                var httpResponse = await client.PostAsync(
                    _config["AppSettings:manage_engine_server"] + "/sdpapi/request/",
                    httpRequest
                );
                // notify the user of the response from the server http://stackoverflow.com/a/13550295/3900824
                ApiResponse response = new ApiResponse(
                    await httpResponse.Content.ReadAsStringAsync()
                );
                Result = response.status + " - " + response.message;
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
                using (var xmlWriter = new XmlTextWriter(stringWriter))
                {
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
                    xmlWriter.WriteElementString(
                        "value",
                        UserHelpers.GetUser(_cache, _context, User.Identity.Name).Fullname_Cust
                    );
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
                    xmlWriter.WriteElementString(
                        "value",
                        UserHelpers.GetUser(_cache, _context, User.Identity.Name).Email
                    );
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
            }

            using (var client = new HttpClient())
            {
                // Compose the http POST request following the API guidelines
                // https://www.manageengine.com/products/service-desk/help/adminguide/api/request-operations.html#add
                var values = new Dictionary<string, string>
                {
                    { "OPERATION_NAME", "ADD_REQUEST" },
                    { "TECHNICIAN_KEY", _config["AppSettings:manage_engine_tech_key"] },
                    { "INPUT_DATA", xmlString }
                };

                var httpRequest = new FormUrlEncodedContent(values);
                // send the request asynchronously
                var httpResponse = await client.PostAsync(
                    _config["AppSettings:manage_engine_server"] + "/sdpapi/request/",
                    httpRequest
                );
                // notify the user of the response from the server http://stackoverflow.com/a/13550295/3900824
                ApiResponse response = new ApiResponse(
                    await httpResponse.Content.ReadAsStringAsync()
                );
                Result = response.status + " - " + response.message;
            }
            return Content(Result);
        }
    }
}
