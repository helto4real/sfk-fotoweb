using Foto.WebServer.Dto;

namespace Foto.WebServer.Shared;

public class ResponseHandler
{
    public string? Message { get; set; }
    public bool HasAlert => Message is not null;

    public bool CheckResponse(ErrorDetail? result)
    {
        if (result is null)
        {
            Clear();
            return true;
        }

        Message = result.Detail;
        return false;
    }

    public T? CheckResponse<T>((T?, ErrorDetail?) result) where T : class
    {
        if (result.Item1 is not null && result.Item2 is null)
        {
           Clear();
           return result.Item1;
        }

        if (result.Item1 is null && result.Item2 is null)
        {
            Message = "Okänt fel";
        }
        else if (result.Item1 is not null && result.Item2 is not null)
        {
            Message = "Okänt fel, både resultat och felmeddelande är satta";
        }
        else if (result.Item2 is not null)
        {
            Message = result.Item2.Detail;
        }

        return null;
    }

    private void Clear() => Message = null;

}