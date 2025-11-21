using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DesignPatterns.CQRS.Good3
{
    // ✅ GOOD: CQRS optimized for scalability and performance

    // ✅ WRITE SIDE - Optimized for transactional writes

    public class CreateUserCommand
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }

    public class UpdateUserEmailCommand
    {
        public int UserId { get; set; }
        public string NewEmail { get; set; }
    }

    // Write model - normalized for ACID operations
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public interface IUserWriteRepository
    {
        Task<int> CreateAsync(User user, CancellationToken ct = default);
        Task<User> GetByIdAsync(int id, CancellationToken ct = default);
        Task UpdateEmailAsync(int userId, string email, CancellationToken ct = default);
        Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    }

    public class CreateUserCommandHandler
    {
        private readonly IUserWriteRepository _writeRepository;
        private readonly IUserReadCacheInvalidator _cacheInvalidator;

        public CreateUserCommandHandler(
            IUserWriteRepository writeRepository,
            IUserReadCacheInvalidator cacheInvalidator)
        {
            _writeRepository = writeRepository;
            _cacheInvalidator = cacheInvalidator;
        }

        public async Task<int> HandleAsync(CreateUserCommand command, CancellationToken ct = default)
        {
            // ✅ Check uniqueness (optimized query)
            if (await _writeRepository.EmailExistsAsync(command.Email, ct))
                throw new InvalidOperationException("Email already exists");

            var user = new User
            {
                Username = command.Username,
                Email = command.Email,
                PasswordHash = command.PasswordHash,
                CreatedAt = DateTime.UtcNow
            };

            // ✅ Write to transactional database
            var userId = await _writeRepository.CreateAsync(user, ct);

            // ✅ Invalidate cache for reads
            await _cacheInvalidator.InvalidateUserAsync(userId, ct);

            return userId;
        }
    }

    public class UpdateUserEmailCommandHandler
    {
        private readonly IUserWriteRepository _writeRepository;
        private readonly IUserReadCacheInvalidator _cacheInvalidator;

        public UpdateUserEmailCommandHandler(
            IUserWriteRepository writeRepository,
            IUserReadCacheInvalidator cacheInvalidator)
        {
            _writeRepository = writeRepository;
            _cacheInvalidator = cacheInvalidator;
        }

        public async Task HandleAsync(UpdateUserEmailCommand command, CancellationToken ct = default)
        {
            // ✅ Optimized update - only affected field
            await _writeRepository.UpdateEmailAsync(command.UserId, command.NewEmail, ct);
            
            // ✅ Invalidate cache
            await _cacheInvalidator.InvalidateUserAsync(command.UserId, ct);
        }
    }

    // ✅ READ SIDE - Optimized for fast queries with caching

    // Lightweight read models for different use cases
    public class UserSummaryDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        // ✅ Only fields needed for listing
    }

    public class UserDetailDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int OrderCount { get; set; }
        public DateTime CreatedAt { get; set; }
        // ✅ Denormalized - includes computed fields
    }

    public class UserSearchResultDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string HighlightedText { get; set; }
        // ✅ Optimized for search with highlighting
    }

    // Queries
    public class GetUserByUsernameQuery
    {
        public string Username { get; set; }
    }

    public class SearchUsersQuery
    {
        public string SearchTerm { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class GetUserStatisticsQuery
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    // ✅ Read repository with caching
    public interface IUserReadRepository
    {
        Task<UserDetailDto> GetByUsernameAsync(string username, CancellationToken ct = default);
        Task<List<UserSearchResultDto>> SearchAsync(string searchTerm, int page, int pageSize, CancellationToken ct = default);
        Task<Dictionary<string, int>> GetOrderCountByUserAsync(int page, int pageSize, CancellationToken ct = default);
        Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    }

    // ✅ Cached read repository
    public class CachedUserReadRepository : IUserReadRepository
    {
        private readonly IUserReadRepository _innerRepository;
        private readonly IDistributedCache _cache;
        private const int CacheDurationMinutes = 15;

        public CachedUserReadRepository(IUserReadRepository innerRepository, IDistributedCache cache)
        {
            _innerRepository = innerRepository;
            _cache = cache;
        }

        public async Task<UserDetailDto> GetByUsernameAsync(string username, CancellationToken ct = default)
        {
            var cacheKey = $"user:username:{username}";
            
            // ✅ Try cache first
            var cached = await _cache.GetAsync<UserDetailDto>(cacheKey, ct);
            if (cached != null)
                return cached;

            // ✅ Load from database
            var user = await _innerRepository.GetByUsernameAsync(username, ct);
            
            // ✅ Store in cache
            if (user != null)
                await _cache.SetAsync(cacheKey, user, TimeSpan.FromMinutes(CacheDurationMinutes), ct);

            return user;
        }

        public async Task<List<UserSearchResultDto>> SearchAsync(string searchTerm, int page, int pageSize, CancellationToken ct = default)
        {
            // ✅ Search queries typically not cached due to high cardinality
            // ✅ But could cache popular searches
            return await _innerRepository.SearchAsync(searchTerm, page, pageSize, ct);
        }

        public async Task<Dictionary<string, int>> GetOrderCountByUserAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var cacheKey = $"stats:ordercounts:page:{page}:size:{pageSize}";
            
            // ✅ Cache expensive aggregations
            var cached = await _cache.GetAsync<Dictionary<string, int>>(cacheKey, ct);
            if (cached != null)
                return cached;

            var stats = await _innerRepository.GetOrderCountByUserAsync(page, pageSize, ct);
            await _cache.SetAsync(cacheKey, stats, TimeSpan.FromMinutes(5), ct);
            
            return stats;
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
        {
            // ✅ Simple existence check - not cached
            return await _innerRepository.EmailExistsAsync(email, ct);
        }
    }

    // ✅ Query handlers
    public class GetUserByUsernameQueryHandler
    {
        private readonly IUserReadRepository _readRepository;

        public GetUserByUsernameQueryHandler(IUserReadRepository readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<UserDetailDto> HandleAsync(GetUserByUsernameQuery query, CancellationToken ct = default)
        {
            // ✅ Fast read from cache or optimized read model
            return await _readRepository.GetByUsernameAsync(query.Username, ct);
        }
    }

    public class SearchUsersQueryHandler
    {
        private readonly IUserReadRepository _readRepository;

        public SearchUsersQueryHandler(IUserReadRepository readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<List<UserSearchResultDto>> HandleAsync(SearchUsersQuery query, CancellationToken ct = default)
        {
            // ✅ Optimized search with pagination
            // ✅ Could use Elasticsearch for full-text search
            return await _readRepository.SearchAsync(
                query.SearchTerm,
                query.PageNumber,
                query.PageSize,
                ct);
        }
    }

    public class GetUserStatisticsQueryHandler
    {
        private readonly IUserReadRepository _readRepository;

        public GetUserStatisticsQueryHandler(IUserReadRepository readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<Dictionary<string, int>> HandleAsync(GetUserStatisticsQuery query, CancellationToken ct = default)
        {
            // ✅ Cached aggregation query
            return await _readRepository.GetOrderCountByUserAsync(
                query.PageNumber,
                query.PageSize,
                ct);
        }
    }

    // ✅ Cache abstraction
    public interface IDistributedCache
    {
        Task<T> GetAsync<T>(string key, CancellationToken ct = default);
        Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken ct = default);
        Task RemoveAsync(string key, CancellationToken ct = default);
    }

    // ✅ Cache invalidation
    public interface IUserReadCacheInvalidator
    {
        Task InvalidateUserAsync(int userId, CancellationToken ct = default);
    }

    public class UserReadCacheInvalidator : IUserReadCacheInvalidator
    {
        private readonly IDistributedCache _cache;

        public UserReadCacheInvalidator(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task InvalidateUserAsync(int userId, CancellationToken ct = default)
        {
            // ✅ Invalidate all related cache keys
            await _cache.RemoveAsync($"user:id:{userId}", ct);
            // Could also invalidate by username if we track that mapping
        }
    }

    // ✅ Example: Using different storage for reads
    public interface IUserSearchRepository
    {
        // ✅ Could be Elasticsearch for full-text search
        Task<List<UserSearchResultDto>> FullTextSearchAsync(string query, CancellationToken ct = default);
    }

    // ✅ Benefits:
    // - Independent scaling of reads and writes
    // - Caching strategy for frequently accessed data
    // - Optimized queries with projections (only needed fields)
    // - Can use different storage (SQL for writes, Elasticsearch for search, Redis for cache)
    // - No blocking between reads and writes
    // - Aggregations cached separately
    // - Read replicas can handle query load
}
