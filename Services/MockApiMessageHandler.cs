using System.Net;
using System.Net.Http.Json;
using PickDriverWeb.Models;
using PickDriverWeb.Models.Auth;
using PickDriverWeb.Models.Leagues;
using PickDriverWeb.Models.Standings;

namespace PickDriverWeb.Services;

internal sealed class MockApiMessageHandler : HttpMessageHandler
{
    private RaceDraft? _draftState;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var path = NormalizePath(request.RequestUri);
        var method = request.Method;
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (Is(method, HttpMethod.Post) && Match(segments, "auth", "login"))
        {
            return Ok(new LoginResponse { User = MockApiData.DemoUser, Token = "demo-token" });
        }

        if (Is(method, HttpMethod.Post) && Match(segments, "auth", "register"))
        {
            return Ok(new RegisterResponse
            {
                User = MockApiData.DemoUser,
                VerificationEmailSent = true
            });
        }

        if (Is(method, HttpMethod.Post) && Match(segments, "auth", "resend-verification"))
        {
            return Ok(new AuthMessageResponse
            {
                Message = "If the account exists and is pending verification, a verification email has been sent."
            });
        }

        if (Is(method, HttpMethod.Post) && Match(segments, "auth", "forgot-password"))
        {
            return Ok(new AuthMessageResponse
            {
                Message = "If the account exists, password reset instructions have been sent."
            });
        }

        if (Is(method, HttpMethod.Post) && Match(segments, "auth", "reset-password"))
        {
            return Ok(new AuthMessageResponse
            {
                Message = "Password updated successfully."
            });
        }

        if (Is(method, HttpMethod.Post) && Match(segments, "auth", "google"))
        {
            return Ok(new LoginResponse { User = MockApiData.DemoUser, Token = "demo-token" });
        }

        if (Is(method, HttpMethod.Get) && Match(segments, "auth", "profile"))
        {
            return Ok(MockApiData.DemoUser);
        }

        if (Is(method, HttpMethod.Put) && Match(segments, "auth", "username"))
        {
            return Ok(MockApiData.DemoUser);
        }

        if (Is(method, HttpMethod.Put) && Match(segments, "auth", "password"))
        {
            return NoContent();
        }

        if (Is(method, HttpMethod.Delete) && Match(segments, "auth", "account"))
        {
            return NoContent();
        }

        if (Is(method, HttpMethod.Get) && Match(segments, "leagues", "my"))
        {
            return Ok(MockApiData.Leagues);
        }

        if (Is(method, HttpMethod.Get) && Match(segments, "leagues", "members", 0, 2))
        {
            return Ok(MockApiData.LeagueMembers);
        }

        if (Is(method, HttpMethod.Get) && Match(segments, "leagues", "teams", 0, 2))
        {
            return Ok(MockApiData.LeagueTeams);
        }

        if (Is(method, HttpMethod.Post) && Match(segments, "leagues", "create"))
        {
            return Ok(MockApiData.DemoLeague);
        }

        if (Is(method, HttpMethod.Post) && Match(segments, "leagues", "join"))
        {
            return Ok(MockApiData.DemoLeague);
        }

        if (Is(method, HttpMethod.Delete) && Match(segments, "leagues", null, 0, 1))
        {
            return NoContent();
        }

        if (Is(method, HttpMethod.Post) && Match(segments, "leagues", "assign-pick-order", 0, 2))
        {
            return NoContent();
        }

        if (Is(method, HttpMethod.Post) && Match(segments, "leagues", "start-draft", 0, 2))
        {
            return NoContent();
        }

        if (Is(method, HttpMethod.Get) && Match(segments, "races"))
        {
            return Ok(MockApiData.Races);
        }

        if (Is(method, HttpMethod.Get) && Match(segments, "races", "upcoming"))
        {
            return Ok(MockApiData.UpcomingRaces);
        }

        if (Is(method, HttpMethod.Get) && Match(segments, "races", "current"))
        {
            return Ok(MockApiData.CurrentRace);
        }

        if (Is(method, HttpMethod.Get) && Match(segments, "drivers"))
        {
            return Ok(MockApiData.Drivers);
        }

        if (Is(method, HttpMethod.Get) && Match(segments, "f1", "teams"))
        {
            return Ok(MockApiData.F1Teams);
        }

        if (Is(method, HttpMethod.Get) && Match(segments, "standings", "f1", "drivers"))
        {
            return Ok(MockApiData.DriverStandings);
        }

        if (Is(method, HttpMethod.Get) && Match(segments, "standings", "f1", "teams"))
        {
            return Ok(MockApiData.TeamStandings);
        }

        if (Is(method, HttpMethod.Get) && Match(segments, "players", "standings", "players"))
        {
            return Ok(MockApiData.PlayerStandings);
        }

        if (Is(method, HttpMethod.Get) && Match(segments, "players", "standings", "teams"))
        {
            return Ok(MockApiData.PlayerTeamStandings);
        }

        if (Is(method, HttpMethod.Get) && Match(segments, "players", "standings", "picks"))
        {
            return Ok(Array.Empty<PickHistory>());
        }

        if (Is(method, HttpMethod.Get) && Match(segments, "leagues", "draft", "deadlines", 0, 2, 4))
        {
            return Ok(MockApiData.DraftDeadlines);
        }

        if (Is(method, HttpMethod.Get) && Match(segments, "leagues", "draft", 0, 2))
        {
            return Ok(DraftState);
        }

        if (Is(method, HttpMethod.Post) && Match(segments, "leagues", "draft", "pick", 0, 2, 4))
        {
            var driverId = await TryReadDriverIdAsync(request, cancellationToken);
            var draft = DraftState;
            if (!driverId.HasValue)
            {
                return Error(HttpStatusCode.BadRequest, "Missing driverID");
            }

            var actingUserId = ResolveUserId();
            var editablePickIndex = ResolveEditablePickIndex(draft, actingUserId);
            if (!editablePickIndex.HasValue)
            {
                return Error(HttpStatusCode.Forbidden, "Your turn is no longer active");
            }

            if (IsDriverUnavailableForPick(draft, driverId.Value, editablePickIndex.Value))
            {
                return Error(HttpStatusCode.Conflict, "Driver no longer available");
            }

            draft.PickedDriverIds[editablePickIndex.Value] = driverId.Value;

            if (editablePickIndex.Value == draft.CurrentPickIndex && draft.CurrentPickIndex < draft.PickOrder.Count - 1)
            {
                draft.CurrentPickIndex++;
            }

            return Ok(BuildDraftResponse(draft));
        }

        if (Is(method, HttpMethod.Post) && Match(segments, "leagues", "draft", "ban", 0, 2, 4))
        {
            var driverId = await TryReadDriverIdAsync(request, cancellationToken);
            var draft = DraftState;
            if (driverId.HasValue)
            {
                if (!draft.BannedDriverIds.Contains(driverId.Value))
                {
                    draft.BannedDriverIds.Add(driverId.Value);
                }

                if (draft.BannedDriverIdsByPickIndex.Count > draft.CurrentPickIndex)
                {
                    draft.BannedDriverIdsByPickIndex[draft.CurrentPickIndex] = driverId.Value;
                }
            }

            return Ok(BuildDraftResponse(draft));
        }

        if (Is(method, HttpMethod.Get) && Match(segments, "leagues", "autopick", 0, 2))
        {
            return Ok(MockApiData.AutopickSettings);
        }

        if (Is(method, HttpMethod.Put) && Match(segments, "leagues", "autopick", 0, 2))
        {
            return Ok(MockApiData.AutopickSettings);
        }

        if (Is(method, HttpMethod.Post) && Match(segments, "teams"))
        {
            return Ok(MockApiData.LeagueTeams[0]);
        }

        if (Is(method, HttpMethod.Put) && Match(segments, "teams", null, 0, 1))
        {
            return Ok(MockApiData.LeagueTeams[0]);
        }

        if (Is(method, HttpMethod.Delete) && Match(segments, "teams", null, 0, 1))
        {
            return NoContent();
        }

        if (Is(method, HttpMethod.Get))
        {
            return Ok(Array.Empty<object>());
        }

        return Error(HttpStatusCode.NotImplemented, "Mock endpoint not implemented.");
    }

    private static string NormalizePath(Uri? uri)
    {
        var path = uri?.AbsolutePath ?? string.Empty;
        path = path.Trim('/');
        if (path.StartsWith("api/", StringComparison.OrdinalIgnoreCase))
        {
            path = path[4..];
        }

        return path;
    }

    private static bool Is(HttpMethod actual, HttpMethod expected) => actual == expected;

    private static bool Match(string[] segments, string first, string? second = "", int firstIndex = 0, int secondIndex = 1)
    {
        if (segments.Length <= firstIndex)
        {
            return false;
        }

        if (!string.Equals(segments[firstIndex], first, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (string.IsNullOrEmpty(second))
        {
            return true;
        }

        if (segments.Length <= secondIndex)
        {
            return false;
        }

        return string.Equals(segments[secondIndex], second, StringComparison.OrdinalIgnoreCase);
    }

    private static bool Match(string[] segments, string first, string second, string? third, int firstIndex = 0, int secondIndex = 1, int thirdIndex = 2)
    {
        if (!Match(segments, first, second, firstIndex, secondIndex))
        {
            return false;
        }

        return segments.Length > thirdIndex
            && string.Equals(segments[thirdIndex], third, StringComparison.OrdinalIgnoreCase);
    }

    private static HttpResponseMessage Ok<T>(T payload)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(payload, options: ApiJson.Options)
        };
    }

    private static HttpResponseMessage NoContent() => new(HttpStatusCode.NoContent);

    private static HttpResponseMessage Error(HttpStatusCode statusCode, string reason)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = JsonContent.Create(new ApiError { Error = true, Reason = reason }, options: ApiJson.Options)
        };
    }

    private RaceDraft DraftState => _draftState ??= MockApiData.Draft;

    private static DraftResponse BuildDraftResponse(RaceDraft draft)
    {
        var nextUserId = draft.CurrentPickIndex >= 0 && draft.CurrentPickIndex < draft.PickOrder.Count
            ? draft.PickOrder[draft.CurrentPickIndex]
            : (int?)null;

        return new DraftResponse
        {
            Status = draft.Status,
            CurrentPickIndex = draft.CurrentPickIndex,
            NextUserId = nextUserId,
            BannedDriverIds = draft.BannedDriverIds.ToList(),
            PickedDriverIds = draft.PickedDriverIds.Where(id => id.HasValue).Select(id => id!.Value).ToList(),
            YourTurn = nextUserId == MockApiData.DemoUser.Id,
            YourDeadline = DateTimeOffset.UtcNow.AddMinutes(30)
        };
    }

    private static int ResolveUserId()
    {
        // Mock auth is always the demo user.
        return MockApiData.DemoUser.Id;
    }

    private static int? ResolveEditablePickIndex(RaceDraft draft, int userId)
    {
        if (draft.CurrentPickIndex >= 0
            && draft.CurrentPickIndex < draft.PickOrder.Count
            && draft.CurrentPickIndex < draft.PickedDriverIds.Count
            && draft.PickOrder[draft.CurrentPickIndex] == userId)
        {
            return draft.CurrentPickIndex;
        }

        var previousIndex = draft.CurrentPickIndex - 1;
        if (previousIndex >= 0
            && previousIndex < draft.PickOrder.Count
            && previousIndex < draft.PickedDriverIds.Count
            && draft.PickOrder[previousIndex] == userId
            && draft.PickedDriverIds[previousIndex].HasValue)
        {
            return previousIndex;
        }

        return null;
    }

    private static bool IsDriverUnavailableForPick(RaceDraft draft, int driverId, int editablePickIndex)
    {
        if (draft.BannedDriverIds.Contains(driverId))
        {
            return true;
        }

        for (var i = 0; i < draft.PickedDriverIds.Count; i++)
        {
            if (i == editablePickIndex)
            {
                continue;
            }

            if (draft.PickedDriverIds[i] == driverId)
            {
                return true;
            }
        }

        return false;
    }

    private static async Task<int?> TryReadDriverIdAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Content is null)
        {
            return null;
        }

        try
        {
            var payload = await request.Content.ReadFromJsonAsync<Dictionary<string, int>>(ApiJson.Options, cancellationToken);
            if (payload is null)
            {
                return null;
            }

            if (payload.TryGetValue("driverID", out var value) ||
                payload.TryGetValue("driverId", out value) ||
                payload.TryGetValue("driver_id", out value))
            {
                return value;
            }
        }
        catch
        {
            // Ignore malformed payloads in mock mode.
        }

        return null;
    }
}
