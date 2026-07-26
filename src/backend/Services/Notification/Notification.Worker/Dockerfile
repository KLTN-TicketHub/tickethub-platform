FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "Services/Notification/Notification.Worker/Notification.Worker.csproj"
RUN dotnet publish "Services/Notification/Notification.Worker/Notification.Worker.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Notification.Worker.dll"]
