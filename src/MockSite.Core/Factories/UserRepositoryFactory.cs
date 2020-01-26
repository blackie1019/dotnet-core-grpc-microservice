using System;
using MockSite.Common.Core.Enums;
using MockSite.Core.Interfaces;

namespace MockSite.Core.Factories
{
    public class UserRepositoryFactory
    {
        private readonly IUserRepository _mariaUserRepository;
        private readonly IMongoUserRepository _mongoUserRepository;

        public UserRepositoryFactory(
            IUserRepository userRepository,
            IMongoUserRepository mongoUserRepository
        )
        {
            _mariaUserRepository = userRepository;
            _mongoUserRepository = mongoUserRepository;
        }

        public IUserRepository Produce(ConnectDb connectDb)
        {
            switch (connectDb)
            {
                case ConnectDb.Mongo:
                    return _mongoUserRepository;
                case ConnectDb.MariaDb:
                    return _mariaUserRepository;
                default:
                    throw new ArgumentOutOfRangeException(nameof(connectDb));
            }
        }
    }
}