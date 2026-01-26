namespace MusicStreaming.Models
{
    public class PlaylistSong
    {
        public int Id { get; set; }
        public int PlaylistId { get; set; }
        public int SongId { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.Now;

        // Foreign keys
        public Playlist? Playlist { get; set; }
        public Song? Song { get; set; }
    }
}
