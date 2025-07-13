using BCrypt.Net;
using Hotel.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

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

    //private string GetSHA256Hash(string input)
    //{
    //    using (SHA256 sha256 = SHA256.Create())
    //    {
    //        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
    //        StringBuilder builder = new StringBuilder();
    //        foreach (var b in bytes)
    //            builder.Append(b.ToString("x2"));
    //        return builder.ToString();
    //    }
    //}
}
