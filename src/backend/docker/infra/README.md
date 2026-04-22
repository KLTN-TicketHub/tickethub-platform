# Infrastructure Docker

This folder contains the local development infrastructure for TicketHub.

## Services

- RabbitMQ with management UI
- Redis

## Run

```bash
docker compose --env-file .env -f docker-compose.yml up -d
```

If you do not want to create a `.env` file yet, the default values in `docker-compose.yml` will be used.

## Useful endpoints

- RabbitMQ management UI: http://localhost:15672
- RabbitMQ AMQP: localhost:5672
- Redis: localhost:6379