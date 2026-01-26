namespace MusicStreaming.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; } // "User", "Artist", "Admin"
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public ICollection<Song> Songs { get; set; } = new List<Song>();
        public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
        public ICollection<SavedSong> SavedSongs { get; set; } = new List<SavedSong>();
        public ICollection<SavedPlaylist> SavedPlaylists { get; set; } = new List<SavedPlaylist>();
        public ICollection<UserFollow> Following { get; set; } = new List<UserFollow>();
        public ICollection<UserFollow> Followers { get; set; } = new List<UserFollow>();
    }
}
