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
        Task<Dtos.LoginResponse> Login(Dtos.LoginRequest login);
        Task LoginWithGoogle();
        Task LoginWithMicrosoft();
        Task LoginWithSlack();
        Task<Dtos.TokenResponse> ExternalLogin(Dtos.ExternalLogin externalLogin);
        Task RequestResetPassword(string email);
        Task ResetPassword(Dtos.ResetPassword resetPassword);
    }
}
