namespace DemoFunc.Models
{
    /// <summary>
    /// Statuses for the ETL process
    /// </summary>
    public enum FeedETLStatus
    {
        /// <summary>
        /// The ETL succeeded without any error
        /// </summary>
        Success,

        /// <summary>
        /// The ETL failed to load the necessary data
        /// </summary>
        FailureOnLoad,

        /// <summary>
        /// The ETL failed to complete the export
        /// </summary>
        FailureOnExport,

        /// <summary>
        /// The ETL failed on transforming the data
        /// </summary>
        FailureOnTransform,
    }
}
