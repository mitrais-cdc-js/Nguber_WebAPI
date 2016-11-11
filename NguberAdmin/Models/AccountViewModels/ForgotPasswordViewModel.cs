using System.ComponentModel.DataAnnotations;

namespace NguberAdmin.Models.AccountViewModels {
  public class ForgotPasswordViewModel {
    [Required]
    [EmailAddress]
    public string Email { get; set; }
  }
}
