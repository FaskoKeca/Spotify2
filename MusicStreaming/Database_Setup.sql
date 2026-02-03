-- Music Streaming Platform Database Setup
-- This database will be created automatically by Entity Framework when the application first runs

-- If you need to reset the database or use an existing SQL Server database:
-- 1. The application will run migrations automatically on startup
-- 2. The connection string in appsettings.json is: "Server=(localdb)\\mssqllocaldb;Database=MusicStreaming;Trusted_Connection=true;"

-- If using a different SQL Server instance, update the connection string and the migrations will apply these tables:

-- Users table
CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Username] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

-- Songs table
CREATE TABLE [Songs] (
    [Id] int NOT NULL IDENTITY,
    [SongName] nvarchar(max) NOT NULL,
    [ArtistId] int NOT NULL,
    [Producer] nvarchar(max) NOT NULL,
    [SongYear] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Songs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Songs_Users_ArtistId] FOREIGN KEY ([ArtistId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

-- Playlists table
CREATE TABLE [Playlists] (
    [Id] int NOT NULL IDENTITY,
    [PlaylistName] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [UserId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Playlists] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Playlists_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

-- PlaylistSongs junction table
CREATE TABLE [PlaylistSongs] (
    [Id] int NOT NULL IDENTITY,
    [PlaylistId] int NOT NULL,
    [SongId] int NOT NULL,
    [AddedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_PlaylistSongs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PlaylistSongs_Playlists_PlaylistId] FOREIGN KEY ([PlaylistId]) REFERENCES [Playlists] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PlaylistSongs_Songs_SongId] FOREIGN KEY ([SongId]) REFERENCES [Songs] ([Id]) ON DELETE CASCADE
);

-- SavedSongs table (user library)
CREATE TABLE [SavedSongs] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [SongId] int NOT NULL,
    [SavedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_SavedSongs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SavedSongs_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_SavedSongs_Songs_SongId] FOREIGN KEY ([SongId]) REFERENCES [Songs] ([Id]) ON DELETE CASCADE
);

-- SavedPlaylists table (user library)
CREATE TABLE [SavedPlaylists] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [PlaylistId] int NOT NULL,
    [SavedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_SavedPlaylists] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SavedPlaylists_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_SavedPlaylists_Playlists_PlaylistId] FOREIGN KEY ([PlaylistId]) REFERENCES [Playlists] ([Id]) ON DELETE CASCADE
);

-- UserFollows table (social following)
CREATE TABLE [UserFollows] (
    [Id] int NOT NULL IDENTITY,
    [FollowerId] int NOT NULL,
    [FollowingId] int NOT NULL,
    [FollowedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_UserFollows] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserFollows_Users_FollowerId] FOREIGN KEY ([FollowerId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_UserFollows_Users_FollowingId] FOREIGN KEY ([FollowingId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [UQ_UserFollows_Unique] UNIQUE ([FollowerId], [FollowingId])
);

-- Create indexes for better query performance
CREATE INDEX [IX_Songs_ArtistId] ON [Songs] ([ArtistId]);
CREATE INDEX [IX_Playlists_UserId] ON [Playlists] ([UserId]);
CREATE INDEX [IX_PlaylistSongs_PlaylistId] ON [PlaylistSongs] ([PlaylistId]);
CREATE INDEX [IX_PlaylistSongs_SongId] ON [PlaylistSongs] ([SongId]);
CREATE INDEX [IX_SavedSongs_UserId] ON [SavedSongs] ([UserId]);
CREATE INDEX [IX_SavedSongs_SongId] ON [SavedSongs] ([SongId]);
CREATE INDEX [IX_SavedPlaylists_UserId] ON [SavedPlaylists] ([UserId]);
CREATE INDEX [IX_SavedPlaylists_PlaylistId] ON [SavedPlaylists] ([PlaylistId]);
CREATE INDEX [IX_UserFollows_FollowerId] ON [UserFollows] ([FollowerId]);
CREATE INDEX [IX_UserFollows_FollowingId] ON [UserFollows] ([FollowingId]);
