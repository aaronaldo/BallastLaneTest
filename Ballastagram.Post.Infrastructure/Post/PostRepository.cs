using Ballastagram.Commom;
using Ballastagram.Post.Infrastructure.Config;
using Ballastagram.Post.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace Ballastagram.Post.Infrastructure.Post
{
    public class PostRepository : IPostRepository
    {
        public string _connectionString { get; set; }
        private readonly DbConfig _config;
        public PostRepository(IOptions<DbConfig> config)
        {
            _connectionString = config.Value.ConnectionString;
            _config = config.Value;
        }

        private const string GET_POST = @"
            SELECT
                p.Id,
                p.AuthorId,
                p.Title,
                p.Text,
                p.CreationDate
            FROM
                [dbo].Post p
        ";

        private const string GET_POST_LIKE_COUNT = @"
            SELECT
                pl.PostId,
                count(*) count
            FROM
                [dbo].PostLike pl
            WHERE
                pl.PostId IN @ids
            GROUP BY
                pl.PostId
            ORDER BY
                pl.PostId
        ";

        private async Task<IList<PostModel>> InternalGetPosts(ModelKey<PostModel> key, string WHERE_CLAUSE)
        {
            using var conn = new SqlConnection(_connectionString);
            string sql = GET_POST + WHERE_CLAUSE;

            conn.Open();

            var posts = (await conn.QueryAsync<PostModel>(sql, key, commandTimeout: _config.CommandTimeout, commandType: CommandType.Text)).ToList();

            if (posts is null || posts.Count == 0)
                return posts;
            
            var postLikes = await conn.QueryAsync<PostLikeCount>(GET_POST_LIKE_COUNT, new { ids = posts.Select(p => p.Id) });
            if (postLikes is null || !postLikes.Any())
                return posts;

            AssignPostsAndLikes(posts, postLikes.ToList());

            return posts;
        }

        public async Task<PostModel> GetPost(PostPK pk)
        {
            const string WHERE_CLAUSE = " WHERE p.Id = @Id";

            var posts = await InternalGetPosts(pk, WHERE_CLAUSE);
            if (posts.Count > 1)
            {
                throw new Exception($"{nameof(PostModel)} PK must return only one post.");
            }

            return posts.First();
        }

        public async Task<IList<PostModel>> GetPostsByAuthorId(PostAuthorKey key)
        {
            const string WHERE_CLAUSE = " WHERE p.AuthorId = @AuthorId";

            return await InternalGetPosts(key, WHERE_CLAUSE);
        }

        private void AssignPostsAndLikes(IList<PostModel> posts, IList<PostLikeCount> postLikes)
        {
            foreach (var like in postLikes)
            {
                var post = posts.FirstOrDefault(c => c.Id == like.PostId);
                post.Likes = like.Count;
            }
        }

        private const string ADD_POST = @"
            INSERT [dbo].Post(
                AuthorId,
                Title,
                Content
            )
            VALUES(
                @AuthorId,
                @Title,
                @Content
            )";

        private const string EDIT_POST = @"
            UPDATE [dbo].Post
            SET 
                Content = @Content
            WHERE
                Id = @Id";

        private const string DELETE_POST = @"
            DELETE FROM [dbo].Post
            WHERE
                Id = @Id";


        public async Task<PostModel> AddPost(PostModel post)
        {
            using var conn = new SqlConnection(_connectionString);

            try
            {
                conn.Open();
                await conn.ExecuteScalarAsync(ADD_POST, post, commandType: CommandType.Text);
                return post;
            }
            catch (Exception ex)
            {
                // TODO: log error
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        public async Task<PostModel> EditPost(PostModel post)
        {
            using var conn = new SqlConnection(_connectionString);
            const string whereClause = " WHERE p.Id = @Id";
            const string queryContract = GET_POST + whereClause;

            try
            {
                conn.Open();
                PostModel oldPost = conn.QueryFirst<PostModel>(queryContract, post.Id);
                oldPost.Content = post.Content;
                await conn.ExecuteScalarAsync(EDIT_POST, post, commandType: CommandType.Text);
                return oldPost;
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

        public async Task DeletePost(ulong postId)
        {
            using var conn = new SqlConnection(_connectionString);

            try
            {
                conn.Open();
                await conn.ExecuteScalarAsync(DELETE_POST, new { Id = postId }, commandType: CommandType.Text);
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
