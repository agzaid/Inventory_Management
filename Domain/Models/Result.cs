using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Result<T>
    {
        // Indicates whether the operation was successful or not
        public bool IsSuccess { get; set; }

        // A message associated with the result (e.g., success or error message)
        public string Message { get; set; }

        // Contains the actual result/data if successful
        public T Data { get; set; }

        // Optional: Error code, useful when you want to return error codes
        public string ErrorCode { get; set; }

        // Optional: Any additional metadata or extra information
        public object Metadata { get; set; }

        // Constructor for success result
        public Result(T data, string message = null)
        {
            IsSuccess = true;
            Data = data;
            Message = message;
        }

        // Constructor for failure result
        public Result(string message, string errorCode = null, object metadata = null)
        {
            IsSuccess = false;
            Message = message;
            ErrorCode = errorCode;
            Metadata = metadata;
        }

        // Factory method for success result
        public static Result<T> Success(T data, string message = null)
        {
            return new Result<T>(data, message);
        }

        // Factory method for failure result
        public static Result<T> Failure(string message, string errorCode = null, object metadata = null)
        {
            return new Result<T>(message, errorCode, metadata);
        }
    }

}
