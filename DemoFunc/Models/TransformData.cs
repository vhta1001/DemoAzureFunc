namespace DemoFunc.Models
{
    /// <summary>
    /// Describes the result of transforming data
    /// </summary>
    public record TransformData(string Contents, FeedETLResult Result)
    {
        public string Contents { get; init; } = Contents;
        public static TransformData Success(string Contents) => new(Contents, FeedETLResult.Success);
    }
}
