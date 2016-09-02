namespace ScheduledTaskRunner

{
    public class TaskRunResult
    {
        public TaskRunResult(bool success)
        {
            Success = success;
        }

        public bool Success { get; private set; }
    }
}