using MediatR;
using Ballastagram.Commom;
using Ballastagram.User.Models;

namespace Ballastagram.User.Infrasctructure

{
    public class UserMediator
    {
        public class Query : IRequest<IList<UserModel>>
        {
            public ModelKey<UserModel> Key { get; }
            public Query(ModelKey<UserModel> key)
            {
                Key = key;
            }
        }

        public class QueryHandler : IRequestHandler<Query, IList<UserModel>>
        {
            private readonly IUserRepository _repository;

            public QueryHandler(IUserRepository repository)
            {
                _repository = repository;
            }

            public async Task<IList<UserModel>> Handle(Query request, CancellationToken _)
            {
                IList<UserModel> result;

                if (request.Key is UserPK pk)
                {
                    result = new List<UserModel>() { await _repository.GetUser(pk) };
                }
                else
                {
                    throw new Exception($"ModelKey not implemented: {request.Key.GetType().Name}");
                }

                return result;
            }
        }

        public class Command : IRequest<IList<UserModel>>
        {
            public UserModel User { get; set; }
            public Command(UserModel user)
            {
                User = user;
            }
        }

        public class CommandHandler : IRequestHandler<Command, IList<UserModel>>
        {
            public Task<IList<UserModel>> Handle(Command request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
