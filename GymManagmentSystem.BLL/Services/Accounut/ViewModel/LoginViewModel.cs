using System.ComponentModel.DataAnnotations;

namespace GymManagmentSystem.BLL.Services.Accounut.ViewModel;
public class LoginViewModel
{
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public bool RememberMe { get; set; }
}
