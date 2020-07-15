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
    public class UserFavorites__Metadata
    {
    }

    [ModelMetadataType(typeof(UserFavorites__Metadata))]
    public partial class UserFavorites
    {

        [NotMapped]
        public string ItemType_Proper
        {
            get
            {
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                return textInfo.ToTitleCase(ItemType.ToLower());
            }
        }
    }
}
