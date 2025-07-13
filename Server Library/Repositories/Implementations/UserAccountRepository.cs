using Base_Library.DTOs;
using Base_Library.Entities;
using Base_Library.Responses;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Server_Library.Data;
using Server_Library.Helpers;
using Server_Library.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Constants = Server_Library.Helpers.Constants;

namespace Server_Library.Repositories.Implementations
{
    public class UserAccountRepository(IOptions<JwtSection> config, AppDpContext appDpContext) : IUserAccountRepository
    {
        public async Task<GeneralResponse> CreateAsync(Register user)
        {

            if (user is null)
            {
                return new GeneralResponse(false, "User cannot be empty.");
            }
            if (string.IsNullOrEmpty(user.FullName) || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
            {
                return new GeneralResponse(false, "All fields are required.");
            }
            var emailExists = await FindUserByEmail(user.Email);
            if (emailExists != null)
            {
                return new GeneralResponse(false, "User already exists.");
            }

            var appicationUser =await AddToDatabase( new ApplicationUser
            {
                FullName = user.FullName,
                Email = user.Email.ToLower(),
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password)
            });
            // the first person who registers will be the admin
            var checkAdminRole = await appDpContext.SystemRoles.FirstOrDefaultAsync(r => r.Name.Equals(Constants.Admin));
            if (checkAdminRole is null)
            {
               var createAdminRole = await AddToDatabase(new SystemRoles()
                {
                    Name = Constants.Admin,
                });
                await AddToDatabase(new UserRole()
                {
                    UserId = appicationUser.Id,
                    RoleId = createAdminRole.Id
                });
                return new GeneralResponse(true, "User created successfully.");

            }
            var checkUserRole = await appDpContext.SystemRoles.FirstOrDefaultAsync(r => r.Name.Equals(Constants.User));
            if (checkUserRole is null)
            {
                var createUserRole = await AddToDatabase(new SystemRoles()
                {
                    Name = Constants.User,
                });
                await AddToDatabase(new UserRole()
                {
                    UserId = appicationUser.Id,
                    RoleId = createUserRole.Id
                });
            }
            else
            {
                 await AddToDatabase(new UserRole()
                {
                    UserId = appicationUser.Id,
                    RoleId = checkUserRole.Id
                });
            }
            return new GeneralResponse(true, "User created successfully.");

        }

        public async Task<LoginReponse> SignInAsync(Login user)
        {
            if (user is null)
            {
                return new LoginReponse(false, "User cannot be empty.");
            }
            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
            {
                return new LoginReponse(false, "All fields are required.");
            }
            var userExists = await FindUserByEmail(user.Email);
            if (userExists is null)
            {
                return new LoginReponse(false, "User does not exist.");
            }
            if (!BCrypt.Net.BCrypt.Verify(user.Password, userExists.Password!))
            {
                return new LoginReponse(false, "Invalid Email or Password.");
            }
            var userRoles = await findUserRole(userExists.Id);
            if (userRoles is null)
            {
                return new LoginReponse(false, "User does not have a role assigned.");
            }
            var role = await findRoleName(userRoles.RoleId);
            if (role is null)
            {
                return new LoginReponse(false, "User role does not exist.");
            }
            var token = GenerateJwtToken(userExists, role.Name);
            var refreshToken = GenerateRefreshToken();
            //save the refresh token to the database
            var findUser = await appDpContext.RefreshTokenInfos.FirstOrDefaultAsync(u => u.UserId == userExists.Id);
            if (findUser is not null) { 
                    findUser.Token = refreshToken;
                await appDpContext.SaveChangesAsync();
            }
            else
            {
                await AddToDatabase(new RefreshTokenInfo()
                {
                    Token = refreshToken,
                    UserId = userExists.Id
                });
            }
            return new LoginReponse(true, "Login successful.", token, refreshToken);


        }

        private string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));


        private string GenerateJwtToken(ApplicationUser userExists, string name)
        {
            
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Value.Key) );
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userExists.Id.ToString()),
                new Claim(ClaimTypes.Name, userExists.FullName!),
                new Claim(ClaimTypes.Email, userExists.Email!),
                new Claim(ClaimTypes.Role, name)
            };
            var token = new JwtSecurityToken(
                issuer: config.Value.Issuer,
                audience: config.Value.Audience,
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<UserRole> findUserRole(int id)
        {
            var userRole = await appDpContext.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == id);
           
            return userRole;

        }
        private async Task<SystemRoles>findRoleName(int id)
        {
            
            var role = await appDpContext.SystemRoles.FirstOrDefaultAsync(r => r.Id == id);
            if (role is null)
            {
                throw new Exception("Role not found.");
            }
            
            return role;
        }


        private async Task<T> AddToDatabase<T>(T model)
        {
            
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model), "Model cannot be null.");
            }
           var result =  await appDpContext.AddAsync(model!);

            await appDpContext.SaveChangesAsync();
            return (T)result.Entity;
        }

       
        private async Task<ApplicationUser> FindUserByEmail(string email)
        {

            return await appDpContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower().Equals(email.ToLower()));

        }

        public async Task<LoginReponse> RefreshTokenAsync(RefreshToken refreshToken)
        {
            if (refreshToken is null || string.IsNullOrEmpty(refreshToken.Token))
            {
                return new LoginReponse(false, "Refresh token cannot be empty.");
            }
            var findToken = appDpContext.RefreshTokenInfos.FirstOrDefault(rt => rt.Token!.Equals(refreshToken.Token));
            if (findToken is null)
            {
                return new LoginReponse(false, "Refresh token does not exist.");
            }

            //get user details
            var user = appDpContext.Users.FirstOrDefault(u => u.Id == findToken.UserId);
            if (user is null)
            {
                return new LoginReponse(false, "User does not exist.");
            }
            //get user role
            var userRole = await findUserRole(user.Id);
            if (userRole is null)
            {
                return new LoginReponse(false, "User does not have a role assigned.");
            }
            //get role name
            var role = await findRoleName(userRole.RoleId);
            if (role is null)
            {
                return new LoginReponse(false, "User role does not exist.");
            }
            //generate new token and refresh token
            var token = GenerateJwtToken(user, role.Name);
            var newRefreshToken = GenerateRefreshToken();
            //update the refresh token in the database

            var updateRefreshToken = await appDpContext.RefreshTokenInfos.FirstOrDefaultAsync(u => u.UserId == user.Id);
            if (updateRefreshToken is null)
            {
                return new LoginReponse(false, "Refresh token does not exist for this user.");
            }
            updateRefreshToken.Token = newRefreshToken;
            await appDpContext.SaveChangesAsync();
            return new LoginReponse(true, "Token refreshed successfully.", token, newRefreshToken);


        }
    }
}
