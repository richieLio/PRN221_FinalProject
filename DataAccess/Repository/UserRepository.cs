using AutoMapper;
using BusinessObject.Object;
using DataAccess.DAO;
using DataAccess.Model.EmailModel;
using DataAccess.Model.OperationResultModel;
using DataAccess.Model.UserModel;
using DataAccess.Model.VerifyModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace DataAccess.Repository
{
    public class UserRepository : IUserRepository
    {
        public Task<ResultModel> ChangePassword(Guid userId, ChangePasswordReqModel changePasswordModel) => UserDAO.Instance.ChangePassword(userId, changePasswordModel);

        public Task CreateAccount(UserReqModel RegisterForm) => UserDAO.Instance.CreateAccount(RegisterForm);

        public Task<User> GetUserByEmail(string Email) => UserDAO.Instance.GetUserByEmail(Email);

        public Task<User> GetUserById(Guid id) => UserDAO.Instance.GetUserById(id);


        public Task<User> GetUserByVerificationToken(string otp) => UserDAO.Instance.GetUserByVerificationToken(otp);

        public string GetUserFullName(Guid id) => UserDAO.Instance.GetUserFullName(id);

        public Task<ResultModel> Login(UserLoginReqModel userLoginReqModel) => UserDAO.Instance.Login(userLoginReqModel);

        public Task ResetPassword(UserResetPasswordReqModel ResetPasswordReqModel) => UserDAO.Instance.ResetPassword(ResetPasswordReqModel);

        public Task<ResultModel> SendOTPEmailRequest(SendOTPReqModel sendOTPReqModel) => UserDAO.Instance.SendOTPEmailRequest(sendOTPReqModel);

        public Task<ResultModel> UpdateUserProfile(UserUpdateModel updateModel) => UserDAO.Instance.UpdateUserProfile(updateModel);

        public Task VerifyEmail(EmailVerificationReqModel verificationModel) => UserDAO.Instance.VerifyEmail(verificationModel);

        public Task<ResultModel> VerifyOTPCode(string email, string otpCode) =>UserDAO.Instance.VerifyOTPCode(email, otpCode);
    }
}
