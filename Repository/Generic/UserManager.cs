using StudyMATEUpload.Data;
using StudyMATEUpload.Models;
using StudyMATEUpload.Models.DTOs;
using StudyMATEUpload.Models.ViewModels;
using StudyMATEUpload.Repository.Extension;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StudyMATEUpload.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudyMATEUpload.Enums;
using Microsoft.AspNetCore.Identity;

namespace StudyMATEUpload.Repository.Generics
{
    public class UserManager : ModelManager<ApplicationUser>
    {
        private readonly IMapper _mapper;
        private readonly AuthRepository _auth;

        public UserManager()
        {

        }
        public UserManager(ApplicationDbContext context, IMapper mapper, AuthRepository auth) : base(context)
        {
            _mapper = mapper;
            _auth = auth;
        }

        public async ValueTask<(string, ApplicationUser, string)> GetOrCreateExternalGoogleLoginUser(string provider, 
            string key, string email, string firstName, string lastName, Role role, bool isVerified)
        {
            // Login already linked to a user
            var user = await Item().FirstOrDefaultAsync(u => u.Provider.ToLower() == provider.ToLower() && key == u.ProviderKey);
            if (user != null) return (_auth.GetToken(email, Enum.GetName(typeof(Role), user.Role)), user, null);

            user = await Item().FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            if (user == null)
            {
                // No user exists with this email address, we create a new one
                user = new ApplicationUser
                {
                    Email = email,
                    Provider = provider,
                    FirstName = firstName,
                    SurName = lastName,
                    ProviderKey = key,
                    Role = role,
                    IsVerified = isVerified,
                    VerifiedOn = DateTime.Now
                };

                var (s, u, e) = await Add(user);
                if (!s) return (null, null, e);
                user = u;
            }
            else
            {
                user.Provider = provider;
                user.ProviderKey = key;
                var (s, u, e) = await Update(user);
                if (!s) return (null, null, e);
                user = u;
            }

            return (_auth.GetToken(email, Enum.GetName(typeof(Role), user.Role)), user, null);
        }

        
        public async ValueTask<(string, ApplicationUser, string)> RegisterUser(RegisterViewModel model)
        {
            if (model.Password == model.ConfirmPassword)
            {
                bool hasAccount = await Item().AnyAsync(u => u.Email.ToLower() == model.Email.ToLower());
                if (hasAccount) { return (null, null, "user already exists"); }

                string passwordHash = Hash.GetHashedValue(model.Password);
                ApplicationUser user = model.Convert<RegisterViewModel, ApplicationUser>(_mapper);
                user.PasswordHash = passwordHash;
                try
                {
                    await Add(user);
                }
                catch (Exception ex)
                {
                    return (null, null, ex.Message);
                }
                return (await LoginUser(new LoginViewModel { Email = model.Email, Password = model.Password }));
            }
            return (null, null, "password must match");

        }

        public async ValueTask<(string, ApplicationUser, string)> LoginUser(LoginViewModel model)
        {
            var loginPassword = model.Password;
            var user = await Item().Where(x => x.Email.ToLower() == model.Email.ToLower())
                                                .FirstOrDefaultAsync();
            if (user == null) { return (null, null, "no such user in the database"); }

            string incomingHash = Hash.GetHashedValue(loginPassword);
            if (incomingHash != user.PasswordHash)
            {
                return (null, null, "password do not match");
            }
            return (_auth.GetToken(model.Email, Enum.GetName(typeof(Role), user.Role)), user, null);
        }

        public async ValueTask<(bool, ApplicationUser, string)> ChangePassword(PatchKnownPasswordDTO patch)
        {
            ApplicationUser user = await FindOne(u => u.Id == patch.Id && !u.Deleted);
            if (user != null)
            {
                string passwordHash = Hash.GetHashedValue(patch.OldPassword);
                if (passwordHash == user.PasswordHash)
                {
                    string newHashedPassword = Hash.GetHashedValue(patch.NewPassword);
                    user.PasswordHash = newHashedPassword;
                    return await Update(user);
                }
            }
            return (false, null, "user not found");
        }
        
        public async ValueTask<UserDTO> GetUserExtras(int id, string what)
        {
            bool found = await Item().AnyAsync(u => u.Id == id);
            if (found)
            {
                //if(what == "ulc")
                //{
                //    return await Item().Where(u => u.Id == id)
                //                            .Include(u => u.UserLearnCourses)
                //                                .ThenInclude(u => u.LearnCourse)
                //                            .Select(u => u.Convert<ApplicationUser, UserDTO>(_mapper))
                //                            .FirstOrDefaultAsync();
                //}
                if (what == "uc")
                {
                    return await Item().Where(u => u.Id == id)
                                            .Include(u => u.UserCourses.Where(u => !u.Deleted))
                                                .ThenInclude(u => u.Course)
                                            .Select(u => u.Convert<ApplicationUser, UserDTO>(_mapper))
                                            .FirstOrDefaultAsync();
                }
                if (what == "uf")
                {
                    return await Item().Where(u => u.Id == id)
                                            .Include(u => u.UserFeedbacks)
                                            .Select(u => u.Convert<ApplicationUser, UserDTO>(_mapper))
                                            .FirstOrDefaultAsync();
                }
                if (what == "us")
                {
                    return await Item().Where(u => u.Id == id)
                                            .Include(u => u.UserSubscriptions)
                                                .ThenInclude(u => u.Subscription)
                                            .Select(u => u.Convert<ApplicationUser, UserDTO>(_mapper))
                                            .FirstOrDefaultAsync();
                }
                if (what == "ua")
                {
                    return await Item().Where(u => u.Id == id)
                                            .Include(u => u.UserAwards)
                                                .ThenInclude(u => u.Award)
                                            .Select(u => u.Convert<ApplicationUser, UserDTO>(_mapper))
                                            .FirstOrDefaultAsync();
                }

            }
            return null;
        }



        public async ValueTask<(bool, ApplicationUser, string)> ForgotPassword(PatchUnknownPasswordDTO patch, string code)
        {
            ApplicationUser user = await FindOne(u => u.Email.ToLower() == patch.Email.ToLower() && !u.Deleted);
            if (user != null)
            {
                if (code == user.Code && user.CodeIssued <= user.CodeWillExpire)
                {

                }
                if (patch.NewPassword == patch.ConfirmNewPassword)
                {
                    string passwordHash = Hash.GetHashedValue(patch.NewPassword);
                    user.PasswordHash = passwordHash;
                    return await Update(user);
                }

            }
            return (false, null, "user not found"); ;
        }


    }
}
