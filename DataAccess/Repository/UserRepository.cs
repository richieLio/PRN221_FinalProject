using AutoMapper;
using BusinessObject.Object;
using DataAccess.DAO;
using DataAccess.Model.EmailModel;
using DataAccess.Model.OperationResultModel;
using DataAccess.Model.UserModel;
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
        public Task CreateAccount(UserReqModel RegisterForm) => UserDAO.Instance.CreateAccount(RegisterForm);

        public Task<User> GetUserByEmail(string Email) => UserDAO.Instance.GetUserByEmail(Email);

        public Task<User> GetUserByVerificationToken(string otp) => UserDAO.Instance.GetUserByVerificationToken(otp);
        public Task<ResultModel> Login(UserLoginReqModel userLoginReqModel) => UserDAO.Instance.Login(userLoginReqModel);

        public Task ResetPassword(UserResetPasswordReqModel ResetPasswordReqModel) => UserDAO.Instance.ResetPassword(ResetPasswordReqModel);

        public Task VerifyEmail(EmailVerificationReqModel verificationModel) => UserDAO.Instance.VerifyEmail(verificationModel);
    }
}
