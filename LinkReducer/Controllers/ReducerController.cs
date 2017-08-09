using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LinkReducer.Utils;
using LinkReducer.Models;

namespace LinkReducer.Controllers
{
    [Route("api/[controller]")]
    public class ReducerController : Controller
    {
        const string COOKIE_NAME = "_reducer_user_id";

        IStringGenerator _keyGenerator;
        IUriReducerRepository _repository;

        public ReducerController(IStringGenerator generator, IUriReducerRepository repository)
        {
            _keyGenerator = generator;
            _repository = repository;
        }

        [HttpGet]
        public IEnumerable<UriStat> GetUriHistory()
        {
            var user = UserId;
            return _repository.GetHistoryList(user);
        }

        [HttpGet("{key}")]
        public string GetFullUri(string key)
        {
            var user = UserId;
            return _repository.GetFullUri(key);
        }

        [HttpPost]
        public KeyObject CreateShortKey([FromBody] UriObject fullUri)
        {
            var user = UserId;
            var key = new KeyObject { ShortKey = _keyGenerator.GenerateString(6) }; //TODO: обработать случай, если такой ключ уже создан
            var dbkey = _repository.GetOrInsertShortKey(fullUri.FullUri, user, key.ShortKey); 
            return dbkey ?? key;
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
