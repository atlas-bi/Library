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
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Data_Governance_WebApp.Models
{
    [NotMapped]
    public class SearchResult
    {
        public SearchResult() { }
        public int Id { get; set; }
        public string ItemType { get; set; }
        public string SearchField { get; set; }
        public int TotalRecords { get; set; }
        public string Name { get; set; }
        public string EpicRecordId { get; set; }
        public string EpicMasterFile { get; set; }
        public string SourceServer { get; set; }
        public string ReportServerPath { get; set; }
        public string Description { get; set; }
        public string ReportType { get; set; }
        public int Documented { get; set; }
        public string ReportUrl { get; set; }
        public string EpicReportTemplateId { get; set; }
        public string ManageReportUrl { get; set; }
        public string EditReportUrl { get; set; }
        public int Hidden { get; set; }
        public int VisibleType { get; set; }
        public int Orphaned { get; set; }
    }
}
