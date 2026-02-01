# Game Rules - PickDriver (API)

This document describes the current rules as implemented in the API.

## Objective
Compete in a league by selecting Formula 1 drivers each race. You earn points based on the real-world performance of your chosen driver, and the league is decided by the highest total season score.

## Leagues
- Requires an active season (`season.active = true`).
- A league is created with status `pending`.
- The creator becomes `owner` and is also added as a member.
- `maxPlayers` defines the member limit.
- To join: the league must be `pending`, you cannot already be a member, and the league cannot be full (`memberCount >= maxPlayers`).

### Owner permissions
- Assign pick order (`assign-pick-order`).
- Start the draft (`start-draft`).
- Delete the league (only while `pending`).

## League options
- `teamsEnabled`, `bansEnabled`, `mirrorEnabled` are accepted with no extra validations at league creation.

## Teams
Only apply when `teamsEnabled = true`.

### Enablement rules
- The league must be `pending`.
- The league must be full (`memberCount == maxPlayers`).

### Team size rules
- Minimum team size: 2.
- Maximum teams = min(totalPlayers / 2, number of F1 teams in the season).
- Distribution must be feasible and balanced:
  - each team must be between floor(totalPlayers / k) and ceil(totalPlayers / k)
  - teams below the minimum are not allowed

### Membership rules
- No duplicate users inside a team.
- A user cannot belong to multiple teams.
- Only league members can be in teams.

## Draft
### Activation (start-draft)
- Owner only.
- League is `pending`.
- League is full (`members == maxPlayers`).
- If `teamsEnabled = true`, all players must be assigned to teams.

### Pick order and rotation
- If `pickOrder` is assigned for all members, it is respected.
- Otherwise, a random order is generated:
  - without teams: direct shuffle
  - with teams: shuffle by team and round-robin across teams
- For each future race starting at `initialRaceRound`, the order is rotated.
- If `mirrorEnabled = true`, the order is duplicated in mirror (rotated + reversed).

## Pick deadlines
- `firstHalfDeadline = fp1Time - 36h`
- `secondHalfDeadline = fp1Time`

## Autopick
- Each user can save an ordered list of drivers (`driverIDs`) per league.
- Duplicates are removed while preserving order.
- The list must match drivers in the league season.
- Sending an empty list clears the configuration.
- When a turn expires, autopick tries the first available driver:
  - not banned by the user
  - not already picked by another player
- If no valid autopick exists, the turn still expires and advances.

## Picks
### Access rules
- Only the user whose turn it is can pick.
- If `teamsEnabled = true` and less than 1 hour remains before `fp1`, a teammate can pick for the current turn.

### Validations
- No pick/ban if the race has started or completed:
  - `race.completed == true` or `race.raceTime < now`
- The driver must exist and belong to the race season.
- You cannot pick a driver already picked (global within the draft).
- You cannot pick a driver banned by the user.
- One pick per user per "mirror slot" (`is_mirror_pick`).

### Effects
- Inserts the pick and advances `currentPickIndex`.
- Notifies the next player when applicable.

## Bans
### Access rules
- Only if `bansEnabled = true`.
- You can only ban the immediately previous pick.
- You cannot ban the last player in the order (unless they are also the first).
- Leagues without teams:
  - each user can ban only once per race
  - a driver can be banned only once per race
- Leagues with teams:
  - each team can ban only once per race
- Permissions:
  - without teams: only the current user
  - with teams: the current user or a teammate

### Ban counts
- Without teams: 2 bans per user.
- With teams: 3 bans per team.
- Per-race restriction: each user/team can only use 1 ban per race; in leagues without teams, a driver can only be banned once per race.

### Effects
- Marks the pick as banned (`is_banned = true`) and stores `banned_by`.
- Moves `currentPickIndex` back to the previous pick so the user re-picks.
- Notifies the next player after the change.

## Standings and scoring
- Only non-banned picks are counted.
- Autopicks are worth 50% of the driver's points.
- Standings are calculated over completed races.
- For mirror picks, the system calculates position using the mirrored order.

## Draft notifications
- Starting the draft notifies the first user in the order.
- Completing a pick notifies the next user.
- Publishing results creates notifications tied to the race.
