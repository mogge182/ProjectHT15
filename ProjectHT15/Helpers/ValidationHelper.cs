using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using ProjectHT15.Models;
using ProjectHT15.Models.Context;

namespace ProjectHT15.Helpers
{
    public class ValidationHelper
    {
        public static bool CheckUserName(string username, Guid id)
        {

            try
            {
                using (var context = new HermodsProjektEntities())
                {

                    var user = context.Users.FirstOrDefault(x => x.Username == username);
                    if (user.Id == id && username != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }


                }
            }
            catch (Exception)
            {

                throw;
            }


        }

        public static bool CheckEmailAdress(string email, Guid id)
        {

            try
            {
                using (var context = new HermodsProjektEntities())
                {

                    var user = context.Users.FirstOrDefault(x => x.Email == email );
                    if (user == null || user.Id == id)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }


                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
