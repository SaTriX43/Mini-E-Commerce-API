namespace Mini_E_Commerce_API.DTOs
{
    public class ApiResponseDto<T>
    {
        public bool Success { get; set; }
        public T? Value { get; set; }
        public string? Error { get; set; }
    }
}
