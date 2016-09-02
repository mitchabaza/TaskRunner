using System;

namespace ScheduledTaskRunner
{
    public class ScheduledTask
    {
        public int OrgId { get; }
        public string ConnectionString { get; }
        public DateTime CreatedAtUtc { get; }
        public string Type { get; private set; }
        public ScheduledTask(int orgId, string connectionString, DateTime createdAtUtc, string type)
        {
            OrgId = orgId;
            ConnectionString = connectionString;
            CreatedAtUtc = createdAtUtc;
            Type = type;
        }


        public override string ToString()
        {
            return $"OrgId: {OrgId}, ConnectionString: {ConnectionString}, CreatedAtUtc: {CreatedAtUtc}";
        }
    }
}