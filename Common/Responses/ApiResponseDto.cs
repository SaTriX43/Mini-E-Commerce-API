namespace Mini_E_Commerce_API.Common.Responses
{
    public class ApiResponseDto<T>
    {
        public bool Success { get; set; }
        public T? Value { get; set; }
        public ApiErrorDto? Error { get; set; }
    }

    public class ApiErrorDto
    {
        public string Code { get; set; } = default!;
        public string Message { get; set; } = default!;
    }
}
