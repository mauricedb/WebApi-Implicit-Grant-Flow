using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApiImplicitGrantFlow
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var cookie = new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookie",
            };
            app.UseCookieAuthentication(cookie);

            // ToDo: Enter application client info
            // Register the application at the Google Developers Console (https://console.developers.google.com/)
            // Copy the ClientId and ClientSecret from the OAuth settings under APIs & auth
            var google = new GoogleOAuth2AuthenticationOptions
            {
                ClientId = ,
                ClientSecret = ,
                AuthenticationType = "Google",
                SignInAsAuthenticationType = "Cookie"
            };
            app.UseGoogleAuthentication(google);

            var oauth = new OAuthAuthorizationServerOptions
            {
                AuthenticationType = "Bearer",
                AllowInsecureHttp = true,
                AuthorizeEndpointPath = new PathString("/authorize"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(1),
                Provider = new OAuthAuthorizationServerProvider
                {
                    OnValidateClientRedirectUri = ctx =>
                    {
                        var client = ctx.ClientId;
                        var redirect = ctx.RedirectUri;

                        if (client == "myclient" &&
                            redirect == "http://localhost:64037/callback.html")
                        {
                            ctx.Validated();
                        }

                        return Task.FromResult(0);
                    }
                }
            };
            app.UseOAuthAuthorizationServer(oauth);

            app.Map("/authorize", authorize => authorize.Run(ctx =>
            {
                var user = ctx.Authentication.User;
                if (user == null || !user.Identity.IsAuthenticated)
                {
                    ctx.Authentication.Challenge("Google");
                    ctx.Response.StatusCode = 401;
                }
                else
                {
                    var claims = user.Claims;
                    var ci = new ClaimsIdentity(claims, "Bearer");
                    ctx.Authentication.SignIn(ci);
                }

                return Task.FromResult(0);
            }));

            var bearer = new OAuthBearerAuthenticationOptions();
            app.UseOAuthBearerAuthentication(bearer);

            app.Map("/api", apiApp =>
            {
                var config = new HttpConfiguration();
                config.MapHttpAttributeRoutes();
                config.SuppressDefaultHostAuthentication();
                config.Filters.Add(new HostAuthenticationAttribute("Bearer"));
                apiApp.UseWebApi(config);
            });
        }
    }
}