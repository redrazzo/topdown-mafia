# Mafia Topdown Full Game Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Turn the current vertical-slice prototype into a compact, shippable Mafia-style premium indie crime game.

**Architecture:** Continue using Unity as the production base, but move from one-off prototype wiring into a milestone-driven content and systems pipeline. Build outward from the existing first mission by expanding districts, campaign definitions, content authoring, travel, combat pressure, and release hardening in measured slices.

**Tech Stack:** Unity 6, URP, C#, editor-driven scene generation, domain tests, compile-check projects, Windows desktop build pipeline.

---

## Phase A: Planning And Contracts

- [ ] Maintain `.omx/context/`, `.omx/plans/prd-*`, and `.omx/plans/test-spec-*` as the authoritative execution contract.
- [ ] Keep each implementation slice tied to a milestone from the PRD.
- [ ] Re-verify the plan against the PRD before each new milestone.

## Phase B: World Foundation

- [ ] Expand from the current docks + office slice into four authored district scenes.
- [ ] Implement district travel links, spawn points, and save-aware travel.
- [ ] Ensure district assets and scene generation stay aligned.

## Phase C: Campaign Authoring

- [ ] Add additional mission definitions beyond the opening mission.
- [ ] Build chapter sequencing and follow-on unlocks.
- [ ] Make save/load resilient across multi-mission play.

## Phase D: Crime Systems

- [ ] Grow combat beyond a placeholder brawl loop.
- [ ] Add weapon content, enemy archetypes, and police-response embodiment.
- [ ] Keep heat, encounters, and mission states coherent.

## Phase E: Presentation

- [ ] Replace prototype-grade visual rhythms with stronger district identity.
- [ ] Add more dialogue, staging, and period-authored UI behavior.
- [ ] Increase atmosphere in both street and interior spaces.

## Phase F: Release Hardening

- [ ] Add settings, accessibility, and save-slot flow.
- [ ] Run stability, launch, and persistence checks repeatedly.
- [ ] Produce a release-candidate build path.

