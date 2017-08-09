using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LinkReducer.Utils;
using LinkReducer.Models;
using Microsoft.Extensions.Options;

namespace LinkReducer.Controllers
{
    [Route("api/[controller]")]
    public class ReducerController : Controller
    {
        const string COOKIE_NAME = "_reducer_user_id";

        private readonly IStringGenerator _keyGenerator;
        private readonly IUriReducerRepository _repository;
        private readonly int _currentKeyLength;


        public ReducerController(IStringGenerator generator, IUriReducerRepository repository, IOptions<Settings> settings)
        {
            _keyGenerator = generator;
            _repository = repository;
            _currentKeyLength = settings.Value.KeyLettersCount;
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
        public string CreateShortKeyPost([FromBody] UriObject fullUri)
        {
            return CreateShortKeyInternal(fullUri.Uri).ShortKey;
        }

        [HttpGet("create")]
        public string CreateShortKeyGet([FromQuery] string uri)
        {
            return CreateShortKeyInternal(uri).ShortKey;
        }

        private KeyObject CreateShortKeyInternal(string fullUri)
        {
            var user = UserId;
            var key = _keyGenerator.GenerateString(_currentKeyLength);
            KeyObject uniqKey;

            do
            {
                uniqKey = _repository.GetOrInsertShortKey(fullUri, user, key);
            } while (!uniqKey.Success);
            
            return uniqKey;
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
