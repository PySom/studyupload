using AutoMapper;
using StudyMATEUpload.Data;
using StudyMATEUpload.Models;
using StudyMATEUpload.Models.ViewModels;
using StudyMATEUpload.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using StudyMATEUpload.Repository.Extension;

namespace StudyMATEUpload.Repository
{
    public class AccountRepository
    {
        private readonly AuthRepository _auth;
        private readonly ApplicationDbContext _ctx;
        private readonly IMapper _mapper;
        public AccountRepository(AuthRepository auth,
            ApplicationDbContext context,
            IMapper mapper)
        {
            _auth = auth;
            _ctx = context;
            _mapper = mapper;
        }

        public async ValueTask<(string, string)> LoginUser(LoginViewModel model)
        {
            var loginPassword = model.Password;
            var user = await _ctx.Users.Where(x => x.Email.Equals(model.Email, StringComparison.InvariantCultureIgnoreCase))
                                                .FirstOrDefaultAsync();
            if(user == null) { return (null, "no such user in the database"); }
            
            string incomingHash = Hash.GetHashedValue(loginPassword);
            if(incomingHash != user.PasswordHash)
            {
                return (null, "password do not match");
            }
            return (_auth.GetToken(model.Email, user.IsAdmin), null);
        }

        public async ValueTask<(string, string)> RegisterUser(RegisterViewModel model)
        {
            if(model.Password == model.ConfirmPassword)
            {
                bool hasAccount = await _ctx.Users.AnyAsync(u => u.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase));
                if (hasAccount) { return (null, "user already exists"); }

                string passwordHash = Hash.GetHashedValue(model.Password);
                ApplicationUser user = model.Convert<RegisterViewModel, ApplicationUser>(_mapper);
                user.PasswordHash = passwordHash;
                user.IsAdmin = CheckAdmin(model.Email);
                try
                {
                    _ctx.Users.Add(user);
                    await _ctx.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Message: {ex.Message}");
                    return (null, ex.Message);
                }
                return (await LoginUser(new LoginViewModel { Email = model.Email, Password = model.Password }));
            }
            return (null, "password must match");
            
        }

        private static bool CheckAdmin(string email)
        {
            var adminEmails = Startup.Configuration.GetSection("AdminEmails")
                                                                    .AsEnumerable()
                                                                    .Where(e => !string.IsNullOrEmpty(e.Value))
                                                                    .Select(e => e.Value);
            return adminEmails.Contains(email, StringComparer.OrdinalIgnoreCase);
        }


        public async ValueTask<bool> UpdateUser(ApplicationUser user)
        {
            try
            {
                _ctx.Users.Update(user);
                await _ctx.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return false;
        }

        public async ValueTask<ApplicationUser> GetUserById(string id) => await _ctx.Users.FindAsync(id);
        public async ValueTask<ApplicationUser> GetUserByEmail(string email)
        {
            return await _ctx.Users.Where(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)).FirstAsync();
        }
    }
}
