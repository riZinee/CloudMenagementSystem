using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence.Configurations
{
    public static class StoredProcedureConfiguration
    {
        public static void EnsureStoredProceduresCreated(AppDbContext context)
        {
            var databaseCreator = context.Database.GetService<IRelationalDatabaseCreator>();
            if (databaseCreator.Exists())
            {
                context.Database.ExecuteSqlRaw(@"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'AddPermissionsToSubStorages')
                    BEGIN
                        EXEC('
                            CREATE PROCEDURE AddPermissionsToSubStorages
                                @UserId UNIQUEIDENTIFIER,
                                @StorageId UNIQUEIDENTIFIER,
                                @PermissionValues NVARCHAR(MAX)
                            AS
                            BEGIN
                                SET NOCOUNT ON;

                                DECLARE @SubStorage TABLE (Id UNIQUEIDENTIFIER);

                                WITH SubStorageHierarchy AS (
                                    SELECT Id FROM StorageMetadata WHERE Id = @StorageId
                                    UNION ALL
                                    SELECT sm.Id
                                    FROM StorageMetadata sm
                                    INNER JOIN SubStorageHierarchy sh ON sm.ParentDirectoryId = sh.Id
                                )
                                INSERT INTO @SubStorage
                                SELECT Id FROM SubStorageHierarchy;

                                DECLARE @CurrentStorageId UNIQUEIDENTIFIER;

                                DECLARE sub_cursor CURSOR FOR 
                                SELECT Id FROM @SubStorage;

                                OPEN sub_cursor;
                                FETCH NEXT FROM sub_cursor INTO @CurrentStorageId;

                                WHILE @@FETCH_STATUS = 0
                                BEGIN
                                    IF NOT EXISTS (
                                        SELECT 1 FROM Permissions 
                                        WHERE UserId = @UserId 
                                        AND StorageId = @CurrentStorageId
                                        AND [Values] = @PermissionValues
                                    )
                                    BEGIN
                                        INSERT INTO Permissions (Id, UserId, StorageId, [Values])
                                        VALUES (NEWID(), @UserId, @CurrentStorageId, @PermissionValues);
                                    END

                                    FETCH NEXT FROM sub_cursor INTO @CurrentStorageId;
                                END;

                                CLOSE sub_cursor;
                                DEALLOCATE sub_cursor;
                            END;
                        ');
                    END
                ");

                context.Database.ExecuteSqlRaw(@"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'UpdatePermissionsForSubStorages')
                    BEGIN
                        EXEC('
                            CREATE PROCEDURE UpdatePermissionsForSubStorages
                                @UserId UNIQUEIDENTIFIER,
                                @StorageId UNIQUEIDENTIFIER,
                                @PermissionValues NVARCHAR(MAX)
                            AS
                            BEGIN
                                SET NOCOUNT ON;

                                DECLARE @SubStorage TABLE (Id UNIQUEIDENTIFIER);

                                -- Znalezienie wszystkich podfolderów rekurencyjnie
                                WITH SubStorageHierarchy AS (
                                    SELECT Id FROM StorageMetadata WHERE Id = @StorageId
                                    UNION ALL
                                    SELECT sm.Id
                                    FROM StorageMetadata sm
                                    INNER JOIN SubStorageHierarchy sh ON sm.ParentDirectoryId = sh.Id
                                )
                                INSERT INTO @SubStorage
                                SELECT Id FROM SubStorageHierarchy;

                                -- Iteracja po znalezionych podfolderach
                                DECLARE @CurrentStorageId UNIQUEIDENTIFIER;

                                DECLARE sub_cursor CURSOR FOR 
                                SELECT Id FROM @SubStorage;

                                OPEN sub_cursor;
                                FETCH NEXT FROM sub_cursor INTO @CurrentStorageId;

                                WHILE @@FETCH_STATUS = 0
                                BEGIN
                                    -- Jeśli uprawnienie istnieje, aktualizujemy
                                    IF EXISTS (
                                        SELECT 1 FROM Permissions 
                                        WHERE UserId = @UserId 
                                        AND StorageId = @CurrentStorageId
                                    )
                                    BEGIN
                                        UPDATE Permissions
                                        SET [Values] = @PermissionValues
                                        WHERE UserId = @UserId 
                                        AND StorageId = @CurrentStorageId;
                                    END

                                    FETCH NEXT FROM sub_cursor INTO @CurrentStorageId;
                                END;

                                CLOSE sub_cursor;
                                DEALLOCATE sub_cursor;
                            END;
                        ');
                    END
                ");
            }
        }
    }
}
