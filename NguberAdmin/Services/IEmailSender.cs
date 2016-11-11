using System.Threading.Tasks;

namespace NguberAdmin.Services {
  public interface IEmailSender {
    Task SendEmailAsync (string email, string subject, string message);
  }
}
