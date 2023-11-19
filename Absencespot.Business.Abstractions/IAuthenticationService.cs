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
        Task Register(Dtos.Register register, CancellationToken cancellationToken = default);
        Task ConfirmEmail(Dtos.ConfirmEmail confirmEmail, CancellationToken cancellationToken = default);
        Task<Dtos.TokenResponse> Login(Dtos.LoginRequest login, CancellationToken cancellationToken = default);
        Task LoginWithGoogle();
        Task LoginWithMicrosoft();
        Task LoginWithSlack();
        Task<Dtos.TokenResponse> ExternalLogin(ExternalLoginInfo externalLogin, CancellationToken cancellationToken = default);
        Task RequestResetPassword(string email, CancellationToken cancellationToken = default);
        Task ResetPassword(Dtos.ResetPassword resetPassword, CancellationToken cancellationToken = default);
    }
}
