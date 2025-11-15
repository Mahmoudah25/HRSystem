using Google.Apis.Auth;
using HRManagmentSystem.DTOs.Account_User;
using HRManagmentSystem.Models;
using HRManagmentSystem.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace HRManagmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AccountController : ControllerBase
    {
        private readonly HRContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IConfiguration config;
        private readonly IAppEmailSender emailSender;
        public AccountController(UserManager<ApplicationUser> userManager, IConfiguration config , IAppEmailSender emailSender, RoleManager<ApplicationRole> roleManager, HRContext context)
        {
            this.userManager = userManager;
            this.config = config;
            this.roleManager = roleManager;
            this.context = context;
            this.emailSender = emailSender;

        }

        //Register
        //[Authorize(Roles ="Admin")]
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task <IActionResult> Register([FromBody] RegisterDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
            };
            var result = await userManager.CreateAsync(user,model.Password);
            if (!result.Succeeded) 
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(error.Code, error.Description);
                return BadRequest(ModelState);
            }

            //Generate Token
            var Token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            //Link
            var confiramationLink = Url.Action(nameof(ConfirmEmail), "Account",
                new { userId = user.Id ,Token = WebUtility.UrlEncode(Token)},
                Request.Scheme);

            // Send Email
            await emailSender.SendEmailAsync(user.Email, "Confirm your email",
                $"Click <a href=\"{confiramationLink}\">here</a> to confirm your account.");

            if (!await roleManager.RoleExistsAsync("Employee"))
            {
                //await roleManager.CreateAsync(new IdentityRole<int>("Employee"));
                await roleManager.CreateAsync(new ApplicationRole { Name = "Employee", Description = "Default employee role" });
            }

            await userManager.AddToRoleAsync(user, "Employee");
            //return Ok(new
            //{
            //    message= "Employee Created Successfully"
            //});

            return Ok("User registered successfully! Please check your email to confirm your account.");


        }




        //LogIn
        [AllowAnonymous]
        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn(LogInDTO logInDTO)
        {
            if (ModelState.IsValid)
            {
                //Check
                ApplicationUser UserFromDB = await userManager.FindByEmailAsync(logInDTO.Email);
                if (UserFromDB != null)
                {
                    bool found = await userManager.CheckPasswordAsync(UserFromDB, logInDTO.Password);
                    if (found == true)
                    {
                        //Generate Token

                        List<Claim> UserClaims = new List<Claim>();

                        //Token Generated id Change(JWt Predefined Claims )

                        UserClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                        UserClaims.Add(new Claim(ClaimTypes.NameIdentifier, UserFromDB.Id.ToString()));
                        UserClaims.Add(new Claim(ClaimTypes.Name, UserFromDB.UserName));
                        var UserRoles = await userManager.GetRolesAsync(UserFromDB);
                        foreach (var RoleName in UserRoles)
                        {
                            UserClaims.Add(new Claim(ClaimTypes.Role, RoleName));
                        }


                        var SignInKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes
                            (config["JWT:SecurityKey"]));

                        SigningCredentials signingCredentials =
                            new SigningCredentials(SignInKey, SecurityAlgorithms.HmacSha256);


                        //Desigin Token
                        JwtSecurityToken mytoken = new JwtSecurityToken(
                            audience: config["JWT:AudienceIp"],
                            issuer: config["JWT:IssuerIP"],
                            expires: DateTime.Now.AddHours(24),
                            claims: UserClaims,
                            signingCredentials: signingCredentials

                            );

                        //genreate token Response
                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(mytoken),
                            expiration = DateTime.Now.AddHours(24)

                        });

                    }
                    ModelState.AddModelError("UserName", "UserManger Or Password InValid");
                }
            }
            return BadRequest(ModelState);
        }

        //Login with Google
        [AllowAnonymous]
        [HttpPost("login-Google")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] GoogleLoginDTO model)
        {
            if (string.IsNullOrEmpty(model.IdToken))
                return BadRequest("Token is required");

            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>
                {
                    "440050551197-ps19ds4mnk1ho0opp01r4bgjr1nmuupm.apps.googleusercontent.com" // Client ID 
                }
            };

            try
            {
                // ✅ Validate Token
                var payload = await GoogleJsonWebSignature.ValidateAsync(model.IdToken, settings);

                // ✅ Check if user exists
                var user = await userManager.FindByEmailAsync(payload.Email);

                if (user == null)
                {
                    // ✅ Register the user automatically if not exists
                    user = new ApplicationUser
                    {
                        UserName = payload.Email,
                        Email = payload.Email,
                        FullName = payload.Name
                    };

                    var result = await userManager.CreateAsync(user);
                    if (!result.Succeeded)
                        return BadRequest("Error creating user with Google");
                }

                var key = Encoding.UTF8.GetBytes(config["JWT:SecurityKey"]);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                   new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                   new Claim(ClaimTypes.Email, user.Email),
                   new Claim(ClaimTypes.Name, user.FullName)
                 }),
                    Expires = DateTime.UtcNow.AddHours(3),
                    Issuer = config["JWT:IssuerIP"],
                    Audience = config["JWT:AudienceIp"],
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return Ok(new
                {
                    Message = "Google Login Successful",
                    Token = tokenHandler.WriteToken(token),
                    Email = user.Email,
                    FullName = user.FullName
                });
            }
            catch
            {
                return BadRequest("Invalid Google token");
            }
        }

        //ConfirmEmail
        [AllowAnonymous]
        [HttpGet("ConfirmEmail")]
        public async Task <IActionResult> ConfirmEmail(string userId , string token)
        {
            if(userId == null ||  token == null)
                return BadRequest("UserId and Token are Require");
            var user = await userManager.FindByIdAsync(userId);
            if(user == null)
                return NotFound("User Not Found");
            var result = await userManager.ConfirmEmailAsync(user,WebUtility.UrlDecode(token));
            if (!result.Succeeded) 
                return BadRequest("Email Confirmed Failed");
            return Ok("Email Confirmed Successfully You Can Now Log In.");
        }

        //ResendEmailConfiremed
        [AllowAnonymous]
        [HttpPost("ResendEmailConfiremed")]
        public async Task <IActionResult> ResendEmailConfiremed([FromBody] ResendEmailConfiremedDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("User Not Found");
            if (await userManager.IsEmailConfirmedAsync(user))
                return BadRequest("Email is Already Confirmed");
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            //Ready To Link
            var confirmationLink = Url.Action(nameof(ConfirmEmail),
                "Account",
                new {userId= user.Id ,Token = WebUtility.UrlEncode(token)},Request.Scheme);

            await emailSender.SendEmailAsync(
                user.Email,
                "Confirm your email",
                $"Click <a href=\"{confirmationLink}\">here</a> to confirm your account."
            );

            return Ok("Confirmation email resent. Please check your inbox.");

        }



        //LogOut
        [Authorize(Roles = "Admin,HR,Employee")]
        [HttpPost("LogOut")]
        public IActionResult LogOut()
        {
            return Ok(new
            {
                Message = "Logged out successfully . Please Remove Token From Client."
            });
        }
        
        //Change Email
        [Authorize(Roles = "Admin,HR,Employee")]
        [HttpPut("Change-Email")]
        public async Task <IActionResult> ChangeEmail(ChangeEmailDTO model)
        {
            if (ModelState.IsValid) 
            {
                var userId =  User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                    return NotFound("User Not Found");
                var ExistingUser = await userManager.FindByEmailAsync(model.NewEmail);
                if(ExistingUser != null)
                    return BadRequest("This email is already taken.");
                var Token = await userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);
                var result = await userManager.ChangeEmailAsync(user,model.NewEmail, Token);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return BadRequest(ModelState);
                }

                user.Email = model.NewEmail;
                await userManager.UpdateAsync(user);
                return Ok("Email changed successfully.");
            }
            return BadRequest(ModelState);
        }

        //Change password
        [Authorize(Roles = "Admin,HR,Employee")]
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO model)
        {
            if(!ModelState.IsValid) 
                return BadRequest(ModelState);
            var userId= User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) 
                return BadRequest("User Not LogIn");
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return BadRequest("User Not Found");
            var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return BadRequest(ModelState);
            }
            return Ok("Password changed successfully");

        }

        //ChangeUserName
        [Authorize(Roles = "Admin,HR,Employee")]
        [HttpPut("ChangeUserName")]
        public async Task <IActionResult> ChangeUserName(ChangeUserNameDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null)
                return BadRequest("User Not LogIn");
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return BadRequest("User Not Found");
            user.UserName = model.NewUserName;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return BadRequest(ModelState);
            }
            return Ok("UserName changed successfully");
        }

        //Delete account
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteAccount")]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAcoountDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("User Is Not Logged In");
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return BadRequest("User Not Found");
            //Check Password
            bool PasswordValid =
                await userManager.CheckPasswordAsync(user, model.Password);
            if(!PasswordValid)
                return BadRequest("IcCorrect Password");
            var result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return BadRequest(ModelState);
            }

            return Ok("Account Deleted Successfully");
        }

        //ForgetPassword
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassord([FromBody] ForgetPasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Ok(new { Message = "if an Account with tahat email exists , a password reset link has been sent" });
            }

            //Genearte Token
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var tokenBytes = Encoding.UTF8.GetBytes(token);
            var eccodedToken = WebEncoders.Base64UrlEncode(tokenBytes);
            var clientUrl = config["Frontend:ResetPasswordUrl"] ?? "https://your-frontend/reset-password";
            var callbackUrl = $"{clientUrl}?email={Uri.EscapeDataString(user.Email)}&token={eccodedToken}";

            // Email body (HTML)
            var html = $@"
            <p>Hi {user.UserName ?? user.Email},</p>
            <p>You requested a password reset. Click the link below to reset your password:</p>
            <p><a href='{callbackUrl}'>Reset your password</a></p>
            <p>If you didn't request this, you can ignore this email.</p>";

            // Send email (await)
            await emailSender.SendEmailAsync(user.Email, "Reset your password", html);

            return Ok(new { message = "If an account with that email exists, a password reset link has been sent." });
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("Invalid request.");
            }

            try
            {
                // Decode token
                var decodedBytes = WebEncoders.Base64UrlDecode(model.Token);
                var token = Encoding.UTF8.GetString(decodedBytes);

                var result = await userManager.ResetPasswordAsync(user, token, model.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                        ModelState.AddModelError("", err.Description);

                    return BadRequest(ModelState);
                }

                return Ok(new { message = "Password has been reset successfully." });
            }
            catch
            {
                return BadRequest("Invalid token.");
            }
        }

        //AboutMe
        [Authorize]
        [HttpGet("AboutMe")]
        public async Task <IActionResult> Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();
            return Ok(new { 
            user.Id,
            user.UserName,
            user.Email
            });
        }
    }


}





