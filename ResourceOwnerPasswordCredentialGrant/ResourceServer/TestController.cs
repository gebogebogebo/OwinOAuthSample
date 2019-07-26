using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ResourceServer
{
    // クラスの追加→WebAPIコントローラクラス
    // http://localhost:38385/api/Test
    // で実行されるWebAPI
    // [Authorize]属性がついているので、AccessTokenが有効な場合だけ実行される
    // AccessTokenの検証はOwinが勝手にやってくれる
    [Authorize]
    public class TestController : ApiController
    {
        public IEnumerable<string> Get()
        {
            // this.User.Identity が Token をデコードしたもの
            string value = $"Your Name is {this.User.Identity.Name}";
            return new string[] { "result value1", value };
        }
    }
}