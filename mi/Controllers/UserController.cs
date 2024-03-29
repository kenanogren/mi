﻿using mi.Areas.Panel.Models;
using mi.Areas.Panel.Models.Repository;
using mi.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using Login = mi.Models.ViewModel.Login;

namespace mi.Controllers
{
    public class UserController : Controller
    {
        CategoryRepository repoCategory = new CategoryRepository(new Areas.Panel.Models.MoriconEntities());
        BrandRepository repoBrand = new BrandRepository(new Areas.Panel.Models.MoriconEntities());
        UserRepository repoUser = new UserRepository(new Areas.Panel.Models.MoriconEntities());

        // GET: User
       
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(register register)
        {
            bool status = false;
            string message = "";
            if (ModelState.IsValid)
            {
                if (isExistUser(register.email))
                {
                    message = "Bu maile kayıtlı bir kullanıcı mevcuttur.";
                    ViewBag.message = message;
                    return View();
                }
                user user = new user();
                user.email = register.email;
                // parolalar şifreleniyor
                user.password = Crypto.Hash(register.password);
                user.rePassword = Crypto.Hash(register.comfirmPassword);
                // aktivasyon kodu üretiyoruz.
                user.activationCode = Guid.NewGuid().ToString();
                user.roleId = 2;
                // oluşturulan kullanıcının mail doğrulması başlangıçta olsun.
                user.isMailVerified = false;

                user.createdDate = DateTime.Now;
                //Kaydet
                repoUser.Save(user);
                // mail gönder
                SendVerificationLinkEmail(user.email, user.activationCode);

                message = "Kaydınız başarılı şekilde gerçekleştirildi. Hesap aktivasyon linki "
                     + user.email + " adresinize gönderilmiştir.Doğrulamayı unutmayınız.";
                status = true;
                ViewBag.message = message;
                ViewBag.status = status;
            }
            return View();
        }

        [NonAction]
        public bool isExistUser(string username)
        {
            var user = repoUser.GetAll().Where(a => a.email == username).FirstOrDefault();
            return user == null ? false : true;
        }

        //mail adresinizdeki doğrulmayı yapmak için
        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool status = false;
            //db.Configuration.ValidateOnSaveEnabled = false;
            var result = repoUser.GetAll().Where(a => a.activationCode == new Guid(id).ToString()).FirstOrDefault();
            if (result != null)
            {
                result.isMailVerified = true;
                repoUser.Update(result);
                status = true;
                ViewBag.status = status;
                ViewBag.message = "Hesabınız başarıyla aktive edilmiştir. Giriş yapabilirsiniz";

            }
            else
            {
                ViewBag.Status = status;
                ViewBag.message = "Geçersiz istek";
            }

            return View("Login");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login login, string ReturnUrl)
        {
            string message = "";
            int sayac = 0;
            bool status = false;
            if (ModelState.IsValid)
            {
                user _user = repoUser.GetAll().Where(x => x.email == login.email).FirstOrDefault();
                if (_user == null)
                {
                    message = "Böyle bir Email kayıtlı değil";
                    ViewBag.message = message;
                    ViewBag.status = status;
                    return View();
                }
                // kullanıcı mail doğrulaması yapmışsa al, yapmamışsa false al.
                bool verify = _user.isMailVerified ?? false;
                if (!verify)
                {
                    message = " Mail doğrulaması yapılmamış";
                    ViewBag.message = message;
                    ViewBag.status = status;
                    sayac++;
                    _user.loginAttempt = sayac;
                    repoUser.Update(_user);
                }
                // kullanıcı aktif değilse
                if (_user.isActive == false)
                {
                    sayac++;
                    message = "Hesabınız askıya alınmıştır";
                    ViewBag.message = message;
                    _user.loginAttempt = sayac;
                    repoUser.Update(_user);
                }
                login.password = Crypto.Hash(login.password);
                // şifreler tutuyorsa
                if (string.Compare(login.password, _user.password) == 0)
                {
                    _user.loginTime = DateTime.Now;
                    _user.loginAttempt = sayac;

                    repoUser.Update(_user);

                    Session["username"] = _user.email;
                    int timeOut = login.rememberMe ? 60 : 10;
                    // form autentication oluşturalım.
                    var ticket = new FormsAuthenticationTicket(login.email, login.rememberMe, timeOut);
                    string encrypted = FormsAuthentication.Encrypt(ticket);
                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                    cookie.Expires = DateTime.Now.AddMinutes(timeOut);
                    cookie.HttpOnly = true;
                    // Cookie ekleniyor
                    FormsAuthentication.SetAuthCookie("userName", login.rememberMe);
                    Response.Cookies.Add(cookie);

                    // admin için
                    if (_user.roleId == 1)
                        return Redirect("~/Panel/Category");
                    // return Url yerel bir url ise
                    if (Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Shop");
                    }
                }
                else
                {
                    sayac++;
                    _user.loginAttempt = sayac;
                    repoUser.Update(_user);
                    message = "Giriş yapılamadı.Parola yanlış!";
                }

            }
            ViewBag.status = status;
            ViewBag.message = message;
            return View();
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string email)
        {
            string message = "";
            bool status = false;
            if (!string.IsNullOrEmpty(email))
            {
                user _user = repoUser.GetAll().Where(x => x.email == email).FirstOrDefault();
                if (_user != null)
                {
                    Guid resetCode = Guid.NewGuid();
                    _user.resetCode = resetCode.ToString();
                    repoUser.Update(_user);

                    SendResetCodeEmail(email, resetCode.ToString());
                    status = true;
                    message = "Parola Sıfırlama işlemi başarılı şekilde gerçekleştirildi. Parola sifirlama linki "
                            + email + " adresinize gönderilmiştir.";

                }
                else
                {
                    message = "Geçersiz Eposta";
                }
            }
            else
            {
                message = "Email alanı boş olamaz";
            }
            ViewBag.message = message;
            ViewBag.status = status;
            return View();
        }
        public ActionResult ResetPassword(string id)
        {
            ResetPassword rp = new ResetPassword()
            {
                resetCode = id
            };
            return View(rp);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPassword rp)
        {
            string message = "";
            bool status = false;
            if (ModelState.IsValid)
            {
                user _user = repoUser.GetAll().Where(x => x.resetCode == rp.resetCode).FirstOrDefault();
                if (_user != null)
                {
                    _user.password = Crypto.Hash(rp.newPassword);
                    _user.rePassword = Crypto.Hash(rp.comfirmPassword);
                    repoUser.Update(_user);
                    status = true;
                    message = "Şifreniz başarıyla değiştirildi";
                }
                else
                    message = "Geçersiz kod";
            }
            ViewBag.message = message;
            ViewBag.status = status;
            return View();
        }



        public ActionResult Signout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [NonAction]
        public void SendResetCodeEmail(string emailID, string resetCode)
        {
            SmtpSection network = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            try
            {
                var resetUrl = "/User/ResetPassword/" + resetCode;
                var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, resetUrl);
                var fromEmail = new MailAddress(network.Network.UserName, "Goldstore Parola Sıfırlama");
                var toEmail = new MailAddress(emailID);

                string subject = "Goldstore Parola Sıfırlama!";
                string body = "<br/><br/>İsteğiniz üzere Goldstore Parola sıfırlama işleminiz talebi alınmıştır. Onaylamak için aşağıdaki bağlantıya tıklayınız" +
                    " <br/><br/><a href='" + link + "'>" + link + "</a> ";
                var smtp = new SmtpClient
                {
                    Host = network.Network.Host,
                    Port = network.Network.Port,
                    EnableSsl = network.Network.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = network.Network.DefaultCredentials,
                    Credentials = new NetworkCredential(network.Network.UserName, network.Network.Password)
                };
                using (var message = new MailMessage(fromEmail, toEmail)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                    smtp.Send(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode)
        {
            SmtpSection network = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            try
            {
                var verifyUrl = "/User/VerifyAccount/" + activationCode;
                var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
                var fromEmail = new MailAddress(network.Network.UserName, "Goldstore Üyeliği");
                var toEmail = new MailAddress(emailID);

                string subject = "Goldstore sitemize Hoşgeldiniz!";
                string body = "<br/><br/>Goldstore hesabınız başarıyla oluşturulmuştur. Hesabınız aktive etmek için aşağıdaki linke tıklayınız" +
                    " <br/><br/><a href='" + link + "'>" + link + "</a> ";
                var smtp = new SmtpClient
                {
                    Host = network.Network.Host,
                    Port = network.Network.Port,
                    EnableSsl = network.Network.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = network.Network.DefaultCredentials,
                    Credentials = new NetworkCredential(network.Network.UserName, network.Network.Password)
                };
                using (var message = new MailMessage(fromEmail, toEmail)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                    smtp.Send(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}