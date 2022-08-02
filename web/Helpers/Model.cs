namespace Atlas_Web.Helpers
{
    public static class ModelHelpers
    {
        public static string RelativeDate(DateTime? fixedDate)
        {
            if (fixedDate == null)
            {
                return "";
            }
            var timeAgo = System.DateTime.Now.Subtract(fixedDate ?? DateTime.Today);
            if (timeAgo.TotalMinutes < 1)
            {
                return String.Concat(timeAgo.Seconds.ToString(), " seconds ago");
            }
            if (timeAgo.TotalHours < 1)
            {
                return String.Concat(timeAgo.Minutes.ToString(), " minutes ago");
            }
            else if (timeAgo.TotalHours < 24)
            {
                return String.Concat(timeAgo.Hours.ToString(), " hours ago");
            }
            else
            {
                return (fixedDate ?? DateTime.Today).ToShortDateString();
            }
        }
    }
}
