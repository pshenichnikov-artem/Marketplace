namespace Marketplace.Core.Response
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public int StatusCode { get; set; }
      //  public ErrorResponse? Error { get; set; }

        public static ServiceResult<T> Success(T? data = default, int statusCode = 200)
        {
            return new ServiceResult<T> { IsSuccess = true, Data = data, StatusCode = statusCode };
        }

        public static ServiceResult<T> Failure(string errorMessage, int statusCode = 400)
        {
            return new ServiceResult<T> { IsSuccess = false, Data = default, ErrorMessage = errorMessage, StatusCode = statusCode };
        }

        public class ErrorResponse
        {
            public Dictionary<string, string[]>? Details;
            public int StatusCode;

            public ErrorResponse(int statusCode, Dictionary<string, string[]>? details = null)
            {
                StatusCode = statusCode;
                Details = details;
            }
        }
    }
}
