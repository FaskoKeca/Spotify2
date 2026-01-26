namespace MusicStreaming.Models
{
    public class Playlist
    {
        public int Id { get; set; }
        public required string PlaylistName { get; set; }
        public required string Description { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign key
        public User? User { get; set; }

        // Navigation properties
        public ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();
        public ICollection<SavedPlaylist> SavedPlaylists { get; set; } = new List<SavedPlaylist>();
    }
}
