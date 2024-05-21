using AutoMapper;
using AutoMapper.Execution;
using BusinessObject.Object;
using DataAccess.Enums;
using DataAccess.Model.UserModel;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Encoder = DataAccess.Utilities.Encoder;
using EmailUltilities = DataAccess.Utilities.Email;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using DataAccess.Model.EmailModel;
using DataAccess.Model.OperationResultModel;
using DataAccess.Model.VerifyModel;
using DataAccess.Utilities;

namespace DataAccess.DAO
{
    public class UserDAO
    {
        private static UserDAO instance = null;
        private static readonly object instanceLock = new object();
        private UserDAO() { }
        public static UserDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new UserDAO();
                    }
                    return instance;
                }
            }
        }

        public async Task<ResultModel> CreateAccount(UserReqModel RegisterForm)
        {
            using var context = new RmsContext();
            try
            {
                var user = await GetUserByEmail(RegisterForm.Email);

                if (user != null)
                {
                    return new ResultModel { IsSuccess = false, Message = "This email already exists!" };

                }
                else
                {
                    string OTP = GenerateOTP();
                    DateTime expirationTime = DateTime.Now.AddMinutes(10);
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<UserReqModel, User>().ForMember(dest => dest.Password, opt => opt.Ignore());
                    });
                    IMapper mapper = config.CreateMapper();
                    User NewUser = mapper.Map<UserReqModel, User>(RegisterForm);
                    if (RegisterForm.Password == null)
                    {
                        RegisterForm.Password = Encoder.GenerateRandomPassword();
                    }
                    string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataAccess", "TemplateEmail", "FirstInformation.html");
                    string newPath = FilePath.Replace("WPF\\bin\\Debug\\net8.0-windows\\", "");
                    string Html = await File.ReadAllTextAsync(newPath);
                    Html = Html.Replace("{{Email}}", RegisterForm.Email);
                    Html = Html.Replace("{{OTP}}", $"{OTP}");

                    bool emailSent = await EmailUltilities.SendEmail(RegisterForm.Email, "Email Verification", Html);

                    if (emailSent)
                    {
                        NewUser.Otp = OTP;
                        NewUser.Otpexpiration = expirationTime;
                        NewUser.Id = Guid.NewGuid();
                        NewUser.Status = UserStatus.INACTIVE;
                        NewUser.CreatedAt = DateTime.Now;
                        NewUser.Role = UserEnum.OWNER;
                        var HashedPasswordModel = Encoder.CreateHashPassword(RegisterForm.Password);
                        NewUser.Password = HashedPasswordModel.HashedPassword;
                        NewUser.Salt = HashedPasswordModel.Salt;
                        context.Add(NewUser);
                        context.SaveChanges();
                        return new ResultModel { IsSuccess = true, Message = "Account registered IsSuccessfully" };
                    }
                    else
                    {
                        return new ResultModel { IsSuccess = false, Message = "Cannot send email right now!" };

                    }
                }
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace
                };
            }
        }
        public async Task<ResultModel> Login(UserLoginReqModel loginForm)
        {
            using var context = new RmsContext();

            try
            {
                var user = await GetUserByEmail(loginForm.Email);
                if (user == null || user.Status == UserStatus.INACTIVE)
                {
                    return new ResultModel { IsSuccess = false, Message = "Please verify your account" };
                }

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<UserLoginReqModel, User>().ForMember(dest => dest.Password, opt => opt.Ignore());
                });
                IMapper mapper = config.CreateMapper();
                User newUser = mapper.Map<UserLoginReqModel, User>(loginForm);

                var salt = user.Salt;
                var passwordStored = user.Password;
                var verify = Encoder.VerifyPasswordHashed(loginForm.Password, salt, passwordStored);

                if (verify)
                {
                    if (user.Status == UserStatus.RESETPASSWORD)
                    {
                        user.Status = UserStatus.ACTIVE;
                        context.Update(user);
                    }

                    user.LastLoggedIn = DateTime.Now;
                    context.Update(user);
                    await context.SaveChangesAsync();

                    return new ResultModel { IsSuccess = true, Message = "Login Successful", Data = user };
                }
                else
                {
                    return new ResultModel { IsSuccess = false, Message = "Incorrect password" };
                }
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace
                };
            }
        }

        public async Task<ResultModel> VerifyEmail(EmailVerificationReqModel verificationModel)
        {
            using var context = new RmsContext();
            try
            {
                var user = await GetUserByVerificationToken(verificationModel.OTP);
                if (user != null && user.Otpexpiration > DateTime.Now)
                {

                    user.Status = UserStatus.ACTIVE;
                    user.Otp = null;
                    user.Otpexpiration = null;
                    context.Update(user);
                    context.SaveChanges();
                    return new ResultModel { IsSuccess = true, Message = "Email verified IsSuccessfully" };
                }
                else if (user.Otpexpiration < DateTime.Now)
                {

                    return new ResultModel { IsSuccess = false, Message = "Expired verification otp.(10 minutes)" };

                }
                else
                {
                    return new ResultModel { IsSuccess = false, Message = "Wrong verification otp." };

                }
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace
                };
            }
        }


        public async Task<ResultModel> ResetPassword(UserResetPasswordReqModel resetPasswordReqModel)
        {
            using var context = new RmsContext();
            try
            {
                var User = await GetUserByEmail(resetPasswordReqModel.Email);
                if (User == null)
                {
                    return new ResultModel { IsSuccess = false, Message = "The User cannot validate to reset password" };
                }
                if (User.Status != UserStatus.RESETPASSWORD)
                {
                    return new ResultModel { IsSuccess = false, Message = "The request is denied!" };

                }
                var HashedPasswordModel = Encoder.CreateHashPassword(resetPasswordReqModel.Password);
                User.Password = HashedPasswordModel.HashedPassword;
                User.Salt = HashedPasswordModel.Salt;
                User.Status = UserStatus.ACTIVE;
                context.Update(User);
                context.SaveChanges();

                return new ResultModel { IsSuccess = true, Message = "Reset password IsSuccessfully!" };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace
                };
            }
        }

        public async Task<ResultModel> VerifyOTPCode(string email, string otpCode)
        {
            using var context = new RmsContext();
            try
            {
                var user = await GetUserByEmail(email);
                if (user == null)
                {
                    return new ResultModel { IsSuccess = false, Message = "User not found." };
                }

                var otp = await GetOTPByUserId(user.Id);
                if (otp == null || otp.IsUsed || (DateTime.Now - otp.CreatedAt).TotalMinutes > 10)
                {
                    if (otp == null)
                    {
                        return new ResultModel { IsSuccess = false, Message = "OTP not found." };
                    }
                    else if (otp.IsUsed)
                    {
                        return new ResultModel { IsSuccess = false, Message = "OTP has already been used." };
                    }
                    else
                    {
                        return new ResultModel { IsSuccess = false, Message = "OTP has expired." };
                    }
                }

                if (otp.OtpCode != otpCode)
                {
                    return new ResultModel { IsSuccess = false, Message = "Incorrect OTP." };
                }

                otp.IsUsed = true;
                context.Update(otp);
                user.Status = UserStatus.RESETPASSWORD;
                context.Update(user);
                context.SaveChanges();

                return new ResultModel { IsSuccess = true, Message = "Account verified successfully." };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = $"{e.GetType().Name}: {e.Message}\n{e.StackTrace}"
                };
            }
        }

        public async Task<ResultModel> SendOTPEmailRequest(SendOTPReqModel sendOTPReqModel)
        {
            using var context = new RmsContext();
            try
            {
                var User = await GetUserByEmail(sendOTPReqModel.Email);
                if (User == null)
                {
                    return new ResultModel { IsSuccess = false, Message = "The User with this email is invalid" };

                }
                var GetOTP = await GetOTPByUserId(User.Id);
                if (GetOTP != null)
                {
                    if ((DateTime.Now - GetOTP.CreatedAt).TotalMinutes < 2)
                    {
                        return new ResultModel { IsSuccess = false, Message = "Can not send OTP right now, try after 2 minutes" };

                    }
                }

                string OTPCode = GenerateOTP();
                string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataAccess", "TemplateEmail", "ResetPassword.html");
                string newPath = FilePath.Replace("WPF\\bin\\Debug\\net8.0-windows\\", "");
                string Html = await File.ReadAllTextAsync(newPath);
                Html = Html.Replace("{{OTPCode}}", OTPCode);
                Html = Html.Replace("{{toEmail}}", sendOTPReqModel.Email);
                bool check = await EmailUltilities.SendEmail(sendOTPReqModel.Email, "Reset Password", Html);
                if (!check)
                {
                    return new ResultModel { IsSuccess = false, Message = "Email send failed, try again!" };

                }
                Otpverify Otp = new()
                {
                    Id = Guid.NewGuid(),
                    UserId = User.Id,
                    OtpCode = OTPCode,
                    CreatedAt = DateTime.Now,
                    ExpiredAt = DateTime.Now.AddMinutes(10),
                    IsUsed = false
                };
                context.Add(Otp);
                context.SaveChanges();
                return new ResultModel { IsSuccess = true, Message = "An OTP has been send to your email" };

            }
            catch (Exception e)
            {
                return new ResultModel { IsSuccess = false, Message = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace };
            };
        }
        public async Task<Otpverify> GetOTPByUserId(Guid UserId)
        {
            using var context = new RmsContext();
            return await context.Otpverifies.Where(x => x.UserId.Equals(UserId)).OrderByDescending(x => x.CreatedAt).FirstOrDefaultAsync();
        }
        public async Task<User> GetUserByEmail(string Email)
        {
            using var context = new RmsContext();
            return await context.Users.Where(x => x.Email.Equals(Email)).FirstOrDefaultAsync();
        }

        private string GenerateOTP()
        {
            Random rnd = new Random();
            int otp = rnd.Next(100000, 999999);
            return otp.ToString();
        }
        public IEnumerable<User> GetAll()
        {
            using var context = new RmsContext();
            List<User> list = context.Users.ToList();
            return list;
        }
        public async Task<User> GetUserByVerificationToken(string otp)
        {
            using var context = new RmsContext();
            return await context.Users.FirstOrDefaultAsync(u => u.Otp == otp);
        }

        public async Task<User> GetUserById(Guid id)
        {
            using var context = new RmsContext();
            return await context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<User> CheckIfCustomerIsExisted(Guid roomId, string email, string phoneNumber, string citizenIdNumber, string licensePlates)
        {
            using var context = new RmsContext();
            return await context.Users
                .Include(u => u.Rooms)
                .Where(u => u.Rooms.Any(r => r.Id == roomId) &&
                            (u.Email == email ||
                             u.PhoneNumber == phoneNumber ||
                             u.CitizenIdNumber == citizenIdNumber ||
                             u.LicensePlates == licensePlates))
                .FirstOrDefaultAsync();
        }
    }
}
