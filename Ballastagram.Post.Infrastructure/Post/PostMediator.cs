using MediatR;
using Ballastagram.Commom;
using Ballastagram.Post.Models;
using Ballastagram.Post.Infrastructure.Post;
using static Ballastagram.Post.Infrastructure.Constants;

namespace Ballastagram.Post.Infrastructure.Post
{
    public class PostMediator
    {
        public class Query : IRequest<IList<PostModel>>
        {
            public ModelKey<PostModel> Key { get; }
            public Query(ModelKey<PostModel> key)
            {
                Key = key;
            }
        }

        public class QueryHandler : IRequestHandler<Query, IList<PostModel>>
        {
            private readonly IPostRepository _repository;

            public QueryHandler(IPostRepository repository)
            {
                _repository = repository;
            }

            public async Task<IList<PostModel>> Handle(Query request, CancellationToken _)
            {
                IList<PostModel> result;

                if (request.Key is PostPK pk)
                {
                    result = new List<PostModel>() { await _repository.GetPost(pk) };
                }
                else if (request.Key is PostAuthorKey key)
                {
                    result = await _repository.GetPostsByAuthorId(key);
                }
                else
                {
                    throw new Exception($"ModelKey not implemented: {request.Key.GetType().Name}");
                }

                return result;
            }
        }

        public class AddCommand : IRequest<PostModel>
        {
            public PostModel Post { get; set; }
            public AddCommand(PostModel post)
            {
                Post = post;
            }
        }

        public class CommandHandler : IRequestHandler<AddCommand, PostModel>
        {
            private readonly IPostRepository _repository;

            public CommandHandler(IPostRepository repository)
            {
                _repository = repository;
            }

            public async Task<PostModel> Handle(AddCommand request, CancellationToken cancellationToken)

            {
                ValidatePost(request.Post);

                return await _repository.AddPost(request.Post);
            }

            public void ValidatePost(PostModel post)
            {
                if (string.IsNullOrEmpty(post.Title))
                    throw new ArgumentException("Title can't be empty");

                if (post.Title.Length > Constants.Post.TITLE_MAX_LENGTH)
                    throw new ArgumentException($"Post max length is {Constants.Post.TITLE_MAX_LENGTH}");

                if (string.IsNullOrEmpty(post.Content))
                    throw new ArgumentException("Content can't be empty");

                if (post.AuthorId <= 0)
                    throw new ArgumentException("Post must have an author");

                if (post.Content.Length > Constants.Post.CONTENT_MAX_LENGTH)
                    throw new ArgumentException($"Post max length is {Constants.Post.CONTENT_MAX_LENGTH}");
            }
        }

        public class EditCommand : IRequest<PostModel>
        {
            public PostModel Post { get; set; }
            public EditCommand(PostModel post)
            {
                Post = post;
            }
        }

        public class EditCommandHandler : IRequestHandler<EditCommand, PostModel>
        {
            private readonly IPostRepository _repository;

            public EditCommandHandler(IPostRepository repository)
            {
                _repository = repository;
            }

            public async Task<PostModel> Handle(EditCommand request, CancellationToken cancellationToken)
            {
                await ValidatePost(request.Post);

                return await _repository.EditPost(request.Post);
            }

            public async Task ValidatePost(PostModel post)
            {
                if (string.IsNullOrEmpty(post.Title))
                    throw new ArgumentException("Title can't be empty");

                if (post.Title.Length > Constants.Post.TITLE_MAX_LENGTH)
                    throw new ArgumentException($"Post max length is {Constants.Post.TITLE_MAX_LENGTH}");

                if (string.IsNullOrEmpty(post.Content))
                    throw new ArgumentException("Content can't be empty");

                if (post.AuthorId <= 0)
                    throw new ArgumentException("Post must have an author");

                if (post.Content.Length > Constants.Post.CONTENT_MAX_LENGTH)
                    throw new ArgumentException($"Post max length is {Constants.Post.CONTENT_MAX_LENGTH}");

                var oldPost = await _repository.GetPost(new PostPK { Id = post.Id });
                if (oldPost is null)
                {
                    throw new ArgumentException($"Could not find post {post.Id}");
                }
            }
        }

        public class DeleteCommand : IRequest
        {
            public ulong PostId { get; set; }
            public DeleteCommand(ulong postId)
            {
                PostId = postId;
            }
        }

        public class DeleteCommandHandler : IRequestHandler<DeleteCommand>
        {
            private readonly IPostRepository _repository;

            public DeleteCommandHandler(IPostRepository repository)
            {
                _repository = repository;
            }

            public async Task Handle(DeleteCommand request, CancellationToken cancellationToken)
            {
                await _repository.DeletePost(request.PostId);
            }
        }
    }
}
