using BusinessObject.Object;
using DataAccess.Model.EncodeModel;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DataAccess.Utilities
{
    public class Encoder
    {

        public Encoder()
        {
        }

        static string GenerateSalt()
        {
            int SaltLength = 16;

            byte[] Salt = new byte[SaltLength];

            using (var Rng = new RNGCryptoServiceProvider())
            {
                Rng.GetBytes(Salt);
            }

            return BitConverter.ToString(Salt).Replace("-", "");
        }

        public static CreateHashPasswordModel CreateHashPassword(string Password)
        {
            string SaltString = GenerateSalt();
            byte[] Salt = Encoding.UTF8.GetBytes(SaltString);
            byte[] PasswordByte = Encoding.UTF8.GetBytes(Password);
            byte[] CombinedBytes = CombineBytes(PasswordByte, Salt);
            byte[] HashedPassword = HashingPassword(CombinedBytes);
            return new CreateHashPasswordModel()
            {
                Salt = Encoding.UTF8.GetBytes(SaltString),
                HashedPassword = HashedPassword
            };
        }



        public static bool VerifyPasswordHashed(string Password, byte[] Salt, byte[] PasswordStored)
        {
            byte[] PasswordByte = Encoding.UTF8.GetBytes(Password);
            byte[] CombinedBytes = CombineBytes(PasswordByte, Salt);
            byte[] NewHash = HashingPassword(CombinedBytes);
            return PasswordStored.SequenceEqual(NewHash);
        }


        static byte[] HashingPassword(byte[] PasswordCombined)
        {
            using (SHA256 SHA256 = SHA256.Create())
            {
                byte[] HashBytes = SHA256.ComputeHash(PasswordCombined);
                return HashBytes;
            }
        }

        static byte[] CombineBytes(byte[] First, byte[] Second)
        {
            byte[] Combined = new byte[First.Length + Second.Length];
            Buffer.BlockCopy(First, 0, Combined, 0, First.Length);
            Buffer.BlockCopy(Second, 0, Combined, First.Length, Second.Length);
            return Combined;
        }


        public static string GenerateRandomPassword()
        {
            int length = 12;
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
            byte[] data = new byte[length];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                byte[] buffer = new byte[sizeof(int)];
                StringBuilder result = new StringBuilder(length);
                for (int i = 0; i < length; i++)
                {
                    crypto.GetBytes(buffer);
                    int randomNumber = BitConverter.ToInt32(buffer, 0);
                    randomNumber = Math.Abs(randomNumber);
                    int index = randomNumber % chars.Length;
                    result.Append(chars[index]);
                }
                return result.ToString();
            }
        }

    }
}
