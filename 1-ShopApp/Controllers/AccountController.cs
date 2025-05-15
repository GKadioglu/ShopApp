using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _1_ShopApp.EmailServices;
using _1_ShopApp.Extensions;
using _1_ShopApp.Identity;
using _1_ShopApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shopapp.business.Abstract;
using shopapp.business.Concrete;

namespace _1_ShopApp.Controllers
{
    [AutoValidateAntiforgeryToken] // bu kod ile, tek tek post alanlarına token kontrolü yapmak yerine en başından tek bir yere yapıyoruz. Güvenlik açısından token kontrolü önemlidir.
    public class AccountController: Controller
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private IEmailSender _emailSender;
        private ICartService _cartService;

        public AccountController(ICartService cartService,SignInManager<User> signInManager, UserManager<User> userManager, IEmailSender emailSender)
        {
            _cartService = cartService;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        
        }

        [HttpGet]
        public IActionResult Login(string ReturnUrl=null)
        {
            return View(new LoginModel()
            {
                ReturnUrl = ReturnUrl?? "~/"
            });
        }
       
       [HttpPost]
       [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.UserName);
            if(user == null)
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı!");
                return View(model);
            }

            if(!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "Lütfen mail hesabınıza gelen link ile üyeliğinizi onaylayın.!");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);

            if(result.Succeeded)
            {
                // ReturnUrl yoksa ana sayfaya yönlendirme yapın
                return Redirect(model.ReturnUrl ?? "~/");
            }

            ModelState.AddModelError("", "Kullanıcı adı veya Parola yanlış.");
            return View(model);
        }


        
         public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // token bilgisi için.
         public async Task<IActionResult> Register(RegisterModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email
            };  

            var result = await _userManager.CreateAsync(user,model.Password);
            
            
            if(result.Succeeded)
            {
                // burada token oluşturalım ve mail olarak gönderelim: 
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action("ConfirmEmail","Account",new {
                    userId = user.Id,
                    token = code
                });

                  TempData.Put("message", new AlertMessage()
                {
            Title = "Hesabınız Oluşturuldu",
            Message = "Hesabınızı Onaylamak İçin Lütfen Mail Adresinizi Kontrol Ediniz",
            AlertType = "warning"
             });

                await _emailSender.SendEmailAsync(model.Email, "Hesabınızı onaylayınız.", $"Lütfen email hesabınızı onaylamak için linke <a href='https://localhost:5001{url}'>tıklayınız.</a>");
                return RedirectToAction("Login", "Account");
            }
            ModelState.AddModelError("","Bilinmeyen bir hata meydana geldi, lütfen yeniden deneyiniz.");
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
             TempData.Put("message", new AlertMessage() {
                        Title = "Oturum Kapatıldı.",
                        Message = "Hesabınız Güvenli Bir Şekilde Kapatıldı.",
                        AlertType = "warning"
                    });
            
            return Redirect("~/");
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
                if(userId == null || token == null)
                {
                    TempData.Put("message", new AlertMessage() {
                        Title = "Geçersiz token",
                        Message = "Geçersiz Token.",
                        AlertType = "danger"
                    });
                    return View();
                }
                var user = await _userManager.FindByIdAsync(userId);
                if(user!= null)
                {
                    var result = await _userManager.ConfirmEmailAsync(user,token);
                    if(result.Succeeded)
                    {

                        // cart objesini oluştur.
                        _cartService.InitializeCart(user.Id);
                        TempData.Put("message", new AlertMessage() 
                        {
                        Title = "Hesabınız Onaylandı!",
                        Message = "Hesabınız Onaylandı, Giriş Yapabilirsiniz!",
                        AlertType = "success"
                    });
                        return View();
                    }
                }
                    TempData.Put("message", new AlertMessage() {
                        Title = "Hesabınız Onaylanmadı!",
                        Message = "Hesabınız Onaylanmadı!",
                        AlertType = "warning"
                    });
                return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            if(string.IsNullOrEmpty(Email))
            {
                return View();
            }
            var user = await _userManager.FindByEmailAsync(Email);
            if(user == null)
            {
                return View();
            }
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

             // burada token oluşturalım ve mail olarak gönderelim: 
                var url = Url.Action("ResetPassword","Account",new {
                    userId = user.Id,
                    token = code
                });

                await _emailSender.SendEmailAsync(Email, "Reset Password", $"Parolanızı yenilemek için lütfen linke <a href='https://localhost:5001{url}'>tıklayınız.</a>");

            return View();
        }

        public IActionResult ResetPassword(string userId, string token)
        {
           if(userId == null || token == null)
           {
             return RedirectToAction ("Home", "Index");
           }

           var model = new ResetPasswordModel {Token=token};
           return View();
        }

        [HttpPost]
         public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user == null)
            {
                return RedirectToAction("Home", "Index");   
            }

            var result = await _userManager.ResetPasswordAsync(user,model.Token,model.Password);
            if(result.Succeeded)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(model);
            
        }
   
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}