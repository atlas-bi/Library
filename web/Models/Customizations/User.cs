using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Atlas_Web.Helpers;

namespace Atlas_Web.Models
{
    public class User__Metadata { }

    [ModelMetadataType(typeof(User__Metadata))]
    public partial class User
    {
        [NotMapped]
        public string Fullname_Cust
        {
            get
            {
                if (FullName != null && FullName != "")
                {
                    return FullName;
                }
                else if (AccountName != null && AccountName != "")
                {
                    try
                    {
                        if (AccountName.Contains(","))
                        {
                            var newString = AccountName.ToLower().Replace(", ", ",").Split(' ')[
                                0
                            ].Split(',');
                            string Result = newString[1] + " " + newString[0];
                            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                            return textInfo.ToTitleCase(Result.ToLower());
                        }
                        else
                        {
                            var tryName = AccountName
                                .Replace(Settings.Property("org_ad_domain") + "\\", "")
                                .Replace("-", " ");
                            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                            return textInfo.ToTitleCase(tryName.ToLower());
                        }
                    }
                    catch
                    {
                        return AccountName;
                    }
                }
                else if (Username != null && Username != "")
                {
                    try
                    {
                        if (Username.Contains(","))
                        {
                            var newString = Username.ToLower().Replace(", ", ",").Split(' ')[
                                0
                            ].Split(',');
                            string Result = newString[1] + " " + newString[0];
                            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                            return textInfo.ToTitleCase(Result.ToLower());
                        }
                        else
                        {
                            var tryName = Username
                                .ToLower()
                                .Replace(Settings.Property("org_ad_domain").ToLower() + @"\", "")
                                .Replace("-", " ");
                            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                            return textInfo.ToTitleCase(tryName.ToLower());
                        }
                    }
                    catch
                    {
                        return Username;
                    }
                }
                return "user not found";
            }
        }

        [NotMapped]
        public string Firstname_Cust
        {
            get
            {
                if (FirstName != null && FirstName != "")
                {
                    return FirstName;
                }
                else if (AccountName != null && AccountName != "")
                {
                    try
                    {
                        if (AccountName.Contains(","))
                        {
                            var newString = AccountName.ToLower().Replace(", ", ",").Split(' ')[
                                0
                            ].Split(',');
                            string Result = newString[1];
                            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                            return textInfo.ToTitleCase(Result.ToLower());
                        }
                        else
                        {
                            var tryName = AccountName
                                .Replace(Settings.Property("org_ad_domain") + "\\", "")
                                .Split('-')[0];
                            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                            return textInfo.ToTitleCase(tryName.ToLower());
                        }
                    }
                    catch
                    {
                        return AccountName;
                    }
                }
                else if (Username != null && Username != "")
                {
                    //try
                    // {
                    if (Username.Contains(","))
                    {
                        var newString = Username.ToLower().Replace(", ", ",").Split(' ')[0].Split(
                            ','
                        );
                        string Result = newString[1];
                        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                        return textInfo.ToTitleCase(Result.ToLower());
                    }
                    else
                    {
                        var tryName = Username
                            .Replace(Settings.Property("org_ad_domain") + "\\", "")
                            .Split('-')[0];
                        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                        return textInfo.ToTitleCase(tryName.ToLower());
                    }
                    //}
                    //catch
                    //{
                    //    return Username;
                    //}
                }
                return "user not found";
            }
        }
    }
}
