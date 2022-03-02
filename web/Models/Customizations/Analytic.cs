using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Atlas_Web.Helpers;

namespace Atlas_Web.Models
{
    public class Analytics__Metadata { }

    [ModelMetadataType(typeof(Analytics__Metadata))]
    public partial class Analytic
    {
        [NotMapped]
        public virtual int ObjectId
        {
            get
            {
                if (Search == null)
                {
                    return 0;
                }
                int Length;
                int StartIndex;

                if ((StartIndex = Search.IndexOf("=") + 1) == 0)
                {
                    return 0;
                }
                if (!Search.Contains("&", StringComparison.CurrentCulture))
                {
                    Length = Search.Length - StartIndex;
                }
                else
                {
                    Length = (Search.IndexOf("&") - StartIndex);
                }

                return Int32.TryParse(Search.Substring(StartIndex, Length), out int result)
                  ? result
                  : 0;
            }
        }

        public virtual string SearchString
        {
            get
            {
                if (Search == null || Search == "")
                {
                    return "None";
                }
                int Length;
                int StartIndex;

                if ((StartIndex = Search.IndexOf("=") + 1) == 0)
                {
                    return "None";
                }
                if (!Search.Contains("&", StringComparison.CurrentCulture))
                {
                    Length = Search.Length - StartIndex;
                }
                else
                {
                    Length = (Search.IndexOf("&") - StartIndex);
                }

                return "\""
                    + Search.Substring(StartIndex, Length).Replace("+", " ").Replace("%20", " ")
                    + "\"";
            }
        }

        [NotMapped]
        public virtual string AccessDateTimeDisplayString
        // don't display the time portion if > 24 hrs ago
        {
            get { return ModelHelpers.RelativeDate(AccessDateTime); }
        }
    }
}
