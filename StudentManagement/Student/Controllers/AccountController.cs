using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Student.Models;
using Student.ViewModels;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Student.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController( UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager )
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Logout( )
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction( "Index", "Home" );
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register( )
        {
            return View();
        }

        [AcceptVerbs( "Get", "Post" )]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse( string email )
        {
            var user = await _userManager.FindByEmailAsync( email );

            if(user == null)
            {
                return Json( true );
            }
            else
            {
                return Json( $"Email{email} is already in use" );
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register( RegisterViewModel model )
        {
            if(ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    City = model.City
                };
                var result = await _userManager.CreateAsync( user, model.Password );

                if(result.Succeeded)
                {
                    if(_signInManager.IsSignedIn( User ) && User.IsInRole( "Admin" ))
                    {
                        return RedirectToAction( "ListUsers", "Administration" );
                    }

                    await _signInManager.SignInAsync( user, isPersistent: false );
                    return RedirectToAction( "Index", "Home" );
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError( "", error.Description );
                }
            }

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login( string returnUrl )
        {
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View( model );
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login( LoginViewModel model, string returnUrl )
        {
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync( model.Email );

                if(user != null && !user.EmailConfirmed &&
                    (await _userManager.CheckPasswordAsync( user, model.Password )))
                {
                    ModelState.AddModelError( string.Empty, "Email not confirmed yet" );
                    return View( model );
                }

                var result = await _signInManager.PasswordSignInAsync( model.Email, model.Password,
                    model.RememberMe, false );

                if(result.Succeeded)
                {
                    if(!string.IsNullOrEmpty( returnUrl ) && Url.IsLocalUrl( returnUrl ))
                    {
                        return Redirect( returnUrl );
                    }
                    else
                    {
                        return RedirectToAction( "Index", "Home" );
                    }
                }
                ModelState.AddModelError( string.Empty, "Invalid Login Attempt" );
            }

            return View( model );
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin( string provider, string returnUrl )
        {
            var redirectUrl = Url.Action( "ExternalLoginCallBack", "Account",
                new { ReturnUrl = returnUrl } );

            var properties = _signInManager.ConfigureExternalAuthenticationProperties( provider, redirectUrl );
            return new ChallengeResult( provider, properties );
        }

        [AllowAnonymous]
        public async Task<IActionResult>
            ExternalLoginCallBack( string returnUrl = null, string remoteError = null )
        {
            returnUrl ??= Url.Content( "~/" );

            LoginViewModel loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if(remoteError != null)
            {
                ModelState.AddModelError( string.Empty, $"Error from external provider: {remoteError}" );

                return View( "Login", loginViewModel );
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if(info == null)
            {
                ModelState.AddModelError( string.Empty, "Error loading external login information." );

                return View( "Login", loginViewModel );
            }

            var email = info.Principal.FindFirstValue( ClaimTypes.Email );
            ApplicationUser user = null;

            if(email != null)
            {
                user = await _userManager.FindByEmailAsync( email );

                if(user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError( string.Empty, "Email not confirmed yet" );
                    return View( "Login", loginViewModel );
                }
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync( info.LoginProvider, info.ProviderKey,
                isPersistent: false, bypassTwoFactor: true );

            if(signInResult.Succeeded)
            {
                return LocalRedirect( returnUrl );
            }
            else
            {
                if(email != null)
                {
                    if(user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue( ClaimTypes.Email ),
                            Email = info.Principal.FindFirstValue( ClaimTypes.Email )
                        };

                        await _userManager.CreateAsync( user );
                    }

                    await _userManager.AddLoginAsync( user, info );
                    await _signInManager.SignInAsync( user, isPersistent: false );

                    return LocalRedirect( returnUrl );
                }

                ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact support on Student@Munchintech.com";

                return View( "Error" );
            }
        }
    }
}
