using FluentValidation;
using FluentValidation.Results;
using Foto.WebServer.Authentication;
using Foto.WebServer.Dto;
using Foto.WebServer.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Foto.WebServer.Pages;

public class LogIn : PageModel
{
    private readonly IValidator<LogIn> _validator;
    private readonly IAuthService _authService;

    public LogIn(IValidator<
        LogIn> validator, 
        IAuthService authService, 
        ExternalProviders externalProviders)
    {
        _validator = validator;
        ExternalProviders = externalProviders;
        _authService = authService;
    }
    public ExternalProviders ExternalProviders { get; }

    public bool IsSubmitting { get; set; }
    
    public void OnGet()
    {
        
    }
    [BindProperty]
    public string? UserName { get; set; } = "";
    
    [BindProperty]
    public string? Password { get; set; } = "";

    public bool HasConsentCookie
    {
        get
        {
            Request.Cookies.TryGetValue("ConsentCookie", out var consentCookie);
            return consentCookie is not null;
        }
    }

    public string? ErrorMessage { get; set; }

    public void SocialLoginClickCallback()
    {
        IsSubmitting = true;
    }
    public string GetClassFromProvider(string provider)
    {
        return provider switch
        {
            "Google" => "bi bi-google",
            "Facebook" => "bi bi-facebook",
            "Twitter" => "bi bi-twitter",
            "Microsoft" => "bi bi-microsoft",
            _ => "bi bi-github"
        };
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        ValidationResult result = await _validator.ValidateAsync(this);
        
        if (!result.IsValid) 
        {
            // Copy the validation results into ModelState.
            // ASP.NET uses the ModelState collection to populate 
            // error messages in the View.
            result.AddToModelState(this.ModelState);

            // re-render the view when validation failed.
            return Page();
        }
        
        var (user, error) = await _authService.LoginAsync(new LoginUserInfo(){Username = UserName!, Password = Password!});
            
        if (user is null)
        {
            if (error?.StatusCode == 403)
            {
                ErrorMessage = "Fel användarnamn eller lösenord.";
            }
            else
            {
                ErrorMessage = "Okänt fel, prova igen senare.";
            }
            return Page();
        }
            
        if (error is not null)
        {
            ErrorMessage = "Okänt fel, prova igen senare.";
            return Page();
        }

        var userClaimsPrincipal = new UserClaimPrincipal(user.UserName, user.IsAdmin, user.RefreshToken);
        
        // Since we are successfully authenticated, we redirect the user to the home page.
        userClaimsPrincipal.AuthenticationProperties.RedirectUri = "/";
        
        return SignIn(userClaimsPrincipal, userClaimsPrincipal.AuthenticationProperties,
            CookieAuthenticationDefaults.AuthenticationScheme);
    }
    public class SingInValidator : AbstractValidator<LogIn> 
    {
        public SingInValidator() 
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("Användarnamn måste anges.")
                .MaximumLength(25)
                .WithMessage("Användarnamn får max vara 25 tecken")
                .MinimumLength(5)
                .WithMessage("Användarnamn måste vara minst 5 tecken");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Lösenord måste anges.")
                .MaximumLength(32)
                .MinimumLength(6)
                .WithMessage("Lösenordet måste vara mellan 6 och 32 tecken.")
                .Matches("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z])(?=.*[^a-zA-Z\\d]).*$")
                .WithMessage("Lösenordet måste innehålla små och stora bokstäver, siffror samt specialtecken.");
        }
    }
}
public static class Extensions 
{
    public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState) 
    {
        foreach (var error in result.Errors) 
        {
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
    }
}