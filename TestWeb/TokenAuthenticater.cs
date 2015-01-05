using JohnsonNet.WebAPI;
using System;
using System.Web;

namespace TestWeb
{
    public class TokenAuthenticater : JohnsonNet.WebAPI.IApiAuthenticater
    {
        public object Authenticate(HttpContext context)
        {
            string token = context.Request["access_token"];

            if (token.EqualsWithCurrentCulture("test_token"))
            {
                return new Member
                {
                    FirstName = "Ad",
                    LastName = "Soyad",
                    Token = token
                };
            };

            return null;
        }


        //DateTime d = DateTime.Now.AddDays(1);
        //int day = d.Day;
        //int month = d.Month;
        //int year = d.Year;
        //DateTime expireDate = new DateTime(year, month, day);

        //MD5 hash = MD5.Create();
        //byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(dto.FirstName + expireDate.ToString()));
        //StringBuilder sb = new StringBuilder();
        //for (int i = 0; i < data.Length; i++) sb.Append(data[i].ToString("x2"));
        //accessToken = sb.ToString();
    }
    public class Member
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Token { get; set; }
    }
}