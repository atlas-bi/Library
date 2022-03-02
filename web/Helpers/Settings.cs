using Newtonsoft.Json.Linq;
using System.IO;

namespace Atlas_Web.Helpers
{
    public class Settings
    {
        public static string Property(string Property)
        {
            using var reader = new StreamReader("appsettings.json");
            var AppSettings = JObject.Parse(reader.ReadToEnd())["AppSettings"];
            return AppSettings.Value<string>(Property) ?? "";
        }
    }
}
