using Microsoft.AspNetCore.Components.Forms;

namespace Foto.Web.Client.Shared;

public class CvEditForm : EditForm
{
    protected override void OnParametersSet()
    {
        base.OnParametersSet(); // This is important!   
        EditContext?.SetFieldCssClassProvider(new BootstrapValidationFieldClassProvider());
        
    }
}
