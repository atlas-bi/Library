using System.Threading.Tasks;
using Atlas_Web.Models;

namespace Atlas_Web.Services
{
    public interface IEmailService
    {
        Task SendAsync(string subject, string body, string sender, string reciever);
    }
}
