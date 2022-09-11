# BlogEngine: a Testcontainers sample app

## Intro

This project illustrates the use of [Testcontainers](https://github.com/testcontainers/testcontainers-dotnet) for DAL integration testing in `dotnet`.

It will be used as the demo for the zipco "Lunch and Learn" held on 14th September 2022.

## Requirements

- dotnet-sdk >= 6.0.400
- Docker

## Setup

This sample project uses EF Core 6 with PostgreSQL provider.

Run the following command to start a locally running PostgreSQL container using Docker. 

```sh
docker run --name blog-engine-postgres \
    -p 5432:5432 \
    -e POSTGRES_PASSWORD=mysecretpassword \
    -d postgres
```

If you change the `POSTGRES_PASSWORD` you will need to update the connection string in the application configuration as well.

Next, you will need to run the EF migrations to create the database schema in the above PostgreSQL instance.

Install or update the [EF Migrations](https://docs.microsoft.com/en-us/ef/core/cli/dotnet) CLI tool if you haven't already:

```sh
// install
dotnet tool install --global dotnet-ef

// ...or update
dotnet tool update --global dotnet-ef
```

Finally, apply the migrations to your database:

```sh
dotnet ef database update
```

## Run

Run in VS Code with the provided launch configuration.

Otherwise, run in console:

```sh
dotnet run --project src/BlogApp.Api
```

Swagger UI is available at `/swagger` endpoint