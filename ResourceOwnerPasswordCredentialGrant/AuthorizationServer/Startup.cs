using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Owin;

[assembly: OwinStartup(typeof(AuthorizationServer.Startup))]

namespace AuthorizationServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Setup Authorization Server
            var option = new OAuthAuthorizationServerOptions {
                // アクセストークンエンドポイントの設定
                TokenEndpointPath = new PathString("/OAuth/Token"),

                // HTTPを許可する（リリース時はHTTPSにしないといけないですが、デバックのときはこうしておきましょう）
                AllowInsecureHttp = true,

                // イベントコールバックメソッドの設定
                Provider = new OAuthAuthorizationServerProvider {
                    // ClientIdとClientSecretの検証
                    OnValidateClientAuthentication = ValidateClientAuthentication,
                    // ResourceOwnerCredentialsのときの処理
                    OnGrantResourceOwnerCredentials = GrantResourceOwnerCredentials
                },

                // リフレッシュトークンの生成と受信コールバックの設定
                RefreshTokenProvider = new AuthenticationTokenProvider {
                    OnCreate = CreateRefreshToken,
                    OnReceive = ReceiveRefreshToken,
                },

                // AccessTokenExpireTimeSpanを10分に設定する(省略した場合のデフォルトは20分)
                AccessTokenExpireTimeSpan = new TimeSpan(0, 10, 0)
            };

            app.UseOAuthAuthorizationServer(option);
        }

        private Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;

            // ClientIDとClientSecretをヘッダまたはフォームからGetする
            if (context.TryGetBasicCredentials(out clientId, out clientSecret) ||
                context.TryGetFormCredentials(out clientId, out clientSecret))
            {
                // clientId と clientSecret をチェックして接続を許可する場合は
                // context.Validated();する
                context.Validated();

            }
            return Task.FromResult(0);
        }

        // ResourceOwnerCredentialsのときの処理
        private Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            // usernameとpasswordをGetする
            string username = context.UserName;
            string password = context.Password;

            // username と password をチェックして接続を許可する場合は
            // identityを作成して
            // context.Validated(identity);する

            // identityを生成
            var identity = new ClaimsIdentity(new GenericIdentity(username, OAuthDefaults.AuthenticationType), context.Scope.Select(x => new Claim("urn:oauth:scope", x)));

            // ここでセットしたidentityがTokenになる
            context.Validated(identity);

            return Task.FromResult(0);
        }

        private void CreateRefreshToken(AuthenticationTokenCreateContext context)
        {
            // リフレッシュトークンの有効期限を設定する(1日)
            int expire = 24 * 60 * 60;
            context.Ticket.Properties.ExpiresUtc = new DateTimeOffset(DateTime.Now.AddSeconds(expire));

            context.SetToken(context.SerializeTicket());
        }

        private void ReceiveRefreshToken(AuthenticationTokenReceiveContext context)
        {
            // このおまじないをするとCreateRefreshToken()イベントが発生してアクセストークンとリフレッシュトークンが再生成される
            context.DeserializeTicket(context.Token);
        }

    }
}
