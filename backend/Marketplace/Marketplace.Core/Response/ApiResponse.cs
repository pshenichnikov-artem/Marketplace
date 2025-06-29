using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Core.Response
{
    public class ApiResponse
    {
        public string Status { get; set; }
        public object? Data { get; set; }
        public string Message { get; set; }
        public ApiError? Error { get; set; } = null;

        public static ApiResponse CreateFailure(string errorMessage = "Validate error", int statusCode = 400, Dictionary<string,string>? details = null)
        {
            return new ApiResponse() { Status = "error", Data = null, Message = errorMessage, Error = new ApiError(statusCode, details)  };
        }

        public static ApiResponse CreateSuccess(object data, string message = "Operation success")
        {
            return new ApiResponse() { Status = "success", Data = data, Message = message };
        }

        public class ApiError
        {
            public int Code { get; set; }
            public Dictionary<string, string>? Details { get; set; }

            public ApiError(int code, Dictionary<string, string>? details)
            {
                Code = code;
                Details = details;
            }
        }
    }
}
