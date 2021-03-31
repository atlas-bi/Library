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
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Atlas_Web.Models
{
    public class MailDrafts__Metadata
    {
    }

    [ModelMetadataType(typeof(MailDrafts__Metadata))]
    public partial class MailDraft
    {
        [NotMapped]
        public virtual string EditDate_MessagePreview
        // don't display the time portion if > 24 hrs ago
        {
            get
            {

                if (EditDate == null) { return ""; }
                var timeAgo = System.DateTime.Now.Subtract(EditDate ?? DateTime.Now);
                if (timeAgo.TotalDays < 1) { return (EditDate ?? DateTime.Now).ToString("h:mm tt"); }
                else if (timeAgo.TotalHours < 2) { return "Yesterday"; }
                else return (EditDate ?? DateTime.Now).ToString("M/d/yy");
            }
        }


        [NotMapped]
        public virtual string SmallSubject
        {
            get
            {
                if (Subject == null) { return ""; }
                return string.Join(" ", Subject.Split(' ').ToList().Take(5));
            }
        }

        [NotMapped]
        public virtual string SmallMessage
        {
            get
            {
                if (MessagePlainText is null) return "";
                return string.Join(" ", MessagePlainText.Split(' ').ToList().Take(10));
            }
        }

    }
}
