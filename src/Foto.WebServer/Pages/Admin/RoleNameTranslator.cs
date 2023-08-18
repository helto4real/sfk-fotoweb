namespace Foto.WebServer.Pages.Admin;

public static class RoleNameTranslator
{
    public static string GetRoleName(string role) =>
        role switch
        {
            "Admin" => "Administratör",
            "Member" => "Medlem",
            "StbildAdministrator" => "ST-Administratör",
            "CompetitionAdministrator" => "Tävlingsadministratör",
            _ => "Okänd roll"
        };
}