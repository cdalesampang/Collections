using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Collection.Common.Utilities
{
    public class Utility
    {
        public string Encryption(string password)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encrypt;
            UTF8Encoding encode = new UTF8Encoding();
            encrypt = md5.ComputeHash(encode.GetBytes(password));
            StringBuilder encryptedData = new StringBuilder();
            for (int i = 0; i < encrypt.Length; i++)
            {
                encryptedData.Append(encrypt[i].ToString());
            }

            return encryptedData.ToString();
        }

    }
}
