# Real Asset City Rebuild Plan

## Goal

Replace the current primitive-heavy city presentation with an asset-driven, license-tracked production pipeline that can actually scale toward the Mafia-inspired launch target.

The honest baseline is that the current build proves gameplay architecture, not visual quality. It has a working campaign/save/travel foundation, but the city still reads like a prototype because too much of the first screen is generated from cubes and low-poly fallback props. This plan fixes the pipeline first so future work changes the big picture instead of nudging small decorations.

## Non-Negotiables

- Stop treating primitive building blocks as final art.
- Use ready assets first, with license evidence recorded in `docs/asset-sourcing-ledger.md`.
- Keep gameplay systems, scene transitions, save state, and mission data intact while replacing the visual layer.
- Make every district visually distinct, not just renamed.
- Verify with screenshots and builds before claiming a slice is fixed.

## Current Production Base

- `PrototypeSceneBuilder` generates boot, four district scenes, and four interiors.
- Campaign assets cover Acts I-III and several side activities.
- Runtime systems exist for save/load, chapter progression, travel, mission objectives, side jobs, police/heat, dialogue, and basic combat.
- Imported Kenney kits exist, but they are not sufficient as the dominant visual language.

## Architecture Pivot

The builder stays, but its responsibility changes:

- `PrototypeSceneBuilder` remains responsible for scene bootstrap, runtime controllers, spawn points, mission wiring, and safe travel.
- New art-kit logic owns the first visual read: district facade rows, hero landmarks, curb/road assets, prop clusters, material overrides, signage, and lighting.
- Imported third-party kit paths are centralized through a catalog so asset changes do not require rewriting district generation.
- Primitive geometry is allowed only for invisible collision, temporary blockout, simple trim, or small custom details where no asset exists yet.

## Target File Layout

- `docs/asset-sourcing-ledger.md`: license and source tracking.
- `Assets/Game/Editor/EnvironmentArtCatalog.cs`: deterministic lookup for imported/fallback art assets.
- `Assets/Game/Editor/PrototypeSceneBuilder.cs`: district builder calls the catalog-driven art pass.
- `Assets/ThirdParty/Quaternius/*`: imported free building/model kits when download succeeds.
- `Assets/Game/Materials/*`: project material overrides and PBR texture bindings.

## Phase 1: Docks Rebuild Slice

Acceptance target:

- Player no longer starts inside or visually trapped by building massing.
- The first screen reads as a dockside street with depth: facades, storefronts, loading doors, vertical detail, rooflines, alley gaps, street furniture, and readable paths.
- Building massing uses imported assets when available and project-authored fallback facades only as backup.
- Gameplay golden path remains intact: Luca, sedan, back office, Vincent, exit.

Implementation steps:

1. Add an editor art catalog that resolves Quaternius, Kenney, and fallback assets by semantic role.
2. Replace the Docks first-screen building pass with `CreateProductionDistrictArtPass`.
3. Create collision-safe sidewalk and road envelopes with explicit walkable margins.
4. Move the initial spawn to a clear sidewalk composition instead of the road center/building edge.
5. Add facade rhythm: windows, doors, signs, fire escapes, roof silhouettes, loading bays, crates, barrels, streetlamps, puddles, and alley breaks.
6. Run scene regeneration, compile checks, domain tests, Windows build, and player-log sanity.
7. Take a screenshot and judge it against the style bible before moving on.

## Phase 2: Four District Productionization

Acceptance target:

- Docks: cold wet warehouse edge, piers, loading doors, crates, steam, waterline.
- Business core: taller masonry/commercial frontage, streetcar rails, lit offices, union square landmark.
- Old quarter: dense tenements, chapel landmark, laundry lines, warm residential windows.
- Rail yard: factory belt, rails, garage, tanks, rust, work lights, fenced yards.

Implementation steps:

1. Give each district a catalog profile: building kit roles, palette, hero landmark, prop family, arrival composition.
2. Replace primitive rows in each district with catalog-driven facades and asset-backed props.
3. Add district-specific gate arrival scenes and return spawns.
4. Run visual verification for all four scenes.

## Phase 3: Real Material Pass

Acceptance target:

- Asphalt, brick, concrete, roof, rust, and wood no longer read as flat color blocks.
- Wet street reflections are material-driven, not a fake full-screen filter.

Implementation steps:

1. Import CC0 PBR texture sets from Poly Haven or ambientCG.
2. Create Unity materials with albedo/normal/roughness where Unity import supports them cleanly.
3. Apply materials through the art catalog, not ad-hoc per-scene calls.
4. Re-run scene generation and build.

## Phase 4: Content Expansion Against The Same Pipeline

Acceptance target:

- 10-14 hero interiors.
- 12-14 main missions.
- 6-10 side jobs.
- No district is an empty connector zone.

Implementation steps:

1. Add remaining hero interiors as separate scenes.
2. Fill Act II and Act III mission scenes using existing campaign data.
3. Bind side jobs to district unlocks, payouts, and save persistence.
4. Keep adding visual content through catalog profiles, not new one-off primitive passes.

## Phase 5: Launch Polish And Hardening

Acceptance target:

- Settings menu, save-slot flow, subtitles/readability defaults, controller checks, release build reproducibility.
- Full main path can be completed without blocking bugs.
- Build is not called launch-ready until the full campaign, side jobs, interiors, saves, travel, and release UX pass verification.

## Ralph Loop For Every Slice

1. Lock the acceptance target.
2. Add or update tests where behavior can be proven automatically.
3. Implement only the slice.
4. Run domain tests, runtime compile check, editor compile check, scene/data regeneration, Windows build, and player launch sanity.
5. Self-review the result against the screenshot/style bible.
6. Fix the highest-impact defect immediately before moving on.

## First Slice Starts Now

The first implementation target is not the full 6-10 hour game in one commit. It is the Docks production-art pivot: build the first screen correctly, prove the pipeline, and then apply it across the rest of the city.
