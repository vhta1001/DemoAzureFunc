using System;

namespace DemoFunc.Models
{
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public record FeedEtlResult(FeedETLStatus Status, string? ErrorMessage = null, ExceptionDetails? Exception = null)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    {
        public static FeedEtlResult Success { get; } = new(FeedETLStatus.Success);

        public static FeedEtlResult FailureOnLoad(Exception e) => new(FeedETLStatus.FailureOnLoad, e.Message, new ExceptionDetails(e));
        public static FeedEtlResult FailureOnLoad(string errMsg) => new(FeedETLStatus.FailureOnLoad, errMsg);

        public static FeedEtlResult FailureOnExport(Exception e) => new(FeedETLStatus.FailureOnExport, e.Message, new ExceptionDetails(e));
        public static FeedEtlResult FailureOnExport(string errMsg) => new(FeedETLStatus.FailureOnExport, errMsg);

        public static FeedEtlResult FailureOnTransform(Exception e) => new(FeedETLStatus.FailureOnTransform, e.Message, new ExceptionDetails(e));
        public static FeedEtlResult FailureOnTransform(string errMsg) => new(FeedETLStatus.FailureOnTransform, errMsg);
    }
}
