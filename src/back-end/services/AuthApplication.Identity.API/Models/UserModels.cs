using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthApplication.Identity.API.NovaPasta
{
    public class UserLogin
    {
        [Required(ErrorMessage = "This field {0} is required")]
        [EmailAddress(ErrorMessage = "This field {0} is invalid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field {0} is required")]
        [StringLength(100, ErrorMessage = "This field {0} should be between {2} and {1} characters", MinimumLength = 0)]
        public string Password { get; set; }
    }

    public class UserRegister
    {

        [Required(ErrorMessage = "This field {0} is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "This field {0} is required")]
        public string Document { get; set; }


        [Required(ErrorMessage = "This field {0} is required")]
        [EmailAddress(ErrorMessage = "This field {0} is invalid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field {0} is required")]
        [StringLength(100, ErrorMessage = "This field {0} should be between {2} and {1} characters", MinimumLength = 0)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string PasswordConfirm { get; set; }
    }


    #region Views

    public class UserLoginView
    {
        public string AccessToken { get; set; }
        public double ExpiresIn { get; set; }
        public UserTokenView UserToken { get; set; }
    }

    public class UserTokenView
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public IEnumerable<UsuarioClaimView> Claims { get; set; }
    }

    public class UsuarioClaimView
    {
        public string Value { get; set; }
        public string Type { get; set; }
    }
    #endregion
}
