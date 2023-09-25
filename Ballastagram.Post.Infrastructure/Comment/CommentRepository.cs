using Ballastagram.Commom;
using Ballastagram.Post.Infrastructure.Comment;
using Ballastagram.Post.Infrastructure.Config;
using Ballastagram.Post.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace Ballastagram.Comment.Infrastructure.Comment
{
    public class CommentRepository : ICommentRepository
    {
        public string _connectionString { get; set; }
        private readonly DbConfig _config;
        public CommentRepository(IOptions<DbConfig> config)
        {
            _connectionString = config.Value.ConnectionString;
            _config = config.Value;
        }

        private const string GET_COMMENT = @"
            SELECT
                c.Id,
                c.CommentId,
                c.AuthorId,
                c.Content,
                c.CreationDate
            FROM
                [dbo].Comment c
        ";

        private const string GET_COMMENT_LIKE_COUNT = @"
            SELECT
                cl.CommentId,
                count(*) count
            FROM
                [dbo].CommentLike cl
            WHERE
                cl.CommentId IN @ids
            GROUP BY
                cl.CommentId
            ORDER BY
                cl.CommentId
        ";

        private async Task<IList<CommentModel>> InternalGetCommments(ModelKey<CommentModel> key, string WHERE_CLAUSE)
        {
            using var conn = new SqlConnection(_connectionString);

            try
            {   
                string sql = GET_COMMENT + WHERE_CLAUSE;

                conn.Open();

                var comments = (await conn.QueryAsync<CommentModel>(sql, key, commandTimeout: _config.CommandTimeout, commandType: CommandType.Text)).ToList();

                if (comments is null || comments.Count == 0)
                    return comments;

                var commentLikes = await conn.QueryAsync<CommentLikeCount>(GET_COMMENT_LIKE_COUNT, new { ids = comments.Select(p => p.Id) });
                if (commentLikes is null || !commentLikes.Any())
                    return comments;

                AssignCommentsAndLikes(comments, commentLikes.ToList());

                return comments;
                
            }
            catch (Exception ex)
            {
                // TODO: log exception
                throw;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        private void AssignCommentsAndLikes(IList<CommentModel> comments, IList<CommentLikeCount> commentLikes)
        {
            foreach (var like in commentLikes)
            {
                var comment = comments.FirstOrDefault(c => c.Id == like.CommentId);
                comment.Likes = like.Count;
            }
        }

        public async Task<CommentModel> GetComment(CommentPK pk)
        {
            const string WHERE_CLAUSE = " WHERE c.Id = @Id";

            var comments = await InternalGetCommments(pk, WHERE_CLAUSE);
            if (comments.Count > 1)
            {
                throw new Exception($"{nameof(CommentModel)} PK must return only one post.");
            }

            return comments.First();
        }

        public async Task<IList<CommentModel>> GetCommentsByAuthorId(CommentAuthorKey key)
        {
            const string WHERE_CLAUSE = " WHERE c.AuthorId = @AuthorId";

            return await InternalGetCommments(key, WHERE_CLAUSE);
        }

        public async Task<IList<CommentModel>> GetCommentsByPostId(PostKey key)
        {
            const string WHERE_CLAUSE = " WHERE c.PostId = @PostId";

            return await InternalGetCommments(key, WHERE_CLAUSE);
        }

        private const string ADD_COMMENT = @"
            INSERT [dbo].Comment(
                PostId,
                AuthorId,
                Content
            )
            VALUES(
                @PostId,
                @AuthorId,
                @Content
            )";

        private const string EDIT_COMMENT = @"
            UPDATE [dbo].Comment
            SET 
                Content = @Content
            WHERE
                Id = @Id";

        private const string DELETE_COMMENT = @"
            DELETE FROM [dbo].Comment
            WHERE
                Id = @Id";


        public async Task<CommentModel> AddComment(CommentModel comment)
        {
            using var conn = new SqlConnection(_connectionString);

            try
            {
                conn.Open();
                await conn.ExecuteScalarAsync(ADD_COMMENT, comment, commandType: CommandType.Text);
                return comment;
            }
            catch (Exception ex)
            {
                // TODO: log
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        public async Task<CommentModel> EditComment(CommentModel comment)
        {
            using var conn = new SqlConnection(_connectionString);
            const string whereClause = " WHERE c.Id = @Id";
            const string queryContract = GET_COMMENT + whereClause;

            try
            {
                conn.Open();
                CommentModel oldComment = conn.QueryFirst<CommentModel>(queryContract, comment.Id);
                oldComment.Content = comment.Content;
                await conn.ExecuteScalarAsync(EDIT_COMMENT, comment, commandType: CommandType.Text);
                return oldComment;
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        public async Task DeleteComment(ulong commentId)
        {
            using var conn = new SqlConnection(_connectionString);

            try
            {
                conn.Open();
                await conn.ExecuteScalarAsync(DELETE_COMMENT, new { Id = commentId }, commandType: CommandType.Text);
            }
            catch (Exception ex)
            {
                // TODO: log
                throw;
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
