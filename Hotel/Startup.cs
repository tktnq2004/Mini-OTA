using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using System.Configuration;

[assembly: OwinStartup(typeof(Hotel.Startup))]

namespace Hotel
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                ClientId = ConfigurationManager.AppSettings["GoogleClientId"],
                ClientSecret = ConfigurationManager.AppSettings["GoogleClientSecret"],
                SignInAsAuthenticationType = "ApplicationCookie",

                Provider = new GoogleOAuth2AuthenticationProvider
                {
                    OnAuthenticated = context =>
                    {
                        // Thêm ClaimTypes.NameIdentifier nếu chưa có
                        if (!context.Identity.HasClaim(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier))
                        {
                            context.Identity.AddClaim(new System.Security.Claims.Claim(
                                System.Security.Claims.ClaimTypes.NameIdentifier,
                                context.Id)); // context.Id là ID từ Google
                        }
                        return System.Threading.Tasks.Task.FromResult(0);
                    }
                }
            });

        }
    }
}
