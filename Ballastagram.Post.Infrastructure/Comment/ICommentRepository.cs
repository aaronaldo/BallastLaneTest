using Ballastagram.Post.Models;

namespace Ballastagram.Post.Infrastructure.Comment
{
    public interface ICommentRepository
    {
        Task<CommentModel> GetComment(CommentPK pk);
        Task<IList<CommentModel>> GetCommentsByAuthorId(CommentAuthorKey key);
        Task<IList<CommentModel>> GetCommentsByPostId(PostKey key);
        Task<CommentModel> AddComment(CommentModel comment);
        Task<CommentModel> EditComment(CommentModel comment);
        Task DeleteComment(ulong commentId);
    }
}
