using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using mvcproject.Models;
using PagedList;

namespace mvcproject.Controllers
{

    public class HomeController : Controller
    {
        Logindbcontext ld = new Logindbcontext();

        public string ReturnUrl { get; private set; }

       
        public ActionResult Home()
        {
            return View();
        }
        public ActionResult Home1()
        {
            return View();
        }
        public ActionResult Aboutus()
        {
            return View();
        }
        public ActionResult Contactus()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Login(Login login, string ReturnUrl = "")
        {
            using (Logindbcontext dc = new Logindbcontext())
            {
                var message = "";
                var v = dc.newusers.Where(a => a.Email == login.Email).FirstOrDefault();
                if (v != null)
                {
                    if (!v.IsEmailVerified)
                    {
                        ViewBag.Message = "Please verify your email first";
                        return View();
                    }
                    if (string.Compare(Crypto.Hash(login.Password), v.Password) == 0)
                    {
                        int timeout = login.RememberMe ? 525600 : 20; // 525600 min = 1 year
                        var ticket = new FormsAuthenticationTicket(login.Email, login.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted)
                        {
                            Expires = DateTime.Now.AddMinutes(timeout),
                            HttpOnly = true
                        };
                        Response.Cookies.Add(cookie);
                  
                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            Session["Username"] = v.Username;
                            return RedirectToAction("StudentDetails");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid login credentials.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login credentials.");
                }
            }
           // ViewBag.message = message;
            return View();
        }
        [Authorize]
        public ActionResult StudentDetails(string option, string search, int? pagenumber,string sort)
        {
            ViewBag.SortByName = string.IsNullOrEmpty(sort) ? "descending name" : "";

            ViewBag.SortByGender = sort == "Gender" ? "descending gender" : "Gender";

            var records = ld.students.AsQueryable();
            
            if (option == "Name")
            {
                records=records.Where(x => x.Name.StartsWith(search) || search == null);
            }
            else if (option == "Gender")
            {
                records=records.Where(x => x.Gender == search || search == null);
            }
            else
            {
                records = records.Where(x => x.Address == search || search == null);
            }
            switch(sort)
            {
                case "descending name":
                    records = records.OrderByDescending(x=>x.Name);
                    break;
                case "descending Gender":
                    records = records.OrderByDescending(x => x.Gender);
                    break;
                case "Gender":
                    records = records.OrderBy(x => x.Name);
                    break;
                default:
                    records = records.OrderBy(x => x.Gender);
                    break;
            }
            return View(records.ToPagedList(pagenumber ?? 1, 3));
        }
         
        public ActionResult Details(int id)
        {
            Student std = ld.students.Find(id);
            return View(std);
        }
        public ActionResult Delete(int id)
        {
            Student std = ld.students.Find(id);
            return View(std);
        }
        [HttpPost]
        public ActionResult Delete(Student student, int id)
        {
            var message = "";
            Student std = ld.students.Find(id);
            ld.students.Remove(std);
            ld.SaveChanges();
            message = "delete successfully..............";
            ViewBag.message = message;
            //return View();
           return RedirectToAction("StudentDetails");
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Student std)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                ld.students.Add(std);
                ld.SaveChanges();
                message = "adding successfully..........";
                //}
                //else
                //{
                //    message = "Invalid Request";
                //}
            }
            ViewBag.message = message;
            return RedirectToAction("StudentDetails");
            //return View();
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            Student std = ld.students.Find(id);
            return View(std);
        }
        [HttpPost]
        public ActionResult Edit(Student std)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                ld.Entry(std).State = EntityState.Modified;
                ld.SaveChanges();
                message = "updated successfully.....";
            }
            ViewBag.Message = message;
            return View(std);
        }
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return View();
        }
        [HttpGet]
        public ActionResult NewUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult NewUser([Bind(Exclude = "IsEmailVerified,ActivationCode")] NewUser newuser)
        {
            bool Status = false;
            string message = "";
            // Model Validation 
            if (ModelState.IsValid)
            {
                #region //Email is already Exist 
                var isExist = IsEmailExist(newuser.Email);
                if (isExist)
                {
                    ModelState.AddModelError("", "Email already exist");
                    return View();
                }
                #endregion
                #region Generate Activation Code 
                newuser.ActivationCode = Guid.NewGuid();
                #endregion
                #region  Password Hashing 
                newuser.Password = Crypto.Hash(newuser.Password);
                newuser.ConfirmPassword = Crypto.Hash(newuser.ConfirmPassword); 
                #endregion
                newuser.IsEmailVerified = false;
                #region Save to Database
                using (Logindbcontext dc = new Logindbcontext())
                {
                    dc.newusers.Add(newuser);
                    dc.SaveChanges();
                    Session["Username"] = newuser.Username.ToString();
                    //Send Email to User
                    SendVerificationLinkEmail(newuser.Email, newuser.ActivationCode.ToString());
                    message = "Registration successfully done. Account activation link " +
                        " has been sent to your email id:" + newuser.Email;
                    Status = true;
                }
                #endregion
            }
            else
            {
                message = "Invalid Request";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View();
        }
        [NonAction]
        public bool IsEmailExist(string emailID)
        {
            using (Logindbcontext dc = new Logindbcontext())
            {
                var v = dc.newusers.Where(a => a.Email == emailID).FirstOrDefault();
                return v != null;
            }
        }
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            var message = "";
                var account = ld.newusers.Where(x => x.Email == email).FirstOrDefault();
                if (account != null)
                {
                    string resetcode = Guid.NewGuid().ToString();
                SendVerificationLinkEmail(account.Email, resetcode, "ResetPassword");
                    account.ResetPasswordCode = resetcode;
                    ld.Configuration.ValidateOnSaveEnabled = false;
                    ld.SaveChanges();
                 message = "Reset password link has been sent to EmailId...";
                }
                else
                {
                    ModelState.AddModelError("", "Invalid EmailId");   
                }
            ViewBag.message = message;
                return View();
        }
        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/Home/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            var verifyUrl2 = "/Home/VerifyAccount2/" + activationCode;
            var link2 = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl2);
            var fromEmail = new MailAddress("dpurushotham19@gmail.com", "Ojas-it");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "9963954103";
            string subject = "";
            string body = "";
            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is successfully created!";
                body = "<br/><br/>We are excited to tell you that your Dotnet Awesome account is" +
                    " successfully created. Please click on the below link to verify your account" +
                    " <br/><br/><a href='" + link + "'>" + link + "</a> ";
            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Hi,<br/>br/>We got request for reset your account password. Please click on the below link to reset your password" +
                    "<br/><br/><a href=" + link2 + ">Reset Password link</a>";
            }
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };
            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }
        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (Logindbcontext dc = new Logindbcontext())
            {
                dc.Configuration.ValidateOnSaveEnabled = false; // This line I have added here to avoid 
                                                                // Confirm password does not match issue on save changes
                var v = dc.newusers.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.IsEmailVerified = true;
                    dc.SaveChanges();
                    Status = true;
                    //return RedirectToAction("ResetPassword",new { Id = id });
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
            }
            ViewBag.Status = Status;
            return View();
        }
        [HttpGet]
        public ActionResult VerifyAccount2(string id)
        {
            bool Status = false;
            using (Logindbcontext dc = new Logindbcontext())
            {
                dc.Configuration.ValidateOnSaveEnabled = false; // This line I have added here to avoid 
                                                                // Confirm password does not match issue on save changes
                var v = dc.newusers.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
                if (v != null)
                {
                    v.IsEmailVerified = true;
                    dc.SaveChanges();
                    Status = true;
                    return RedirectToAction("ResetPassword", new { Id = id });
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
            }
            ViewBag.Status = Status;
            return View();
        }

        [HttpGet]
        public ActionResult ResetPassword(string Id)
        {
            //Verify the reset password link
            //Find account associated with this link
            //redirect to reset password page
            if (string.IsNullOrWhiteSpace(Id))
            {
                return HttpNotFound();
            }

            //using (MyDatabaseEntities dc = new MyDatabaseEntities())
            //{
            var user = ld.newusers.Where(a => a.ResetPasswordCode == Id).FirstOrDefault();
                if (user != null)
                {
                    ResetPassword model = new ResetPassword
                    ();
                    //model.ResetCode = Id;
                    return View();
                }
                else
                {
                    return HttpNotFound();
                }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPassword model,string Id)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                    var user = ld.newusers.Where(a => a.ResetPasswordCode == Id).FirstOrDefault();
                    if (user != null)
                    {
                        user.Password = Crypto.Hash(model.NewPassword);
                        user.ConfirmPassword  = Crypto.Hash(model.ConfirmPassword);
                        user.ResetPasswordCode = "";
                        ld.Configuration.ValidateOnSaveEnabled = false;
                        ld.SaveChanges();
                        message = "New password updated successfully";
                    }
            }
            else
            {
                message = "Something invalid";
            }
            ViewBag.Message = message;
            return View(model);
        }
    }
    //[HttpPost]
    //public ActionResult ResetPassword(ResetPassword reset)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        bool resetResponse = WebSecurity.ResetPassword(reset.Resetcode, reset.NewPassword);
    //        if (resetResponse)
    //        {
    //            ViewBag.Message = "Successfully Changed";
    //        }
    //        else
    //        {
    //            ViewBag.Message = "Something went horribly wrong!";
    //        }
    //    }
    //    return View(reset);
    //}
    //// POST: Account/LostPassword
    //[HttpPost]
    //public ActionResult ForgotPassword(ResetPassword model)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        MembershipUser user;
    //        using (var context = new Logindbcontext())
    //        {
    //            var foundUserName = (from u in context.newusers
    //                                 where u.Email == model.Email
    //                                 select u.Username).FirstOrDefault();
    //            if (foundUserName != null)
    //            {
    //                // Generae password token that will be used in the email link to authenticate user
    //                var token = WebSecurity.GeneratePasswordResetToken(foundUserName);
    //                // Generate the html link sent via email
    //                string resetLink = "<a href='"
    //                   + Url.Action("ResetPassword", "Account", new { rt = token }, "http")
    //                   + "'>Reset Password Link</a>";
    //                // Email stuff
    //                string subject = "Reset your password for asdf.com";
    //                string body = "You link: " + resetLink;
    //                string from = "purushotham.d@ojas-it.com";
    //                MailMessage message = new MailMessage(from, model.Email);
    //                message.Subject = subject;
    //                message.Body = body;
    //                SmtpClient client = new SmtpClient();
    //                // Attempt to send the email
    //                try
    //                {
    //                    client.Send(message);
    //                }
    //                catch (Exception e)
    //                {
    //                    ModelState.AddModelError("", "Issue sending email: " + e.Message);
    //                }
    //            }
    //            else // Email not found
    //            {
    //                /* Note: You may not want to provide the following information
    //                * since it gives an intruder information as to whether a
    //                * certain email address is registered with this website or not.
    //                * If you're really concerned about privacy, you may want to
    //                * forward to the same "Success" page regardless whether an
    //                * user was found or not. This is only for illustration purposes.
    //                */
    //                ModelState.AddModelError("", "No user found by that email.");
    //            }
    //        }
    //        /* You may want to send the user to a "Success" page upon the successful
    //        * sending of the reset email link. Right now, if we are 100% successful
    //        * nothing happens on the page. :P
    //        */
    //    }
    //    return View(model);
    //}

}


        

  