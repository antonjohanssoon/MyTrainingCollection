namespace Domain
{
    public class OperationResult<T>
    {
        public bool IsSuccess { get; private set; }
        public string ErrorMessage { get; private set; }
        public T Data { get; private set; }
        public string Message { get; private set; }

        private OperationResult(bool isSuccess, string errorMessage, T data, string message)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            Data = data;
            Message = message;
        }

        public static OperationResult<T> Success(T data, string message = "Operation was successful")
        {
            return new OperationResult<T>(true, null, data, message);
        }

        public static OperationResult<T> Failure(string errorMessage, string message = "Operation failed")
        {
            return new OperationResult<T>(false, errorMessage, default, message);
        }


    }
}
