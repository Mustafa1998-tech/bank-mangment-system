namespace BankManagement.API.DTOs
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
        public int StatusCode { get; set; } = 200;

        public static ServiceResult<T> Success(T data, string message = "")
        {
            return new ServiceResult<T>
            {
                IsSuccess = true,
                Data = data,
                Message = message,
                StatusCode = 200
            };
        }

        public static ServiceResult<T> Failure(string message, int statusCode = 400)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                Message = message,
                StatusCode = statusCode
            };
        }

        public static ServiceResult<T> Failure(List<string> errors, string message = "", int statusCode = 400)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                Message = message,
                Errors = errors,
                StatusCode = statusCode
            };
        }
    }

    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? TraceId { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }

    public class ValidationError
    {
        public string Field { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class PaginationDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "Id";
        public string SortDirection { get; set; } = "asc";

        public void ValidateAndCorrect()
        {
            if (Page < 1) Page = 1;
            if (PageSize < 1) PageSize = 10;
            if (PageSize > 100) PageSize = 100;
            
            SortDirection = SortDirection.ToLower() == "desc" ? "desc" : "asc";
        }
    }

    public class DateRangeDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool IsValid()
        {
            if (!StartDate.HasValue || !EndDate.HasValue)
                return true;

            return StartDate.Value <= EndDate.Value;
        }

        public void SetDefaults()
        {
            if (!StartDate.HasValue)
                StartDate = DateTime.UtcNow.AddMonths(-1);

            if (!EndDate.HasValue)
                EndDate = DateTime.UtcNow;
        }
    }
}

