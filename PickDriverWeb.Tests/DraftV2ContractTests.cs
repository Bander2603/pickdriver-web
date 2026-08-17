using System.Text.Json;
using PickDriverWeb.Models;
using PickDriverWeb.Models.Leagues;
using PickDriverWeb.Services;
using PickDriverWeb.Tests.Infrastructure;
using Xunit;

namespace PickDriverWeb.Tests;

public sealed class DraftV2ContractTests
{
    [Fact]
    public void LegacyPayload_RemainsCompatibleWhenV2FieldsAreAbsent()
    {
        const string json = """
            {"id":1,"league":{"id":10},"raceID":39,"pickOrder":[1,2],"pickedDriverIDs":[null,null],"bannedDriverIDs":[],"bannedDriverIDsByPickIndex":[null,null],"currentPickIndex":0,"mirrorPicks":false,"status":"active"}
            """;

        var draft = JsonSerializer.Deserialize<RaceDraft>(json, ApiJson.Options);

        Assert.NotNull(draft);
        Assert.Equal("legacy", draft!.GameplayVersion);
        Assert.False(DraftV2UiPolicy.IsV2(draft));
    }

    [Fact]
    public void V2Payload_PreservesNullSlotsMirrorOccurrencesAndRevision()
    {
        const string json = """
            {"id":2,"league":{"id":10},"raceID":39,"pickOrder":[1,2,1],"pickedDriverIDs":[12,null,7],"bannedDriverIDs":[4],"bannedDriverIDsByPickIndex":[null,4,null],"bannedByUserIDsByPickIndex":[null,1,null],"currentPickIndex":0,"mirrorPicks":true,"status":"active","gameplayVersion":"v2","resolutionState":"resolved","resolutionRevision":3,"bansUsedByUserID":{"1":1},"bansUsedByTeamID":{"5":2},"banLimitPerActor":3}
            """;

        var draft = JsonSerializer.Deserialize<RaceDraft>(json, ApiJson.Options);

        Assert.NotNull(draft);
        Assert.Equal(new[] { 1, 2, 1 }, draft!.PickOrder);
        Assert.Null(draft.PickedDriverIds[1]);
        Assert.Equal(3, draft.ResolutionRevision);
        Assert.Equal(2, draft.BansUsedByTeamID["5"]);
    }

    [Fact]
    public void PreferenceNotice_DistinguishesEmptyPartialAndStaleLists()
    {
        var cutoff = DateTimeOffset.Parse("2026-08-10T12:00:00Z");

        Assert.Equal("Lista de picks vacia!", DraftV2UiPolicy.PreferenceNotice(
            new PickPreferenceSettings { Submitted = true }, 3, cutoff));
        Assert.Equal("Lista de picks incompleta", DraftV2UiPolicy.PreferenceNotice(
            new PickPreferenceSettings { Submitted = true, DriverIds = new() { 1, 2 } }, 3, cutoff));
        Assert.Equal("Lista aun no actualizada este GP", DraftV2UiPolicy.PreferenceNotice(
            new PickPreferenceSettings { Submitted = true, DriverIds = new() { 1, 2, 3 }, UpdatedAt = cutoff.AddMinutes(-1) }, 3, cutoff));
    }

    [Fact]
    public void BanWindow_UsesInclusiveOpenAndExclusiveClose()
    {
        var opens = DateTimeOffset.Parse("2026-08-20T11:30:00Z");
        var closes = DateTimeOffset.Parse("2026-08-21T11:30:00Z");
        var draft = new RaceDraft
        {
            GameplayVersion = "v2",
            ResolutionState = "resolved",
            SubmissionDeadline = opens,
            BanWindowClosesAt = closes
        };

        Assert.True(DraftV2UiPolicy.IsBanWindowOpen(draft, null, true, opens));
        Assert.False(DraftV2UiPolicy.IsBanWindowOpen(draft, null, true, closes));
        Assert.False(DraftV2UiPolicy.IsBanWindowOpen(draft, null, false, opens));
    }

    [Fact]
    public void PickButton_IsDisabledAtDeadlineAndForEveryPostSubmissionState()
    {
        var deadline = DateTimeOffset.Parse("2026-08-20T11:30:00Z");
        var draft = new RaceDraft
        {
            GameplayVersion = "v2",
            ResolutionState = "collecting",
            SubmissionDeadline = deadline
        };

        Assert.True(DraftV2UiPolicy.CanEditPreferences(draft, null, deadline.AddTicks(-1)));
        Assert.False(DraftV2UiPolicy.CanEditPreferences(draft, null, deadline));

        draft.ResolutionState = "resolved";
        Assert.False(DraftV2UiPolicy.CanEditPreferences(draft, null, deadline.AddHours(-1)));
        draft.ResolutionState = "finalized";
        Assert.False(DraftV2UiPolicy.CanEditPreferences(draft, null, deadline.AddDays(1)));
        draft.ResolutionState = "cancelled";
        Assert.False(DraftV2UiPolicy.CanEditPreferences(draft, null, deadline.AddDays(1)));
    }

    [Fact]
    public void TeamBanUsageAndTargetUsageAreCalculatedByUserAndPickIndex()
    {
        var draft = new RaceDraft
        {
            GameplayVersion = "v2",
            ResolutionState = "resolved",
            PickOrder = new() { 1, 2, 1, 3 },
            BannedDriverIdsByPickIndex = new() { null, 9, null, null },
            BannedByUserIdsByPickIndex = new() { null, 3, null, null }
        };
        int? TeamOf(int id) => id is 1 or 3 ? 5 : 6;

        Assert.True(DraftV2UiPolicy.IsTargetAlreadyBanned(draft, 2));
        Assert.False(DraftV2UiPolicy.IsTargetAlreadyBanned(draft, 1));
        Assert.True(DraftV2UiPolicy.HasActorAlreadyBannedThisDraft(draft, 1, 5, TeamOf));
    }

    [Fact]
    public async Task PickPreferences_AllowSubmittedEmptyAndDeduplicatePartialLists()
    {
        var client = new ApiClient(
            new HttpClient(new MockApiMessageHandler()) { BaseAddress = new Uri("https://example.test/") },
            new FakeAuthSessionStore());

        var empty = await client.PutAsync<AutopickSettings, PickPreferenceSettings>(
            "leagues/10/pick-preferences",
            new AutopickSettings(),
            auth: true);
        var partial = await client.PutAsync<AutopickSettings, PickPreferenceSettings>(
            "leagues/10/pick-preferences",
            new AutopickSettings { DriverIds = new() { 12, 4, 12 } },
            auth: true);

        Assert.True(empty.Success);
        Assert.True(empty.Data!.Submitted);
        Assert.Empty(empty.Data.DriverIds);
        Assert.Equal(new[] { 12, 4 }, partial.Data!.DriverIds);
    }
}
