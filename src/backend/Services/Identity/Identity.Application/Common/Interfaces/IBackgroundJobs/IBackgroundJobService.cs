using Identity.Domain.Entities;

namespace Identity.Application.Common.Interfaces.IBackgroundJobs
{
    public interface IBackgroundJobService
    {
        string ScheduleDeletePendingUser(
            User user,
            TimeSpan delay);

        bool DeleteJob(string jobId);
    }
}
