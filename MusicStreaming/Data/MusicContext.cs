using MusicStreaming.Models;
using Microsoft.EntityFrameworkCore;

namespace MusicStreaming.Data
{
    public class MusicContext : DbContext
    {
        public MusicContext(DbContextOptions<MusicContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistSong> PlaylistSongs { get; set; }
        public DbSet<SavedSong> SavedSongs { get; set; }
        public DbSet<SavedPlaylist> SavedPlaylists { get; set; }
        public DbSet<UserFollow> UserFollows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User relationships
            modelBuilder.Entity<User>()
                .HasMany(u => u.Songs)
                .WithOne(s => s.Artist)
                .HasForeignKey(s => s.ArtistId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Playlists)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure PlaylistSong relationships
            modelBuilder.Entity<PlaylistSong>()
                .HasOne(ps => ps.Playlist)
                .WithMany(p => p.PlaylistSongs)
                .HasForeignKey(ps => ps.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlaylistSong>()
                .HasOne(ps => ps.Song)
                .WithMany(s => s.PlaylistSongs)
                .HasForeignKey(ps => ps.SongId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure SavedSong relationships
            modelBuilder.Entity<SavedSong>()
                .HasOne(ss => ss.User)
                .WithMany(u => u.SavedSongs)
                .HasForeignKey(ss => ss.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SavedSong>()
                .HasOne(ss => ss.Song)
                .WithMany(s => s.SavedSongs)
                .HasForeignKey(ss => ss.SongId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure SavedPlaylist relationships
            modelBuilder.Entity<SavedPlaylist>()
                .HasOne(sp => sp.User)
                .WithMany(u => u.SavedPlaylists)
                .HasForeignKey(sp => sp.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SavedPlaylist>()
                .HasOne(sp => sp.Playlist)
                .WithMany(p => p.SavedPlaylists)
                .HasForeignKey(sp => sp.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure UserFollow relationships
            modelBuilder.Entity<UserFollow>()
                .HasOne(uf => uf.Follower)
                .WithMany(u => u.Following)
                .HasForeignKey(uf => uf.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserFollow>()
                .HasOne(uf => uf.Following)
                .WithMany(u => u.Followers)
                .HasForeignKey(uf => uf.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);

            // Add unique constraint on UserFollow to prevent duplicate follows
            modelBuilder.Entity<UserFollow>()
                .HasIndex(uf => new { uf.FollowerId, uf.FollowingId })
                .IsUnique();
        }
    }
}
