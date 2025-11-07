using UserManagementAPI.Models;
using System.Collections.Concurrent;

namespace UserManagementAPI.Repositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<Guid, User> _store = new();

        public InMemoryUserRepository()
        {
            var seed = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@techhive.com",
                Department = "HR",
                CreatedAt = DateTime.UtcNow
            };
            _store[seed.Id] = seed;
        }

        public Task<User> CreateAsync(User user, CancellationToken ct = default)
        {
            user.Id = Guid.NewGuid();
            user.CreatedAt = DateTime.UtcNow;
            _store[user.Id] = user;
            return Task.FromResult(user);
        }

        public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var ok = _store.TryRemove(id, out _);
            return Task.FromResult(ok);
        }

        public Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default)
        {
            var items = _store.Values.OrderBy(u => u.CreatedAt).AsEnumerable();
            return Task.FromResult(items);
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            var user = _store.Values.FirstOrDefault(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(user);
        }

        public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            _store.TryGetValue(id, out var user);
            return Task.FromResult(user);
        }

        public Task<bool> UpdateAsync(User user, CancellationToken ct = default)
        {
            if (!_store.ContainsKey(user.Id)) return Task.FromResult(false);
            _store[user.Id] = user;
            return Task.FromResult(true);
        }
    }
}
