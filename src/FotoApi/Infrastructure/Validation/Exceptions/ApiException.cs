namespace FotoApi.Infrastructure.Validation.Exceptions
{
    public abstract class ApiException : Exception
    {
        protected ApiException(string title, string message)
            : base(message) =>
            Title = title;

        public string Title { get; }
    }
}
