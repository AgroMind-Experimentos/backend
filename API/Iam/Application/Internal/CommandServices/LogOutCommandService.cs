using EcotrackPlatform.API.Iam.Domain.Repositories;
using EcotrackPlatform.API.Shared.Domain.Repositories;

namespace EcotrackPlatform.API.Iam.Application.Internal.CommandServices;

public enum LogoutError
{
    None,
    SessionNotFoundOrInactive
}

public record LogoutResult(LogoutError Error = LogoutError.None)
{
    public bool Success => Error == LogoutError.None;
}

public class LogoutCommandService(IAuthSessionRepository sessions, IUnitOfWork uow)
{
    public async Task<LogoutResult> LogoutAsync(Guid sessionId)
    {
        var s = await sessions.FindByIdAsync(sessionId);
        if (s is null || !s.IsActive())
            return new LogoutResult(Error: LogoutError.SessionNotFoundOrInactive);

        s.Revoke();
        sessions.Update(s);
        await uow.CompleteAsync();

        return new LogoutResult();
    }
}