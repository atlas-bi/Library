using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Atlas_Web.Services
{
    public interface IRazorPartialToStringRenderer
    {
        Task<string> RenderPartialToStringAsync(string partialName, ViewDataDictionary ViewData);
    }
}
