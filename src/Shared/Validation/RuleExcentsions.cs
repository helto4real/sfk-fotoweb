using FluentValidation;
using FluentValidation.Validators;

namespace Shared.Validation;

public static class RuleExcentsions
{
//     public static IRuleBuilderOptions<T, string> Matches<T>(this IRuleBuilder<T, string> ruleBuilder,
// #if NET7_0_OR_GREATER
//         [StringSyntax(StringSyntaxAttribute.Regex)]
// #endif
//         string expression)

//     public static IRuleBuilderOptions<T, string> EmailAddress<T>(this IRuleBuilder<T, string> ruleBuilder, EmailValidationMode mode = EmailValidationMode.AspNetCoreCompatible) {
// #pragma warning disable 618
//         var validator = mode == EmailValidationMode.AspNetCoreCompatible ? new AspNetCoreCompatibleEmailValidator<T>() : (PropertyValidator<T,string>)new EmailValidator<T>();
// #pragma warning restore 618
//         return ruleBuilder.SetValidator(validator);
//     }
    public static IRuleBuilderOptions<T, string> AppPhoneNumber<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Matches(@"^((07|01)\d-?\d{7})|((0\d{1,3})-?\d{5,8})$")
            .WithMessage("Giltiga telefonnummer är bara siffror eller med ett streck efter riktnummer.");
    }  
    
    public static IRuleBuilderOptions<T, string?> AppPassword<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage("Lösenord måste anges.")
            .MaximumLength(32)
            .MinimumLength(6)
            .WithMessage("Lösenordet måste vara mellan 6 och 32 tecken.")
            .Matches("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z])(?=.*[^a-zA-Z\\d]).*$")
            .WithMessage("Lösenordet måste innehålla små och stora bokstäver, siffror samt specialtecken.");
    }
}