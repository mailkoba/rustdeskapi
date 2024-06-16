using LiteDB;
using RustDeskApi.Domain;
using RustDeskApi.Models;

namespace RustDeskApi.Services
{
    public interface IStorageService
    {
        User[] GetAllUsers();

        User GetOrCreateUser(string login);

        User GetUserById(Guid userId);

        void UpdateUserTagsAndPeers(Guid userId, string[] tags, PeerModel[] peers);

        void GetUserTagsAndPeers(Guid userId, out string[] tags, out PeerModel[] peers);
    }

    public class StorageService : IStorageService
    {
        public User[] GetAllUsers()
        {
            Semaphore.Wait();

            try
            {
                using var db = new LiteDatabase(DbName);
                var users = db.GetCollection<User>(nameof(User).ToLower());
                return users.FindAll().ToArray();
            }
            finally
            {
                Semaphore.Release();
            }
        }

        public User GetOrCreateUser(string login)
        {
            Semaphore.Wait();

            try
            {
                using var db = new LiteDatabase(DbName);
                var users = db.GetCollection<User>(nameof (User).ToLower());

                var user = users.FindOne(x => x.Name.Equals(login));

                if (user == null)
                {
                    user = new User
                    {
                        Id = Guid.NewGuid(),
                        Name = login
                    };

                    users.Insert(user);
                    users.EnsureIndex(x => x.Name);
                }

                return user;
            }
            finally
            {
                Semaphore.Release();
            }
        }

        public User GetUserById(Guid userId)
        {
            Semaphore.Wait();

            try
            {
                using var db = new LiteDatabase(DbName);
                var users = db.GetCollection<User>(nameof(User).ToLower());

                return users.FindOne(x => x.Id == userId);
            }
            finally
            {
                Semaphore.Release();
            }
        }

        public void UpdateUserTagsAndPeers(Guid userId, string[] tags, PeerModel[] peers)
        {
            Semaphore.Wait();

            try
            {
                using var db = new LiteDatabase(DbName);
                var userTags = db.GetCollection<UserTag>(nameof(UserTag).ToLower());

                userTags.DeleteMany(x => x.UserId == userId);

                if (tags.Any())
                {
                    userTags.InsertBulk(tags.Select(x => new UserTag
                    {
                        UserId = userId,
                        Tag = x
                    }));
                }

                var userPeers = db.GetCollection<UserPeer>(nameof(UserPeer).ToLower());

                userPeers.DeleteMany(x => x.UserId == userId);

                if (peers.Any())
                {
                    userPeers.InsertBulk(peers.Select(x => new UserPeer
                    {
                        UserId = userId,
                        PeerId = x.Id,
                        HostName = x.Hostname,
                        UserName = x.Username,
                        Platform = x.Platform,
                        Alias = x.Alias,
                        Tags = x.Tags
                    }));
                }
            }
            finally
            {
                Semaphore.Release();
            }
        }

        public void GetUserTagsAndPeers(Guid userId, out string[] tags, out PeerModel[] peers)
        {
            tags = Array.Empty<string>();
            peers = Array.Empty<PeerModel>();

            Semaphore.Wait();

            try
            {
                using var db = new LiteDatabase(DbName);
                var userTags = db.GetCollection<UserTag>(nameof(UserTag).ToLower());

                tags = userTags.Find(x => x.UserId == userId).Select(x => x.Tag).ToArray();

                var userPeers = db.GetCollection<UserPeer>(nameof(UserPeer).ToLower());

                peers = userPeers.Find(x => x.UserId == userId)
                                 .Select(x => new PeerModel
                                  {
                                      Id = x.PeerId,
                                      Hostname = x.HostName,
                                      Platform = x.Platform,
                                      Username = x.UserName,
                                      Alias = x.Alias,
                                      Tags = x.Tags
                                  })
                                 .ToArray();
            }
            finally
            {
                Semaphore.Release();
            }
        }

        private static readonly SemaphoreSlim Semaphore = new(1);
        private const string DbName = "./api.db";
    }
}
