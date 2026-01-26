namespace MusicStreaming.Models
{
    public class Song
    {
        public int Id { get; set; }
        public required string SongName { get; set; }
        public int ArtistId { get; set; }
        public required string Producer { get; set; }
        public int SongYear { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign key
        public User? Artist { get; set; }

        // Navigation properties
        public ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();
        public ICollection<SavedSong> SavedSongs { get; set; } = new List<SavedSong>();
    }
}
