using PickDriverWeb.Models.Leagues;

namespace PickDriverWeb.Services;

internal static class DraftV2UiPolicy
{
    internal static bool IsV2(RaceDraft? draft)
        => string.Equals(draft?.GameplayVersion, "v2", StringComparison.OrdinalIgnoreCase);

    internal static bool IsResultVisible(RaceDraft? draft)
        => IsState(draft, "resolved") || IsState(draft, "finalized");

    internal static bool CanEditPreferences(
        RaceDraft? draft,
        DraftDeadline? deadlines,
        DateTimeOffset now)
    {
        if (!IsState(draft, "collecting"))
        {
            return false;
        }

        var submissionDeadline = draft?.SubmissionDeadline ?? deadlines?.SubmissionDeadline;
        return !submissionDeadline.HasValue || now < submissionDeadline.Value;
    }

    internal static bool IsBanWindowOpen(
        RaceDraft? draft,
        DraftDeadline? deadlines,
        bool bansEnabled,
        DateTimeOffset now)
    {
        if (!bansEnabled || !IsState(draft, "resolved"))
        {
            return false;
        }

        var opensAt = draft?.SubmissionDeadline ?? deadlines?.SubmissionDeadline;
        var closesAt = draft?.BanWindowClosesAt ?? deadlines?.BanWindowClosesAt;
        return opensAt.HasValue && closesAt.HasValue && now >= opensAt.Value && now < closesAt.Value;
    }

    internal static string? PreferenceNotice(
        PickPreferenceSettings? preferences,
        int totalPickCount,
        DateTimeOffset? previousRaceCutoff)
    {
        if (preferences is null || !preferences.Submitted || preferences.DriverIds.Count == 0)
        {
            return "Lista de picks vacia!";
        }

        if (preferences.DriverIds.Count < totalPickCount)
        {
            return "Lista de picks incompleta";
        }

        if (previousRaceCutoff.HasValue &&
            preferences.UpdatedAt.HasValue &&
            preferences.UpdatedAt.Value <= previousRaceCutoff.Value)
        {
            return "Lista aun no actualizada este GP";
        }

        return null;
    }

    internal static bool HasActorAlreadyBannedThisDraft(
        RaceDraft draft,
        int actorUserId,
        int? actorTeamId,
        Func<int, int?> resolveTeamId)
    {
        foreach (var bannerUserId in draft.BannedByUserIdsByPickIndex.Where(id => id.HasValue).Select(id => id!.Value))
        {
            if (actorTeamId.HasValue)
            {
                if (resolveTeamId(bannerUserId) == actorTeamId)
                {
                    return true;
                }
            }
            else if (bannerUserId == actorUserId)
            {
                return true;
            }
        }

        return false;
    }

    internal static bool IsTargetAlreadyBanned(RaceDraft draft, int targetUserId)
    {
        var count = Math.Min(draft.PickOrder.Count, draft.BannedDriverIdsByPickIndex.Count);
        for (var index = 0; index < count; index++)
        {
            if (draft.PickOrder[index] == targetUserId && draft.BannedDriverIdsByPickIndex[index].HasValue)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsState(RaceDraft? draft, string expected)
        => IsV2(draft) && string.Equals(draft?.ResolutionState, expected, StringComparison.OrdinalIgnoreCase);
}
