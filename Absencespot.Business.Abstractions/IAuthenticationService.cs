using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Business.Abstractions
{
    public interface IAuthenticationService
    {
        Task Register(Dtos.Register register);
        Task ConfirmEmail(Dtos.ConfirmEmail confirmEmail);
        Task<Dtos.TokenResponse> Login(Dtos.LoginRequest login);
        Task LoginWithGoogle();
        Task LoginWithMicrosoft();
        Task LoginWithSlack();
        Task<Dtos.TokenResponse> ExternalLogin(ExternalLoginInfo externalLogin);
        Task RequestResetPassword(string email);
        Task ResetPassword(Dtos.ResetPassword resetPassword);
    }
}
