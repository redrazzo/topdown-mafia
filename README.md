# MafiaTopDown

A Unity-first prototype for a Mafia 1/2-inspired top-down 3D crime game.

## Current state

- Unity editor installed: `6000.3.14f1`
- Windows IL2CPP module installed
- Blender installed
- Visual Studio Community installed
- .NET SDK installed for domain-layer tests
- Domain tests passing outside Unity

## Known blocker

The local Unity editor needs a valid Unity license activation before batchmode editor automation can create scenes or import packages. The project skeleton, runtime scripts, and editor bootstrap helpers are already in place so the first licensed launch can finish setup quickly.

## First launch checklist

1. Sign in to Unity Hub and activate a Personal or Pro license.
2. Open this project folder in Unity.
3. Run `MafiaTopDown/Setup/Install Recommended Packages`.
4. Run `MafiaTopDown/Setup/Create Starter Scenes`.
5. Assign scene references in the opening chapter director.

## Slice goal

The first playable chapter should prove:

- walk to the assigned car
- enter and drive through a compact port-city district
- arrive at the target building
- transition into the interior
- complete a tense handoff interaction
- exit with the feeling that the protagonist is now inside the family world

