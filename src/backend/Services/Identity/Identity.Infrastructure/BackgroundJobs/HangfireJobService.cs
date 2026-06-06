using Hangfire;
using Identity.Application.Common.Interfaces.IBackgroundJobs;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.BackgroundJobs
{
    public class HangfireJobService : IBackgroundJobService
    {
        public string ScheduleDeletePendingUser(
            User user,
            TimeSpan delay)
        {
            return BackgroundJob.Schedule<UserManager<User>>(
                x => x.DeleteAsync(user),
                delay);
        }

        public bool DeleteJob(string jobId)
        {
            return BackgroundJob.Delete(jobId);
        }
    }
}
