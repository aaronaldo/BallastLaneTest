using Ballastagram.Commom;
using Ballastagram.Post.Infrastructure;
using Ballastagram.Post.Infrastructure.Comment;
using Ballastagram.Post.Models;
using MediatR;

namespace Ballastagram.Comment.Infrastructure.Comment
{
    public class CommentMediator
    {
        public class Query : IRequest<IList<CommentModel>>
        {
            public ModelKey<CommentModel> Key { get; }
            public Query(ModelKey<CommentModel> key)
            {
                Key = key;
            }
        }

        public class QueryHandler : IRequestHandler<Query, IList<CommentModel>>
        {
            private readonly ICommentRepository _repository;

            public QueryHandler(ICommentRepository repository)
            {
                _repository = repository;
            }

            public async Task<IList<CommentModel>> Handle(Query request, CancellationToken _)
            {
                IList<CommentModel> result;

                if (request.Key is CommentPK pk)
                {
                    result = new List<CommentModel>() { await _repository.GetComment(pk) };
                }
                else if (request.Key is CommentAuthorKey authorKey)
                {
                    result = await _repository.GetCommentsByAuthorId(authorKey);
                }
                else if (request.Key is PostKey postKey)
                {
                    result = await _repository.GetCommentsByPostId(postKey);
                }
                else
                {
                    throw new Exception($"ModelKey not implemented: {request.Key.GetType().Name}");
                }

                return result;
            }
        }

        public class AddCommand : IRequest<CommentModel>
        {
            public CommentModel Comment { get; set; }
            public AddCommand(CommentModel comment)
            {
                Comment = comment;
            }
        }

        public class AddCommandHandler : IRequestHandler<AddCommand, CommentModel>
        {
            private readonly ICommentRepository _repository;

            public AddCommandHandler(ICommentRepository repository)
            {
                _repository = repository;
            }

            public async Task<CommentModel> Handle(AddCommand request, CancellationToken cancellationToken)
            {
                ValidateComment(request.Comment);

                return await _repository.AddComment(request.Comment);
            }

            public void ValidateComment(CommentModel comment)
            {
                if (string.IsNullOrEmpty(comment.Content))
                    throw new ArgumentException("Comment can't be empty");

                if (comment.AuthorId <= 0)
                    throw new ArgumentException("Comment must have an author");

                if (comment.PostId <= 0)
                    throw new ArgumentException("Comment must be linked to a post");

                if (comment.Content.Length > Constants.Comment.COMMENT_MAX_LENGTH)
                    throw new ArgumentException($"Comment max length is {Constants.Comment.COMMENT_MAX_LENGTH}");
            } 
        }

        public class EditCommand : IRequest<CommentModel>
        {
            public CommentModel Comment { get; set; }
            public EditCommand(CommentModel comment)
            {
                Comment = comment;
            }
        }

        public class EditCommandHandler : IRequestHandler<EditCommand, CommentModel>
        {
            private readonly ICommentRepository _repository;

            public EditCommandHandler(ICommentRepository repository)
            {
                _repository = repository;
            }

            public async Task<CommentModel> Handle(EditCommand request, CancellationToken cancellationToken)
            {
                await ValidateComment(request.Comment);

                return await _repository.EditComment(request.Comment);
            }

            public async Task ValidateComment(CommentModel comment)
            {
                if (string.IsNullOrEmpty(comment.Content))
                    throw new ArgumentException("Comment can't be empty");

                if (comment.AuthorId <= 0)
                    throw new ArgumentException("Comment must have an author");

                if (comment.PostId <= 0)
                    throw new ArgumentException("Comment must be linked to a post");

                if (comment.Content.Length > Constants.Comment.COMMENT_MAX_LENGTH)
                    throw new ArgumentException($"Comment max length is {Constants.Comment.COMMENT_MAX_LENGTH}");

                var oldComment = await _repository.GetComment(new CommentPK { Id = comment.Id });
                if (oldComment is null)
                {
                    throw new ArgumentException($"Could not find comment {comment.Id}");
                }
            }
        }

        public class DeleteCommand : IRequest
        {
            public ulong CommentId { get; set; }
            public DeleteCommand(ulong commentId)
            {
                CommentId = commentId;
            }
        }

        public class DeleteCommandHandler : IRequestHandler<DeleteCommand>
        {
            private readonly ICommentRepository _repository;

            public DeleteCommandHandler(ICommentRepository repository)
            {
                _repository = repository;
            }

            public async Task Handle(DeleteCommand request, CancellationToken cancellationToken)
            {
                await _repository.DeleteComment(request.CommentId);
            }
        }
    }
}
