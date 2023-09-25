using Ballastagram.Post.Models;

namespace Ballastagram.Post.API.Requests
{
    public class PostRequest
    {
        // TODO: get id based on user claims
        public ulong Id { get; set; }
        public ulong AuthorId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }

    public static class PostRequestConverter
    {
        public static PostModel ToModel(PostRequest input)
        {
            return new PostModel() 
            { 
                Id = input.Id,
                AuthorId = input.AuthorId,
                Title = input.Title,
                Content = input.Text
            };
        }
    }
}
