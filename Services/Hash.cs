using System;
using System.Security.Cryptography;
using System.Text;

namespace StudyMATEUpload.Services
{
    public static class Hash
    {
        public static string GetHashedValue(string password)
        {
            //using SHA512 to generate the hashing
            using SHA256 hashSvc = SHA256.Create();
            //creating the hash
            byte[] hash = hashSvc.ComputeHash(Encoding.UTF8.GetBytes(password));
            //using bitconverter, we convert the hased bits into hex removing the '-'
            string hex = BitConverter.ToString(hash).Replace("-", "");
            return hex;
        }
    }
}
