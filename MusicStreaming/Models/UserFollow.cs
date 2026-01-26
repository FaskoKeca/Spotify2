namespace MusicStreaming.Models
{
    public class UserFollow
    {
        public int Id { get; set; }
        public int FollowerId { get; set; }
        public int FollowingId { get; set; }
        public DateTime FollowedAt { get; set; } = DateTime.Now;

        // Foreign keys
        public User? Follower { get; set; }
        public User? Following { get; set; }
    }
}
