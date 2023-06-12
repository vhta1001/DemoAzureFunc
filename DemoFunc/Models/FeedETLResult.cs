using System;

namespace DemoFunc.Models
{
    public record FeedETLResult(FeedETLStatus Status, string? ErrorMessage = null, ExceptionDetails? Exception = null)
    {
        public static FeedETLResult Success { get; } = new(FeedETLStatus.Success);

        public static FeedETLResult FailureOnLoad(Exception e) => new(FeedETLStatus.FailureOnLoad, e.Message, new ExceptionDetails(e));
        public static FeedETLResult FailureOnLoad(string errMsg) => new(FeedETLStatus.FailureOnLoad, errMsg);

        public static FeedETLResult FailureOnExport(Exception e) => new(FeedETLStatus.FailureOnExport, e.Message, new ExceptionDetails(e));
        public static FeedETLResult FailureOnExport(string errMsg) => new(FeedETLStatus.FailureOnExport, errMsg);

        public static FeedETLResult FailureOnTransform(Exception e) => new(FeedETLStatus.FailureOnTransform, e.Message, new ExceptionDetails(e));
        public static FeedETLResult FailureOnTransform(string errMsg) => new(FeedETLStatus.FailureOnTransform, errMsg);
    }
}
