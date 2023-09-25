using Ballastagram.Commom;

namespace Ballastagram.Post.Models
{
    public record PostModel
    {
        public ulong Id { get; set; }
        public ulong AuthorId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreationDate { get; set; } 
        public ulong Likes { get; set; }
    }

    public record PostLikeModel
    {
        public ulong PostId { get; set; }
        public ulong UserId { get; set; }
    }

    public record PostLikeCount
    {
        public ulong PostId { get; set; }
        public ulong Count { get; set; }
    }

    #region Keys
    public class PostPK : ModelKey<PostModel>
    {
        public ulong Id { get; set; }
    }

    public class PostAuthorKey : ModelKey<PostModel>
    {
        public ulong AuthorId { get; set; }
    } 
    #endregion
}