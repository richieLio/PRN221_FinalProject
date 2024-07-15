using AutoMapper;
using AutoMapper.Execution;
using BusinessObject.Object;
using DataAccess.Enums;
using DataAccess.Model.UserModel;
using Encoder = DataAccess.Utilities.Encoder;
using EmailUltilities = DataAccess.Utilities.Email;
using Microsoft.EntityFrameworkCore;
using DataAccess.Model.EmailModel;
using DataAccess.Model.OperationResultModel;
using DataAccess.Model.VerifyModel;

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

                        NewUser.Id = Guid.NewGuid();
                        NewUser.Status = UserStatus.INACTIVE;
                        NewUser.CreatedAt = DateTime.Now;
                        NewUser.Role = UserEnum.OWNER;
                        var HashedPasswordModel = Encoder.CreateHashPassword(RegisterForm.Password);
                        NewUser.Password = HashedPasswordModel.HashedPassword;
                        NewUser.Salt = HashedPasswordModel.Salt;

                        context.Add(NewUser);
                        context.SaveChanges();



                        var otp = new Otpverify
                        {
                            Id = Guid.NewGuid(),
                            CreatedAt = DateTime.Now,
                            ExpiredAt = DateTime.Now.AddMinutes(10),
                            IsUsed = false,
                            OtpCode = OTP,
                            UserId = NewUser.Id,
                        };

                        context.Otpverifies.Add(otp);

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
                    return new ResultModel { IsSuccess = false, Message = "Email not found" };
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
                var user = await GetUserByVerificationToken(verificationModel.OTP, verificationModel.Email);
                if (user == null)
                {
                    return new ResultModel { IsSuccess = false, Message = "Wrong verification OTP." };
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

                // If OTP is valid
                otp.IsUsed = true;  // Mark the OTP as used
                context.Entry(otp).State = EntityState.Modified;

                user.Status = UserStatus.ACTIVE;
                context.Entry(user).State = EntityState.Modified;

                // Save the changes first to ensure OTP and user status are updated
                await context.SaveChangesAsync();

                // Now remove the OTP from the context
                context.Remove(otp);
                await context.SaveChangesAsync();

                return new ResultModel { IsSuccess = true, Message = "Email verified successfully" };
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
                context.Entry(otp).State = EntityState.Modified;
                user.Status = UserStatus.RESETPASSWORD;
                context.Entry(user).State = EntityState.Modified;
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
            return await context.Otpverifies.AsNoTracking().Where(x => x.UserId.Equals(UserId)).OrderByDescending(x => x.CreatedAt).FirstOrDefaultAsync();
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
        public async Task<User> GetUserByVerificationToken(string otp, string email)
        {
            using var context = new RmsContext();
            var user = await GetUserByEmail(email);


            var otpverify = await context.Otpverifies
                .AsNoTracking()
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OtpCode == otp && !o.IsUsed && o.ExpiredAt > DateTime.UtcNow);

           

            if (otpverify == null)
            {
                return null;
            }

            if (user.Id != otpverify.User.Id)
            {
                return null;
            }
            return otpverify.User;
        }

        public async Task<User> GetUserById(Guid id)
        {
            using var context = new RmsContext();
            return await context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public string GetUserFullName(Guid id)
        {
            using (var context = new RmsContext())
            {
                var user = context.Users.Find(id);
                if (user != null)
                {
                    return user.FullName;
                }
                else
                {
                    return "User not found";
                }
            }
        }

        public async Task<ResultModel> ChangePassword(Guid userId, ChangePasswordReqModel changePasswordModel)
        {
            using var context = new RmsContext();

            ResultModel result = new ResultModel();
            try
            {
                var user = await GetUserById(userId);

                if (user == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404; // Not Found
                    result.Message = "User not found";
                    return result;
                }

                // Verify the old password
                var oldPasswordHash = user.Password;
                var oldPasswordSalt = user.Salt;
                var isOldPasswordValid = Encoder.VerifyPasswordHashed(changePasswordModel.OldPassword, oldPasswordSalt, oldPasswordHash);

                if (!isOldPasswordValid)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Old password is incorrect";
                    return result;
                }

                // Generate new password hash and salt
                var newPasswordHashModel = Encoder.CreateHashPassword(changePasswordModel.NewPassword);
                user.Password = newPasswordHashModel.HashedPassword;
                user.Salt = newPasswordHashModel.Salt;

                context.Update(user);
                await context.SaveChangesAsync();

                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Password updated successfully";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 500; // Internal Server Error
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> UpdateUserProfile(UserUpdateModel updateModel)
        {
            using var context = new RmsContext();
            ResultModel Result = new();
            try
            {
                var user = await GetUserById(updateModel.Id);

                if (user == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "Not found";
                    return Result;
                }
                user.Email = updateModel.Email;
                user.PhoneNumber = updateModel.PhoneNumber;
                user.Address = updateModel.Address;
                user.Gender = updateModel.Gender;
                user.FullName = updateModel.FullName;
                user.Dob = updateModel.Dob;


                context.Update(user);
                await context.SaveChangesAsync();
                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Message = "Profile updated successfully";
            }
            catch (Exception ex)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.Message = ex.Message;

            }
            return Result;
        }
    }
}
