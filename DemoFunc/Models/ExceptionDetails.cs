using System;

namespace DemoFunc.Models
{
    public record ExceptionDetails
    {
        public ExceptionDetails(Exception e)
        {
            Message = e.Message;
            StackTrace = e.StackTrace;
            Source = e.Source;
            Data = e.Data;
            if (e.InnerException != null)
                InnerException = new ExceptionDetails(e.InnerException);
        }
        public string Message { get; init; }
        public string? StackTrace { get; init; }
        public string? Source { get; init; }
        public System.Collections.IDictionary Data { get; init; }
        public ExceptionDetails? InnerException { get; init; }
    }
}
