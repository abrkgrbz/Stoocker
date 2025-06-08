using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.DTOs.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string? ErrorMessage { get; }
        public string? ErrorCode { get; }
        public Dictionary<string, string[]>? ValidationErrors { get; }

        protected Result(bool isSuccess, string? errorMessage = null, string? errorCode = null, Dictionary<string, string[]>? validationErrors = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
            ValidationErrors = validationErrors;
        }

        public static Result Success() => new(true);

        public static Result Failure(string errorMessage, string? errorCode = null)
            => new(false, errorMessage, errorCode);

        public static Result ValidationFailure(Dictionary<string, string[]> errors)
            => new(false, "Validation failed", "VALIDATION_ERROR", errors);
    }

    public class Result<T> : Result
    {
        public T? Data { get; }

        protected Result(bool isSuccess, T? data, string? errorMessage = null, string? errorCode = null, Dictionary<string, string[]>? validationErrors = null)
            : base(isSuccess, errorMessage, errorCode, validationErrors)
        {
            Data = data;
        }

        public static Result<T> Success(T data) => new(true, data);

        public static new Result<T> Failure(string errorMessage, string? errorCode = null)
            => new(false, default, errorMessage, errorCode);

        public static new Result<T> ValidationFailure(Dictionary<string, string[]> errors)
            => new(false, default, "Validation failed", "VALIDATION_ERROR", errors);
    }
}
