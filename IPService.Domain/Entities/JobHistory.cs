namespace IPService.Domain.Entities
{
    public class JobHistory
    {
        public long Id { get; }// auto-increment PK (BIGINT)
        public DateTime StartedAt { get; }
        public DateTime FinishedAt { get; }
        public string Status { get; } = string.Empty;// NVARCHAR(20): "Success"/"Failed"
        public int ProcessedRecords { get; }
        public int UpdatedRecords { get; }// = Changed from handler
        public int FailedRecords { get; }// = Failed

        public JobHistory(
            DateTime startedAt,
            DateTime finishedAt,
            string status,
            int processedRecords,
            int updatedRecords,
            int failedRecords)
        {
            StartedAt = startedAt;
            FinishedAt = finishedAt;
            Status = status;
            ProcessedRecords = processedRecords;
            UpdatedRecords = updatedRecords;
            FailedRecords = failedRecords;
        }

        public JobHistory() { }
    }
}