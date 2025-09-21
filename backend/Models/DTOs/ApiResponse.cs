namespace OtpEmailSystem.Models.DTOs
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();

        public static ApiResponse SuccessResult(string message = "Success")
        {
            return new ApiResponse { Success = true, Message = message };
        }

        public static ApiResponse ErrorResult(string message, List<string>? errors = null)
        {
            return new ApiResponse 
            { 
                Success = false, 
                Message = message, 
                Errors = errors ?? new List<string>() 
            };
        }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T? Data { get; set; }

        public static ApiResponse<T> SuccessResult(T data, string message = "Success")
        {
            return new ApiResponse<T> { Success = true, Message = message, Data = data };
        }

        public static new ApiResponse<T> ErrorResult(string message, List<string>? errors = null)
        {
            return new ApiResponse<T> 
            { 
                Success = false, 
                Message = message, 
                Errors = errors ?? new List<string>() 
            };
        }
    }
}