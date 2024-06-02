using BusinessObject.Object;
using DataAccess.Model.EmailModel;
using DataAccess.Model.OperationResultModel;
using DataAccess.Model.UserModel;
using DataAccess.Model.VerifyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public interface IUserRepository
    {
        Task CreateAccount(UserReqModel RegisterForm);
        Task<User> GetUserByEmail(string Email);
        Task<ResultModel> Login(UserLoginReqModel userLoginReqModel);
        Task<ResultModel> VerifyEmail(EmailVerificationReqModel verificationModel);
        Task<User> GetUserByVerificationToken(string otp);
        Task ResetPassword(UserResetPasswordReqModel ResetPasswordReqModel);
        Task<ResultModel> VerifyOTPCode(string email, string otpCode);
        Task<ResultModel> SendOTPEmailRequest(SendOTPReqModel sendOTPReqModel);
        Task<User> GetUserById(Guid id);
        string GetUserFullName(Guid id);
        Task<ResultModel> ChangePassword(Guid userId, ChangePasswordReqModel changePasswordModel);
        Task<ResultModel> UpdateUserProfile(UserUpdateModel updateModel);

    }
}
