using JohnsonNet.Serialization;
using JohnsonNet.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestWeb.Controller.Api
{
    public class TestController : ApiController
    {
        public TestController()
        {
            this.Authenticater = new TokenAuthenticater();
            this.Serializer = new JsonSerializer { ToLowerCase = true };
        }
        [Authenticate]
        public GenericOutput<object> GetMember(string param, Member member)
        {
            return new GenericOutput<object>
            {
                Data = new
                {
                    param,
                    member
                }
            };
        }
    }
}