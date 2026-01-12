namespace Mini_E_Commerce_API.Common.Errors
{
    public sealed record Error(string Code, string Message, ErrorType Type);
}
