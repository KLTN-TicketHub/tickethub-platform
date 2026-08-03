namespace Notification.Common.Dtos.Notifications
{
    public class NotificationStatsDto
    {
        public DateTime FromUtc { get; set; }

        public DateTime ToUtc { get; set; }

        public int TotalSent { get; set; }

        public int DirectSent { get; set; }

        public int DirectRead { get; set; }

        public double DirectReadRate { get; set; }

        public int BroadcastSent { get; set; }

        public int BroadcastReadTotal { get; set; }

        public int DistinctReaders { get; set; }

        public List<NotificationTypeStatsDto> ByType { get; set; } = new();

        public List<NotificationDailyStatsDto> Daily { get; set; } = new();
    }

    public class NotificationTypeStatsDto
    {
        public string Type { get; set; } = string.Empty;

        public int Sent { get; set; }

        public int ReadCount { get; set; }
    }

    public class NotificationDailyStatsDto
    {
        public DateTime Date { get; set; }

        public int Sent { get; set; }

        public int ReadCount { get; set; }
    }
}
