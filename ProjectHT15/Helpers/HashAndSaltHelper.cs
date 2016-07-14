using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using ProjectHT15.Models;

namespace ProjectHT15.Helpers
{
    public static class HashAndSaltHelper
    {
        public static string PasswordHash(LoginModel hs)
        {
            var cArray = hs.Salt.ToCharArray();
            Array.Reverse(cArray);
            

            string saltAndPassword = String.Concat(hs.Password, new string(cArray));
            string hashedPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(saltAndPassword, "sha1");

            return hashedPwd;
        }
    }
}
