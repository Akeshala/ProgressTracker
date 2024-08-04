using Microsoft.EntityFrameworkCore;
using ProgressTracker.Data;
using ProgressTracker.Models;

namespace ProgressTracker.Services
{
    public class SessionService : ISessionService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SessionService> _logger;

        public SessionService(AppDbContext context, ILogger<SessionService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<SessionModel>> GetAll()
        {
            var sessions = await _context.Sessions.ToListAsync();
            if (sessions.Count == 0)
            {
                _logger.LogWarning($"No session found.");
                return new List<SessionModel>();
            }
            return sessions.ToList();
        }

        public async Task<SessionModel?> GetOneById(int id)
        {
            var session = await _context.Sessions.FindAsync(id);
            return session;
        }
        
        public async Task<IEnumerable<SessionModel>> GetMultiByIds(IEnumerable<int> sessionIds)
        {
            var result = new List<SessionModel>();
            foreach (var sessionId in sessionIds)
            {
                var session = await _context.Sessions.FindAsync(sessionId);
                if (session != null)
                {
                    result.Add(session);
                }
            }
            return result;
        }

        public async void AddOne(SessionModel? session)
        {
            if (session != null)
            {
                await _context.Sessions.AddAsync(session);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> RemoveOne(int id)
        {
            var session = await _context.Sessions.FindAsync(id);
            if (session == null)
            {
                return false;
            }
            
            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
