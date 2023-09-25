using Ballastagram.Commom;

namespace Ballastagram.Post.Models
{
    public record CommentModel
    {
        public ulong Id { get; set; }
        public ulong AuthorId { get; set; }
        public ulong PostId { get; set; }
        public string Content { get; set; }
        public ulong Likes { get; set; }
    }

    public record CommentLikeModel
    {
        public ulong CommentId { get; set; }
        public ulong UserId { get; set; }
    }

    public record CommentLikeCount
    {
        public ulong CommentId { get; set; }
        public ulong Count { get; set; }
    }
    #region Keys
    public class CommentPK : ModelKey<CommentModel>
    {
        public ulong Id { get; set; }
    }

    public class CommentAuthorKey : ModelKey<CommentModel>
    {
        public ulong AuthorId { get; set; }
    }

    public class PostKey : ModelKey<CommentModel>
    {
        public ulong PostId { get; set; }
    }
    #endregion
}