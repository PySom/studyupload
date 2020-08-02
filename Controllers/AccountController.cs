using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using StudyMATEUpload.Models;
using StudyMATEUpload.Models.ViewModels;
using StudyMATEUpload.Repository;
using StudyMATEUpload.Repository.Extension;
using StudyMATEUpload.Repository.Generics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using StudyMATEUpload.Models.DTOs;
using StudyMATEUpload.Services;
using Microsoft.AspNetCore.JsonPatch;
using static Google.Apis.Auth.GoogleJsonWebSignature;
using Microsoft.AspNetCore.Identity;
//using StudyMATEUpload.Services;

namespace StudyMATEUpload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager _acc;
        private readonly IMapper _mapper;
        private readonly IEmailSender _email;
        
        public AccountController(UserManager account, 
            IEmailSender email,
            IMapper mapper)
        {
            _acc = account;
            _mapper = mapper;
           _email = email;
           
        }

        [HttpPost("auth/google")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GoogleLogin(GoogleLoginRequest request)
        {
            Payload payload = null;
            try
            {
                payload = await ValidateAsync(request.IdToken, new ValidationSettings
                {
                    Audience = new[] { "598105055249-eeim29vgq4e3lese6hvn2bt8n1epkt7q.apps.googleusercontent.com" }
                });
                // It is important to add your ClientId as an audience in order to make sure
                // that the token is for your application!
            }
            catch(Exception ex)
            {
                Console.WriteLine($"message: {ex.Message}");
                // Invalid token
            }

            var (token, whoLoggedIn, error) = await _acc.GetOrCreateExternalGoogleLoginUser("google", payload.Subject, payload.Email, 
                payload.GivenName, payload.FamilyName, request.Role, payload.EmailVerified, payload.Picture);
            if (token != null)
            {
                UserViewModel dto = whoLoggedIn.Convert<ApplicationUser, UserViewModel>(_mapper);
                dto.Token = token;
                return Ok(dto);
            }
            return BadRequest(new { Message = error });
        }

        [HttpPost("login")]
        public async ValueTask<IActionResult> Post([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var (token, whoLoggedIn, error) = await _acc.LoginUser(model);
                if(token != null)
                {
                    UserViewModel dto = whoLoggedIn.Convert<ApplicationUser, UserViewModel>(_mapper);
                    dto.Token = token;
                    return Ok(dto);
                }
                return BadRequest(new { Message = error });
            }
            return BadRequest(new { Errors = ModelState.Values.SelectMany(e => e.Errors).ToList() });
        }



        [HttpPost("register")]
        public async ValueTask<IActionResult> Post([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var (token, whoRegistered, error) = await _acc.RegisterUser(model);
                if (token != null)
                {
                    UserViewModel dto = whoRegistered.Convert<ApplicationUser, UserViewModel>(_mapper);
                    dto.Token = token;
                    return Ok(dto);
                }
                return BadRequest(new { Message = error });
            }
            return BadRequest(new { Errors = ModelState.Values.SelectMany(e => e.Errors).ToList() });
        }


        private string GetUserEmailFromToken() => HttpContext.User.Claims
                                                        .Where(u => u.Type.Contains("emailaddress"))
                                                        .Select(u => u.Value).FirstOrDefault();

        [Authorize]
        [HttpGet("getcurrentuser")]
        public async ValueTask<IActionResult> GetCurrentUser()
        {
            var userEmail = GetUserEmailFromToken();
            if (userEmail == null) return NotFound();
            UserViewModel user = await _acc.FindByEmailAsync(userEmail);
            return Ok(user);
        }

        [Authorize]
        [HttpGet("getuserlearncourses")]
        public async ValueTask<IActionResult> GetUserLearnCourses(int id)
        {
            var user = await _acc.GetUserExtras(id, "ulc");
            if (user == null) return NotFound();
            return Ok(user);
        }

        [Authorize]
        [HttpGet("getuserawards")]
        public async ValueTask<IActionResult> GetUserAwards(int id)
        {
            var user = await _acc.GetUserExtras(id, "ua");
            if (user == null) return NotFound();
            return Ok(user);
        }

        [Authorize]
        [HttpGet("getusercourses")]
        public async ValueTask<IActionResult> GetUserCourses(int id)
        {
            var user = await _acc.GetUserExtras(id, "uc");
            if (user == null) return NotFound();
            return Ok(user);
        }

        [Authorize]
        [HttpGet("getuserfeedbacks")]
        public async ValueTask<IActionResult> GetUserFeedbacks(int id)
        {
            var user = await _acc.GetUserExtras(id, "uf");
            if (user == null) return NotFound();
            return Ok(user);
        }

        [Authorize]
        [HttpGet("getusersubscriptions")]
        public async ValueTask<IActionResult> GetUserSubscriptions(int id)
        {
            var user = await _acc.GetUserExtras(id, "us");
            if (user == null) return NotFound();
            return Ok(user);
        }


        [Authorize]
        [HttpPut("user")]
        public async ValueTask<IActionResult> Put([FromBody] UserDTO model)
        {
            if (ModelState.IsValid)
            {
                bool isUser = await _acc.Item().AnyAsync(u => model.Id == u.Id);
                if (isUser)
                {
                    ApplicationUser userMappedFromModel = model.Convert<UserDTO, ApplicationUser>(_mapper);
                    (bool succeeded, ApplicationUser updatedUser, string error) = await _acc.Update(userMappedFromModel);
                    if (succeeded) return Ok(updatedUser.Convert<ApplicationUser, UserDTO>(_mapper));
                    return BadRequest(new { Message = error });
                }
            }
            return BadRequest(new { Errors = ModelState.Values.SelectMany(e => e.Errors).ToList() });
        }


        [Authorize]
        [HttpPatch("user/{id:int}")]
        public async ValueTask<IActionResult> Put([FromBody]JsonPatchDocument<ApplicationUser> patchDoc, int id)
        {
            var model = await _acc.Item().FindAsync(id);
            if (model != null)
            {
                patchDoc.ApplyTo(model, ModelState);
                if (ModelState.IsValid)
                {
                    (bool succeeded, ApplicationUser updatedUser, string error) = await _acc.Update(model);
                    if (succeeded) return Ok(_mapper.Map<ApplicationUser, UserDTO>(updatedUser));
                    return BadRequest(new { Message = error });
                }
                return BadRequest(new { Errors = ModelState.Values.SelectMany(e => e.Errors).ToList() });
            }
            return BadRequest(new { Message = "No such item" });
        }

        [Authorize]
        [HttpPut("changepassword")]
        public async ValueTask<IActionResult> Put([FromBody] PatchKnownPasswordDTO model)
        {
            if (ModelState.IsValid)
            {
                var (succeeded, user, error) = await _acc.ChangePassword(model);
                if (succeeded) return Ok(new { succeeded });
                return BadRequest(new { Message = error });
            }
            return BadRequest(new { Errors = ModelState.Values.SelectMany(e => e.Errors).ToList() });

        }

        [HttpPost("generatecode")]
        public async ValueTask<IActionResult> Generate(string email, bool validate = false)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _acc.Item()
                                                    .Where(u => u.Email.ToLower() == email.ToLower())
                                                    .FirstOrDefaultAsync();
                if (user != null)
                {
                    string code = $"{Guid.NewGuid().ToString().Replace("-", "")}{Guid.NewGuid().ToString().Replace("-", "")}";
                    user.Code = code;
                    user.CodeIssued = DateTime.Now;
                    user.CodeWillExpire = DateTime.Now.AddDays(2);
                    (bool succeeded, ApplicationUser _, string error) = await _acc.Update(user);
                    if (succeeded)
                    {
                        string subject = validate ? "Validate Email" : "Change Password";
                        string message = validate ? BuildEmailVerificationyText(user.FirstName, code) : BuildPasswordRecoveryText(user.FirstName, code);
                        await _email.SendEmailAsync(email, subject, message.ToString());
                        return NoContent();
                    }
                }

                return NotFound();
            }
            return BadRequest(new { Errors = ModelState.Values.SelectMany(e => e.Errors).ToList() });

        }

        

        [HttpPut("forgotpassword")]
        public async ValueTask<IActionResult> Put(string code, [FromBody] PatchUnknownPasswordDTO model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(code)) return BadRequest(new { Message = "Are you normal? Code kwanu?" });
                var (succeeded, _, error) = await _acc.ForgotPassword(model, code);
                if (succeeded) return Ok(new { succeeded });
                return BadRequest(new { Message = error });
            }
            return BadRequest(new { Errors = ModelState.Values.SelectMany(e => e.Errors).ToList() });

        }

        [HttpPut("verifyemail")]
        public async ValueTask<IActionResult> Put(string code)
        {
            if (ModelState.IsValid)
            {
                if (code == null) return BadRequest(new { Message = "Are you normal? Code kwanu?" });
                var user = await _acc.Item().Where(u => u.Code == code).FirstOrDefaultAsync();
                if (user != null)
                {
                    if(user.CodeIssued <= user.CodeWillExpire)
                    {
                        user.VerifiedOn = DateTime.Now;
                        user.IsVerified = true;
                        var (succeeded, _, error) = await _acc.Update(user);
                        if (succeeded) return Ok(new { succeeded });
                        return BadRequest(new { Message = error });
                    }
                    return BadRequest(new { Message = "code is invalid or has expired" });
                }
                return BadRequest(new { Message = "user not found" });
            }
            return BadRequest(new { Errors = ModelState.Values.SelectMany(e => e.Errors).ToList() });

        }

        [HttpGet]
        public async ValueTask<IActionResult> Get()
        {
            ICollection<UserViewModel> users = await _acc.Item()
                                                        .Select(u => u.Convert<ApplicationUser, UserViewModel>(_mapper))
                                                        .ToListAsync();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async ValueTask<IActionResult> Get(int id)
        {
            UserViewModel user = await _acc.Item()
                                          .Where(u => id == u.Id)
                                          .Select(u => u.Convert<ApplicationUser, UserViewModel>(_mapper))
                                          .FirstOrDefaultAsync();
            return Ok(user);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async ValueTask<IActionResult> Delete(int id)
        {
            var model = new ApplicationUser { Id = id };
            (bool succeeded, string error) = await _acc.Delete(model);
            if (succeeded) return NoContent();
            return NotFound(new { Message = error });
        }

        [Authorize]
        [HttpGet("leaderboard/{id:int}")]
        public async ValueTask<IActionResult> GetLeaderByScore(int id)
        {
            var found = await _acc.Item().AnyAsync(u => u.Id == id);
            if (!found) return NotFound();
            var users = await _acc.Item()
                .Include(u => u.UserCourses)
                    .ThenInclude(u => u.UserTests)
                .ToListAsync();
            var leaders = new List<LeaderBoard>();
            if (users.Count > 0)
            {
                foreach (var user in users)
                {
                    foreach (var course in user.UserCourses)
                    {
                        var userScore = course.UserTests.Aggregate(0, (agr, next) => next.Score + agr);
                        var boardInLeader = leaders.Where(l => l.Id == user.Id).FirstOrDefault();
                        if(boardInLeader != null)
                        {
                            leaders.Remove(boardInLeader);
                            boardInLeader.Score += userScore;
                            leaders.Add(boardInLeader);
                        }
                        else
                        {
                            leaders.Add(new LeaderBoard 
                            { Id = user.Id, 
                                Name = $"{user.FirstName} {user.SurName}", 
                                UserImg=user.Image, 
                                Score = userScore 
                            });
                        }
                    }
                }
            }

            var positions = leaders.Where(u => u.Score != 0).OrderByDescending(u => u.Score).ToList();
            var leadersWithPosition = new List<LeaderBoard>();
            int pos = 1;
            foreach (var position in positions)
            {
                position.Position = pos;
                leadersWithPosition.Add(position);
                pos++;
            }

            var model = new List<LeaderBoard>();

            var currentUser = leadersWithPosition.Where(u => u.Id == id).FirstOrDefault();
            if(currentUser != null && currentUser.Position <= 5)
            {
                model = leadersWithPosition.Take(6).ToList();
            }
            else if(currentUser != null)
            {
                model = leadersWithPosition.Take(5).ToList();
                model.Add(currentUser);
            }
            else
            {
                model = leadersWithPosition.Take(5).ToList();
                var cu = leaders.Where(u => u.Id == id).FirstOrDefault();
                cu.Position = model.Count + 1;
                model.Add(cu);
            }
            return Ok(model);
        }

        private class LeaderBoard
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string UserImg { get; set; }
            public int Score { get; set; }
            public int Position { get; set; }
        }

        private string BuildPasswordRecoveryText(string firstName, string code)
        {
            StringBuilder message = new StringBuilder(100);
            message.Append($"Dear {firstName},<br/><br/>");
            message.Append($"You requested a change in password.<br/>Kindly use this link to reset your password.<br/>The code will expire in less than two (2) days.<br/><br/>");
            message.Append($"<a href='https://studymate.ng/forgotpassword?code={code}'>Reset Password</a><br/><br/>");
            message.Append("Thank you.<br/><br/>Sincerely,<br/>Admin.");
            return message.ToString();
        }

        private string BuildEmailVerificationyText(string firstName, string code)
        {
            StringBuilder message = new StringBuilder(100);
            message.Append($"Dear {firstName},<br/><br/>");
            message.Append($"Please verify your email.<br/>The code will expire in less than two (2) days.<br/>");
            message.Append($"<a href='https://studymate.ng/verifyemail?code={code}'>Verify Email</a><br/><br/>");
            message.Append("Thank you.<br/><br/>Sincerely,<br/>Admin.");
            return message.ToString();
        }
    }
}