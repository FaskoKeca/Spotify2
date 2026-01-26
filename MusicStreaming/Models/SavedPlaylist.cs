namespace MusicStreaming.Models
{
    public class SavedPlaylist
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PlaylistId { get; set; }
        public DateTime SavedAt { get; set; } = DateTime.Now;

        // Foreign keys
        public User? User { get; set; }
        public Playlist? Playlist { get; set; }
    }
}
