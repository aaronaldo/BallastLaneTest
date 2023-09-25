using Ballastagram.Post.Models;

namespace Ballastagram.Post.Infrastructure.Post
{
    public interface IPostRepository
    {
        Task<PostModel> GetPost(PostPK pk);
        Task<IList<PostModel>> GetPostsByAuthorId(PostAuthorKey key);
        Task<PostModel> AddPost(PostModel post);
        Task<PostModel> EditPost(PostModel post);
        Task DeletePost(ulong commentId);
    }
}
