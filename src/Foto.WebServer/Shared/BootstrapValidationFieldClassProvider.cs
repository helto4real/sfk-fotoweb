using Microsoft.AspNetCore.Components.Forms;

namespace Foto.WebServer.Shared;

/// <summary>
///     This class is used to add the "is-invalid" class to invalid fields in Bootstrap.
/// </summary>
public class BootstrapValidationFieldClassProvider : FieldCssClassProvider
{
    public override string GetFieldCssClass(EditContext editContext,
        in FieldIdentifier fieldIdentifier)
    {
        var isValid = !editContext.GetValidationMessages(fieldIdentifier).Any();
        return isValid ? string.Empty : "is-invalid";
    }
}