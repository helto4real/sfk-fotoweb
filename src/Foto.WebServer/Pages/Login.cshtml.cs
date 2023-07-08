using BlazorStrap.V5;
using BlazorStrap;
using FluentValidation;
using FluentValidation.Results;
using Foto.WebServer.Authentication;
using Foto.WebServer.Dto;
using Foto.WebServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Foto.WebServer.Pages;

public class LogIn : PageModel
{
    private readonly IValidator<LogIn> _validator;
    private readonly IUserService _userService;

    public LogIn(IValidator<
        LogIn> validator, 
        IUserService userService, 
        ExternalProviders externalProviders)
    {
        _validator = validator;
        _userService = userService;
        ExternalProviders = externalProviders;
    }
    // [CascadingParameter]
    // public bool ConsentGiven { get; set; }
    //
    // public BSModal? AcceptCookieModal;
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
        
        var (user, error) = await _userService.LoginAsync(new LoginUserInfo(){Username = UserName!, Password = Password!});
            
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
            
        var externalClaimsPrincipalManager = new UserClaimPrincipal(user.UserName, user.Email, user.IsAdmin);
        if (error is not null)
        {
            ErrorMessage = "Okänt fel, prova igen senare.";
            return Page();
        }

        var authProps =  new AuthenticationProperties();
        var tokens = new[]
        {
            new AuthenticationToken { Name = TokenNames.AccessToken, Value = user.Token },
        };
        authProps.StoreTokens(tokens);
        authProps.RedirectUri = "/";
        return SignIn(externalClaimsPrincipalManager, authProps,
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