using Ballastagram.Post.Models;

namespace Ballastagram.Post.API.Requests
{
    public class CommentRequest
    {
        public ulong Id { get; set; }
        public ulong AuthorId { get; set; }
        public string Content { get; set; }
    }

    public static class CommentRequestConverter
    {
        public static CommentModel ToModel(CommentRequest input)
        {
            return new CommentModel()
            {
                Id = input.Id,
                AuthorId = input.AuthorId,
                Content = input.Content
            };
        }
    }
}
