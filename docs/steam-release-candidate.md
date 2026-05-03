# Steam Release Candidate Checklist

This project is still a solo-scale indie candidate, not a guaranteed commercial launch. Treat every Windows build as a candidate only after the verification gate below passes.

## Required Gate

- Domain tests pass.
- Runtime compile check passes.
- Editor compile check passes.
- Unity batch scene/data regeneration succeeds.
- Unity Windows build succeeds.
- `MafiaTopDown.exe` launches and exits without blocking runtime errors.
- `Builds/Windows/release-manifest.json` exists for the exact build.
- `Builds/Windows/STEAM_RELEASE_README.txt` is included beside the executable.

## Gameplay Gate

- Start a clean save in slot 1.
- Start separate saves in slots 2 and 3.
- Relaunch and continue each occupied slot.
- Complete the Act I golden path from Luca to Vincent and back out.
- Travel docks to business, old quarter, and rail yard, then save/load at the correct spawn.
- Trigger heat escalation and confirm police response is visible and bounded.
- Complete one side job and confirm the main story state is not corrupted.

## Store-Candidate Notes

- The current strategy remains Windows/Steam first.
- Mobile stays post-launch until Windows retention, performance, and controls are stable.
- Do not upload a build without the release manifest and a clean launch sanity log.
- Keep outsourced/marketplace asset licenses in `docs/asset-sourcing-ledger.md`.
