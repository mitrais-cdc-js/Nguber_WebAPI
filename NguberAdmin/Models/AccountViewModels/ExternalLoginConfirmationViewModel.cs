using System.ComponentModel.DataAnnotations;

namespace NguberAdmin.Models.AccountViewModels {
  public class ExternalLoginConfirmationViewModel {
    [Required]
    [EmailAddress]
    public string Email { get; set; }
  }
}
