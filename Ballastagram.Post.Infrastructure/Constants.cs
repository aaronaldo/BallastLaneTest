namespace Ballastagram.Post.Infrastructure
{
    public static class Constants
    {
        public static class Comment
        {
            public const ushort COMMENT_MAX_LENGTH = 4000;
        }

        public static class Post
        {
            public const ushort CONTENT_MAX_LENGTH= 4000;
            public const ushort TITLE_MAX_LENGTH = 100;
        }
    }
}
