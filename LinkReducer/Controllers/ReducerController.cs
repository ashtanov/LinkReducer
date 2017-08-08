using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LinkReducer.Utils;

namespace LinkReducer.Controllers
{
    [Route("api/[controller]")]
    public class ReducerController : Controller
    {
        const string COOKIE_NAME = "_reducer_user_id";

        IStringGenerator _keyGenerator;

        public ReducerController(IStringGenerator generator)
        {
            _keyGenerator = generator;
        }

        [HttpGet]
        public IEnumerable<string> GetUriHistory()
        {
            var user = UserId;
            return new string[] { "value1", "value2" };
        }

        [HttpGet("{key}")]
        public string GetFull(int key)
        {
            var user = UserId;
            return user.ToString();
        }

        [HttpPost]
        public string CreateLink([FromBody]string fullUri)
        {
            var user = UserId;
            var key = _keyGenerator.GenerateString(6);
            return key;
        }

        private Guid UserId
        {
            get
            {
                if (!Guid.TryParse(Request.Cookies[COOKIE_NAME], out Guid id))
                {
                    id = Guid.NewGuid();
                    Response.Cookies.Append(
                        COOKIE_NAME, 
                        id.ToString("N"),
                        new Microsoft.AspNetCore.Http.CookieOptions
                        {
                            Expires = new DateTimeOffset(DateTime.Now.AddDays(30))
                        });
                }
                return id;

            }
        }
    }
}
