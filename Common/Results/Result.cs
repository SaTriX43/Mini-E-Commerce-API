using Mini_E_Commerce_API.Common.Errors;

namespace Mini_E_Commerce_API.Common.Results
{
    public class Result
    {
        public bool IsSuccess { get; }
        public  Error? Error { get; }

        protected Result(bool isSuccess, Error? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success()
            => new(true, null);

        public static Result Failure(Error error)
            => new(false, error);

        public static Result Failure(string message)
           => new(false, new Error("General.Error", message, ErrorType.Validation));
    }

    public class Result<T> : Result
    {
        public T? Value { get; }

        protected Result(bool isSuccess, T? value, Error? error)
            : base(isSuccess, error)
        {
            Value = value;
        }

        public static Result<T> Success(T value)
            => new(true, value, null);

        public static Result<T> Failure(Error error)
            => new(false, default, error);
        public static Result<T> Failure(string message)
            => new(false, default, new Error("General.Error", message, ErrorType.Validation));

    }


}
