using BCrypt.Net;
using Hotel.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security;

public class UserController : Controller
{
    dbHotelDataContext db = new dbHotelDataContext();

    public ActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Register(FormCollection collection, User c)
    {
        var name = collection["fullname"];
        var username = collection["username"];
        var password = collection["password"];
        var confirmpassword = collection["confirmpassword"];
        var email = collection["email"];
        var address = collection["address"];
        var numberphone = collection["numberphone"];

        if (String.IsNullOrEmpty(confirmpassword))
        {
            ViewData["enterpassword"] = "Must enter password to confirm";
        }
        else
        {
            if (!password.Equals(confirmpassword))
            {
                ViewData["samepassword"] = "Password and Confirmpassword must be the same";
            }
            else
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                c.FullName = name;
                c.Username = username;
                c.Password = hashedPassword;
                c.Email = email;
                c.Phone = numberphone;
                c.Role = "Customer";

                db.Users.InsertOnSubmit(c);
                db.SubmitChanges();
                return RedirectToAction("Login");
            }
        }

        return this.Register();
    }

    [HttpGet]
    public ActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Login(FormCollection collection)
    {
        var username = collection["username"];
        var password = collection["password"];

        var c = db.Users.SingleOrDefault(n => n.Username == username);
        if (c != null && BCrypt.Net.BCrypt.Verify(password, c.Password))
        {
            ViewBag.ThongBao = "Congratulations on successful login";
            Session["User"] = c;
        }
        else
        {
            ViewBag.ThongBao = "Username or password is incorrect";
        }

        return RedirectToAction("HotelIndex", "Hotel");
    }

    [HttpGet]
    public ActionResult Profile()
    {
        if (Session["User"] == null)
        {
            return RedirectToAction("Login");
        }

        User c = (User)Session["User"];
        return View(c);
    }

    [HttpPost]
    public ActionResult Profile(FormCollection collection)
    {
        User currentUser = (User)Session["User"];
        User c = db.Users.SingleOrDefault(u => u.UserID == currentUser.UserID);

        var name = collection["fullname"];
        var username = collection["username"];
        var password = collection["password"];
        var changepassword = collection["changepassword"];
        var email = collection["email"];
        var address = collection["address"];
        var numberphone = collection["numberphone"];

        if (String.IsNullOrEmpty(password))
        {
            c.FullName = name;
            c.Username = username;
            c.Email = email;
            c.Phone = numberphone;

            db.SubmitChanges();
            Session["User"] = c;
            return RedirectToAction("Profile");
        }

        if (String.IsNullOrEmpty(changepassword))
        {
            ViewData["enterpassword"] = "Must enter password to confirm";
            return View(c);
        }

        if (!password.Equals(changepassword))
        {
            ViewData["samepassword"] = "Password and Confirmpassword must be the same";
            return View(c);
        }

        c.FullName = name;
        c.Username = username;
        c.Password = BCrypt.Net.BCrypt.HashPassword(password);
        c.Email = email;
        c.Phone = numberphone;

        db.SubmitChanges();
        Session["User"] = c;

        return RedirectToAction("Profile");
    }

    public ActionResult Logout()
    {
        Session["User"] = null;
        return RedirectToAction("HotelIndex", "Hotel");
    }
    public ActionResult ExternalLogin(string provider, string returnUrl)
    {
        System.Diagnostics.Debug.WriteLine($"🔁 ExternalLogin được gọi. Provider = {provider}, ReturnUrl = {returnUrl}");
        return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "User", new { ReturnUrl = returnUrl }));
    }


    public ActionResult ExternalLoginCallback(string returnUrl)
    {
        System.Diagnostics.Debug.WriteLine("📥 ExternalLoginCallback được gọi.");

        var authResult = HttpContext.GetOwinContext().Authentication.AuthenticateAsync("ExternalCookie").Result;
        if (authResult?.Identity == null)
        {
            System.Diagnostics.Debug.WriteLine("❌ authResult.Identity null. Đăng nhập thất bại.");
            ViewBag.Error = "Không thể đăng nhập bằng Google.";
            return RedirectToAction("Login");
        }

        var claims = authResult.Identity.Claims.ToList();
        foreach (var claim in claims)
        {
            System.Diagnostics.Debug.WriteLine($"🔐 Claim - Type: {claim.Type}, Value: {claim.Value}");
        }

        var email = claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
        var name = claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            System.Diagnostics.Debug.WriteLine("❌ Không lấy được email từ claim.");
            return RedirectToAction("Login");
        }

        System.Diagnostics.Debug.WriteLine($"✅ Lấy được email: {email}, name: {name}");

        var user = db.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            System.Diagnostics.Debug.WriteLine("ℹ️ Người dùng chưa tồn tại. Tạo mới...");

            user = new User
            {
                FullName = name ?? email,
                Email = email,
                Username = email,
                Role = "Customer",
                Password = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString())
            };

            try
            {
                db.Users.InsertOnSubmit(user);
                db.SubmitChanges();
                System.Diagnostics.Debug.WriteLine("✅ User mới đã được lưu vào DB.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Lỗi khi lưu user: {ex.Message}");
                ViewBag.Error = "Lỗi khi lưu thông tin người dùng.";
                return RedirectToAction("Login");
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("✅ Người dùng đã tồn tại. Đăng nhập tiếp...");
        }

        // Tạo ClaimsIdentity cho OWIN
        var identity = new System.Security.Claims.ClaimsIdentity("ApplicationCookie");
        identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Username));
        identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email));
        identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Email));

        System.Diagnostics.Debug.WriteLine("✅ Tạo OWIN Identity xong. Bắt đầu SignIn...");
        HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties
        {
            IsPersistent = true
        }, identity);

        Session["User"] = user;
        System.Diagnostics.Debug.WriteLine("✅ Gán Session xong.");

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            System.Diagnostics.Debug.WriteLine($"➡️ Redirect về returnUrl: {returnUrl}");
            return Redirect(returnUrl);
        }

        System.Diagnostics.Debug.WriteLine("➡️ Redirect về HotelIndex (default).");
        return RedirectToAction("HotelIndex", "Hotel");
    }

    internal class ChallengeResult : HttpUnauthorizedResult
    {
        public ChallengeResult(string provider, string redirectUri)
        {
            LoginProvider = provider;
            RedirectUri = redirectUri;
        }

        public string LoginProvider { get; set; }
        public string RedirectUri { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
            context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
        }
    }
}
