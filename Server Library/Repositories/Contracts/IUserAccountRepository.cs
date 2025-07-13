using Base_Library.DTOs;
using Base_Library.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_Library.Repositories.Contracts
{
    public interface IUserAccountRepository
    {
        Task<GeneralResponse> CreateAsync(Register user);
        Task<LoginReponse> SignInAsync(Login user);
        Task<LoginReponse> RefreshTokenAsync(RefreshToken refreshToken);
    }
}
