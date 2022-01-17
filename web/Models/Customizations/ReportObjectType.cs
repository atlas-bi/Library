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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas_Web.Models
{
    public class ReportObjectType__Metadata
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReportObjectTypeId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Name { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string DefaultEpicMasterFile { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public virtual ICollection<ReportObject> ReportObject { get; set; }
    }

    [ModelMetadataType(typeof(ReportObjectType__Metadata))]
    public partial class ReportObjectType
    {
    }
}
