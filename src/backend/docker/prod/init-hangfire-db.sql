IF DB_ID(N'TicketHub.Identity.Hangfire.Db') IS NULL
BEGIN
    CREATE DATABASE [TicketHub.Identity.Hangfire.Db];
END

IF DB_ID(N'TicketHub.AI.Hangfire.Db') IS NULL
BEGIN
    CREATE DATABASE [TicketHub.AI.Hangfire.Db];
END
