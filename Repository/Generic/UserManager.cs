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
        private readonly IModelManager<UserCourse> _usercourse;
        private readonly IModelManager<Course> _course;

        public UserManager()
        {

        }
        public UserManager(ApplicationDbContext context, IMapper mapper, 
            AuthRepository auth, IModelManager<UserCourse> usercourse, 
            IModelManager<Course> course) : base(context)
        {
            _mapper = mapper;
            _auth = auth;
            _usercourse = usercourse;
            _course = course;
        }

        public async ValueTask<(string, ApplicationUser, string)> GetOrCreateExternalLoginUser(string provider, 
            string key, string email, string firstName, string lastName, Role role, bool isVerified, string picture)
        {
            // Login already linked to a user
            var user = await Item().Include(u => u.UserSubscriptions)
                .ThenInclude(s => s.Subscription)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.Trim().ToLower());

            //there is a user
            if (user != null)
            {
                //user has not used this provider key
                if ( user.Provider?.ToLower() != provider?.ToLower() && key != user.ProviderKey)
                {
                    //user exists but hasn't used external.
                    user.Provider = provider;
                    user.ProviderKey = key;
                    var (s, u, e) = await Update(user);
                    if (!s) return (null, null, e);
                    user = u;
                }                
                UserSubscription userSub = user.UserSubscriptions.Where(u => u.StartedOn + new TimeSpan(u.Subscription.Duration, 0, 0, 0) >= DateTime.Now).FirstOrDefault();
                TimeSpan durationLeft = new TimeSpan(0, 0, 0);
                bool hasDuration = false;
                if(userSub is object)
                {
                    durationLeft = (userSub.StartedOn + new TimeSpan(userSub.Subscription.Duration, 0, 0, 0)) - DateTime.Now;
                    hasDuration = durationLeft > new TimeSpan(0,0,0,0);
                    user.IsSubscribed = hasDuration;
                }
                return (_auth.GetToken(email, Enum.GetName(typeof(Role), user.Role), userSub is object, durationLeft, hasDuration), user, null);
            }

            // No user exists with this email address and provider, we create a new one
            else
            {
                user = new ApplicationUser
                {
                    Email = email,
                    Provider = provider,
                    FirstName = firstName,
                    SurName = lastName,
                    ProviderKey = key,
                    Image = picture,
                    Role = role,
                    IsVerified = isVerified,
                    VerifiedOn = DateTime.Now
                };

                var (s, u, e) = await Add(user);
                if (!s) return (null, null, e);
                user = u;

                //await AddUserCoursesAsync(user.Id);
            }
            return (_auth.GetToken(email, Enum.GetName(typeof(Role), user.Role), false, new TimeSpan(0,0,0), false), user, null);
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
                    var (success, addedUser, message) = await Add(user);
                    //if(success)
                    //{
                    //    await AddUserCoursesAsync(addedUser.Id);
                    //}
                }
                catch (Exception ex)
                {
                    return (null, null, ex.Message);
                }
                return (await LoginUser(new LoginViewModel { Email = model.Email, Password = model.Password }));
            }
            return (null, null, "password must match");

        }

        private async Task AddUserCoursesAsync(int uid)
        {
            string cca = "CREATIVE AND CULTURAL ART";
            string jmaths = "Mathematics";
            string fmaths = "Further Mathematics";
            string chemistry = "Chemistry";
            var getDefaultUserCourses = await _course.Item()
                .Where(c => c.Name.ToLower() == cca.ToLower() ||
                        (c.Name.ToLower() == jmaths.ToLower() && c.StudyLevel == StudyLevel.Junior) ||
                        c.Name.ToLower() == fmaths.ToLower() ||
                        c.Name.ToLower() == chemistry.ToLower())
                .Select(c => new { c.Id })
                .ToListAsync();
            var userCoursesToAdd = getDefaultUserCourses
                .Select(gd => new UserCourse { CourseId = gd.Id, UserId = uid }).ToList();
            var _ = await _usercourse.Add(userCoursesToAdd);
        }

        public async ValueTask<(string, ApplicationUser, string)> LoginUser(LoginViewModel model)
        {
            var loginPassword = model.Password;
            var user = await Item().Include(u => u.UserSubscriptions).ThenInclude(s => s.Subscription)
                .Where(x => x.Email.ToLower() == model.Email.ToLower())
                                                .FirstOrDefaultAsync();
            if (user == null) { return (null, null, "no such user in the database"); }

            string incomingHash = Hash.GetHashedValue(loginPassword);
            if (incomingHash != user.PasswordHash)
            {
                return (null, null, "hmmm! Something is fishy. The username or password is incorrect");
            }
            UserSubscription userSub = user.UserSubscriptions.Where(u => u.StartedOn + new TimeSpan(u.Subscription.Duration, 0, 0, 0) >= DateTime.Now).FirstOrDefault();
            TimeSpan durationLeft = new TimeSpan(0, 0, 0);
            bool hasDuration = false;
            if (userSub is object)
            {
                durationLeft = (userSub.StartedOn + new TimeSpan(userSub.Subscription.Duration, 0, 0, 0)) - DateTime.Now;
                hasDuration = durationLeft > new TimeSpan(0, 0, 0, 0);
                user.IsSubscribed = hasDuration;
            }
            return (_auth.GetToken(model.Email, Enum.GetName(typeof(Role), user.Role), userSub is object, durationLeft, hasDuration), user, null);
        }


        public async ValueTask<UserViewModel> FindByEmailAsync(string email)
        {
            var user = await Item().Include(u => u.UserSubscriptions).ThenInclude(s => s.Subscription)
                .Where(x => x.Email.ToLower() == email.ToLower())
                 .FirstOrDefaultAsync();
            if (user is null) { return null; }
            UserSubscription userSub = user.UserSubscriptions.Where(u => u.StartedOn + new TimeSpan(u.Subscription.Duration, 0, 0, 0) >= DateTime.Now).FirstOrDefault();
            TimeSpan durationLeft = new TimeSpan(0, 0, 0);
            bool hasDuration = false;
            if (userSub is object)
            {
                durationLeft = (userSub.StartedOn + new TimeSpan(userSub.Subscription.Duration, 0, 0, 0)) - DateTime.Now;
                hasDuration = durationLeft > new TimeSpan(0, 0, 0, 0);
                user.IsSubscribed = hasDuration;
            }
            return _mapper.Map<UserViewModel>(user);
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

                if (what == "refer")
                {
                    return await Item().Where(u => u.Id == id)
                                            .Include(u => u.Referrals)
                                                .ThenInclude(u => u.Referrals)
                                            .Select(u => u.Convert<ApplicationUser, UserDTO>(_mapper))
                                            .FirstOrDefaultAsync();
                }

            }
            return null;
        }



        public async ValueTask<(bool, ApplicationUser, string)> ForgotPassword(PatchUnknownPasswordDTO patch, string code)
        {
            var user = await Item().Where(u => u.Email.Equals(patch.Email)).FirstOrDefaultAsync();
            if (user != null)
            {
                if (code == user.Code && user.CodeIssued <= user.CodeWillExpire)
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
