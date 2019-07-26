using Constants;
using DotNetOpenAuth.OAuth2;
using System;
using System.Net.Http;
using System.Web.Mvc;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private WebServerClient _webServerClient;

        public ActionResult Index()
        {
            ViewBag.AccessToken = Request.Form["AccessToken"] ?? "";
            ViewBag.Action = "";
            ViewBag.ApiResponse = "";

            InitializeWebServerClient();
            var accessToken = Request.Form["AccessToken"];
            if (string.IsNullOrEmpty(accessToken))
            {
                var authorizationState = _webServerClient.ProcessUserAuthorization(Request);
                if (authorizationState != null)
                {
                    ViewBag.AccessToken = authorizationState.AccessToken;
                    ViewBag.Action = Request.Path;
                }
            }

            if (!string.IsNullOrEmpty(Request.Form.Get("submit.Authorize")))
            {
                var userAuthorization = _webServerClient.PrepareRequestUserAuthorization(new[] { "リソースサーバーにアクセスする", "一般モード" });
                userAuthorization.Send(HttpContext);
                Response.End();
            }
            else if (!string.IsNullOrEmpty(Request.Form.Get("submit.Refresh")))
            {
                var state = new AuthorizationState
                {
                    AccessToken = Request.Form["AccessToken"],
                };
                if (_webServerClient.RefreshAuthorization(state))
                {
                    ViewBag.AccessToken = state.AccessToken;
                }
            }
            else if (!string.IsNullOrEmpty(Request.Form.Get("submit.CallApi")))
            {
                var resourceServerUri = new Uri(Paths.ResourceServerBaseAddress);
                var client = new HttpClient(_webServerClient.CreateAuthorizingHandler(accessToken));
                var body = client.GetStringAsync(new Uri(resourceServerUri, Paths.MePath)).Result;
                ViewBag.ApiResponse = body;
            }

            return View();
        }

        private void InitializeWebServerClient()
        {
            var authorizationServerUri = new Uri(Paths.AuthorizationServerBaseAddress);
            var authorizationServer = new AuthorizationServerDescription
            {
                AuthorizationEndpoint = new Uri(authorizationServerUri, Paths.AuthorizePath),
                TokenEndpoint = new Uri(authorizationServerUri, Paths.TokenPath)
            };

            // ClientIDとSecretをここで設定する
            _webServerClient = new WebServerClient(authorizationServer, Clients.Client1.Id, Clients.Client1.Secret);
        }
    }
}