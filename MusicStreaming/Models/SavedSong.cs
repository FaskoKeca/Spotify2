namespace MusicStreaming.Models
{
    public class SavedSong
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SongId { get; set; }
        public DateTime SavedAt { get; set; } = DateTime.Now;

        // Foreign keys
        public User? User { get; set; }
        public Song? Song { get; set; }
    }
}
