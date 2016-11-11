using System.Threading.Tasks;

namespace NguberAdmin.Services {
  public interface ISmsSender {
    Task SendSmsAsync (string number, string message);
  }
}
