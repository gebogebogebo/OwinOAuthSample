using System.Web.Http;

namespace ResourceServer
{
    // http://localhost:38385/api/Me
    // で実行されるWebAPI
    // [Authorize]属性がついているので、AccessTokenが有効な場合だけ実行される
    // AccessTokenの検証はOwinが勝手にやってくれる
    [Authorize]
    public class MeController : ApiController
    {
        public string Get()
        {
            // User.IdentityはApiControllerクラスのメンバ
            // AccessTokenを復号したクラス
            // Nameを返すと呼び出しAppにNameの内容がResponseとして返る
            return this.User.Identity.Name;
        }
    }
}
