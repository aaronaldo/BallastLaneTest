create table Users(
	Id int identity(1,1) Primary key,
	Name varchar(50),
	Bio nvarchar(4000),
	Email varchar(30),
	Nickname varchar(30),
	Birthdate datetime,
	CreationDate DateTime DEFAULT GetDate()
)
GO

create table Post (
	Id int identity(1,1) Primary key,	
	AuthorId int,
	Title varchar(100),
	Content nvarchar(4000),
	CreationDate DateTime,
	Constraint FK_Users_Post Foreign Key(AuthorId) references Users(Id)
)
GO

create table Comment (
	Id int identity(1,1) Primary key,
	PostId int,
	AuthorId int,
	Content nvarchar(4000),
	CreationDate DateTime DEFAULT GetDate(),
	Constraint FK_Post_Comment Foreign Key(PostId) references Post(Id),
	Constraint FK_User_Comment Foreign Key(AuthorId) references Users(Id)
)
GO

create table PostLike (
	Id int identity(1,1) Primary key,
	PostId int,
	AuthorId int,
	CreationDate DateTime DEFAULT GetDate(),
	Constraint FK_Post_PostLike Foreign Key(PostId) references Post(Id),
	Constraint FK_Author_PostLike Foreign Key(AuthorId) references Users(Id)
)
GO

create table CommentLike (
	Id int identity(1,1) Primary key,
	CommentId int,
	AuthorId int,
	CreationDate DateTime DEFAULT GetDate(),
	Constraint FK_Post_CommentLike Foreign Key(CommentId) references Comment(Id),
	Constraint FK_Author_CommentLike Foreign Key(AuthorId) references Users(Id)
)
GO