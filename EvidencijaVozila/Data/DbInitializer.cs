using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EvidencijaVozila.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (await context.Database.CanConnectAsync())
        {
            await EnsureLegacyCompatibilityAsync(context);
            await BaselineLegacyDatabaseAsync(context);
        }

        await context.Database.MigrateAsync();
    }

    private static async Task EnsureLegacyCompatibilityAsync(ApplicationDbContext context)
    {
        await context.Database.ExecuteSqlRawAsync("""
            IF OBJECT_ID(N'[Users]', N'U') IS NOT NULL AND COL_LENGTH('Users', 'ContactPhone') IS NULL
            BEGIN
                ALTER TABLE [Users] ADD [ContactPhone] nvarchar(30) NULL;
            END
            """);

        await context.Database.ExecuteSqlRawAsync("""
            IF OBJECT_ID(N'[Vehicles]', N'U') IS NOT NULL AND COL_LENGTH('Vehicles', 'ServiceIntervalKm') IS NOT NULL
            BEGIN
                ALTER TABLE [Vehicles] DROP COLUMN [ServiceIntervalKm];
            END
            """);

        await context.Database.ExecuteSqlRawAsync("""
            IF OBJECT_ID(N'[Vehicles]', N'U') IS NOT NULL AND COL_LENGTH('Vehicles', 'TireChangeNote') IS NULL
            BEGIN
                ALTER TABLE [Vehicles] ADD [TireChangeNote] nvarchar(100) NULL;
            END
            """);
    }

    private static async Task BaselineLegacyDatabaseAsync(ApplicationDbContext context)
    {
        var hasMigrationHistory = await context.Database.SqlQueryRaw<int>("""
            SELECT CASE
                WHEN OBJECT_ID(N'[__EFMigrationsHistory]', N'U') IS NOT NULL
                THEN 1
                ELSE 0
            END AS [Value]
            """).SingleAsync();

        if (hasMigrationHistory == 1)
        {
            return;
        }

        var hasLegacyTables = await context.Database.SqlQueryRaw<int>("""
            SELECT CASE
                WHEN OBJECT_ID(N'[Users]', N'U') IS NOT NULL
                 AND OBJECT_ID(N'[Vehicles]', N'U') IS NOT NULL
                 AND OBJECT_ID(N'[VehicleOrders]', N'U') IS NOT NULL
                THEN 1
                ELSE 0
            END AS [Value]
            """).SingleAsync();

        if (hasLegacyTables == 0)
        {
            return;
        }

        var firstMigration = context.Database.GetMigrations().FirstOrDefault();
        if (string.IsNullOrWhiteSpace(firstMigration))
        {
            return;
        }

        var productVersion = context.Model.GetProductVersion();

        await context.Database.ExecuteSqlInterpolatedAsync($"""
            IF OBJECT_ID(N'[__EFMigrationsHistory]', N'U') IS NULL
            BEGIN
                CREATE TABLE [__EFMigrationsHistory] (
                    [MigrationId] nvarchar(150) NOT NULL,
                    [ProductVersion] nvarchar(32) NOT NULL,
                    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
                );
            END;

            IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = {firstMigration})
            BEGIN
                INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
                VALUES ({firstMigration}, {productVersion});
            END;
            """);
    }
}
