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

using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Data_Governance_WebApp.Helpers;
using Microsoft.Extensions.Configuration;

namespace Data_Governance_WebApp.Models
{
    public class User__Metadata
    {
    }

    [ModelMetadataType(typeof(User__Metadata))]
    public partial class User
    {

        [NotMapped]
        public string Fullname_Cust
        {
            get
            {

                if (FullName != null && FullName != "") { return FullName; }

                else if (AccountName != null && AccountName != "")
                {
                    try
                    {
                        if (AccountName.Contains(","))
                        {
                            var newString = AccountName.ToLower().Replace(", ", ",").Split(' ')[0].Split(',');
                            string Result = newString[1] + " " + newString[0];
                            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                            return textInfo.ToTitleCase(Result.ToLower());

                        }
                        else
                        {
                            var tryName = AccountName.Replace(Settings.Property("org_ad_domain") + "\\", "").Replace("-", " ");
                            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                            return textInfo.ToTitleCase(tryName.ToLower());
                        }
                    } catch
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
                            var newString = Username.ToLower().Replace(", ", ",").Split(' ')[0].Split(',');
                            string Result = newString[1] + " " + newString[0];
                            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                            return textInfo.ToTitleCase(Result.ToLower());

                        }
                        else
                        {
                            var tryName = Username.ToLower().Replace(Settings.Property("org_ad_domain").ToLower() + @"\", "").Replace("-", " ");
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
                if (FirstName != null && FirstName != "") { return FirstName; }

                else if (AccountName != null && AccountName != "")
                {
                    try
                    {
                        if (AccountName.Contains(","))
                        {
                            var newString = AccountName.ToLower().Replace(", ", ",").Split(' ')[0].Split(',');
                            string Result = newString[1];
                            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                            return textInfo.ToTitleCase(Result.ToLower());

                        }
                        else
                        {
                            var tryName = AccountName.Replace(Settings.Property("org_ad_domain") + "\\", "").Split('-')[0];
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
                            var newString = Username.ToLower().Replace(", ", ",").Split(' ')[0].Split(',');
                            string Result = newString[1];
                            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                            return textInfo.ToTitleCase(Result.ToLower());

                        }
                        else
                        {
                            var l = Settings.Property("org_ad_domain");
                            var tryName = Username.Replace(Settings.Property("org_ad_domain") + "\\", "").Split('-')[0];
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
