# Asset Sourcing Ledger

This project is no longer treating primitive cubes as acceptable final city art. Imported assets must be license-tracked before they are used in generated scenes.

## Approved Free Sources

| Source | Use | License | Shipping Status | Notes |
| --- | --- | --- | --- | --- |
| Kenney City Kits | temporary road, city, and prop kit fallback | CC0 | Approved | Already imported. Useful for fast layout proofing, but not strong enough alone for the Mafia production target. |
| Quaternius Buildings Pack | supplemental building meshes in FBX/OBJ/Blend | CC0 | Approved to import | Official page lists FBX, OBJ, Blend formats and free personal/commercial use. |
| Quaternius Ultimate Buildings Pack | modular building kit in FBX/OBJ/Blend | CC0 | Approved to import | Preferred free ready-made building kit for replacing primitive facade blocks. |
| Poly Haven | PBR textures, HDRIs, select models | CC0 | Approved to import | Good source for asphalt, brick, concrete, plaster, metal, and wet street material upgrades. |
| ambientCG | PBR textures and materials | CC0 | Approved to import | Good source for brick, concrete, asphalt, wood, roof, rust, and grime surfaces. |

## License Evidence

- Quaternius Buildings Pack page lists `CC0`, `FBX`, `OBJ`, and `Blend`, and says the pack is free for personal and commercial projects.
- Quaternius Ultimate Buildings Pack page lists `CC0`, `FBX`, `OBJ`, and `Blend`, and describes a modular building set.
- Poly Haven states that HDRIs, textures, and 3D models on the site are licensed as CC0 and can be used commercially.
- ambientCG states that downloadable assets are under Creative Commons CC0 1.0 and can be included raw in a video game.
- Kenney license files are already present under `Assets/ThirdParty/Kenney/*/License.txt` and state CC0 personal, educational, and commercial use.

## Asset Rules

- Do not claim the game is visually fixed while launch district scenes still depend on primitive cube facades as the first read.
- Do not import paid assets without explicit purchase approval from the user.
- Prefer FBX or OBJ over GLB until Unity import support is intentionally added.
- Keep third-party assets under `Assets/ThirdParty/<Vendor>/<Pack>/`.
- Keep any copied license/readme file beside the imported pack.
- Do not edit vendor files in-place except for Unity-generated `.meta` files; customize through project prefabs/material overrides.
- Use Kenney only as fallback background massing or low-priority props unless deliberately restyled.
- Every district must have a named art kit entry before it is considered productionized.

## Immediate Target Stack

- District mesh kits: Quaternius building packs where available, Kenney as fallback.
- Surface realism: Poly Haven or ambientCG PBR textures for asphalt, brick, concrete, roof, rust, and wet reflections.
- Project-authored style layer: custom materials, lights, fog, signage, facade trims, portals, and collision-safe placement.
- Verification: visual screenshot, Unity batch scene regeneration, compile checks, domain tests, Windows build, player log sanity.
