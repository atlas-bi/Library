# Homepage Parity Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Rebuild the Next.js homepage so it matches the C# homepage interaction model with lazy in-page tabs for Stars, Subscriptions, Report Runs, and Groups.

**Architecture:** Render a server shell at `/` and lazy-load tab panels through internal API routes. Normalize backend responses into focused panel models and render them with reusable homepage-specific components that mirror the current C# structure.

**Tech Stack:** Next.js App Router, React 19, TypeScript, Vitest, Testing Library, Biome

---

### Task 1: Homepage data contracts

**Files:**
- Create: `lib/home/types.ts`
- Create: `lib/home/constants.ts`
- Test: `lib/home/constants.test.ts`

- [ ] Define homepage tab ids and panel response types.
- [ ] Add tests that lock the tab order to `stars`, `subscriptions`, `report-runs`, `groups`.
- [ ] Verify the tests fail before implementation, then pass after adding constants and types.

### Task 2: Homepage tab data fetchers

**Files:**
- Create: `lib/home/api.ts`
- Modify: `lib/profile/api.ts`
- Test: `lib/home/api.test.ts`

- [ ] Add thin server helpers that fetch lazy tab data from the existing backend-facing utilities.
- [ ] Introduce a new groups fetcher and any small normalization helpers needed for subscriptions and run lists.
- [ ] Add tests for error and empty-state normalization.

### Task 3: Lazy tab API routes

**Files:**
- Create: `app/api/home/stars/route.ts`
- Create: `app/api/home/subscriptions/route.ts`
- Create: `app/api/home/report-runs/route.ts`
- Create: `app/api/home/groups/route.ts`
- Test: `app/api/home/home-routes.test.ts`

- [ ] Write route tests that assert each endpoint returns JSON payloads or auth errors.
- [ ] Implement the route handlers with the homepage fetchers.
- [ ] Re-run the route tests until green.

### Task 4: Homepage shell and lazy tab client

**Files:**
- Create: `components/home/home-shell.tsx`
- Create: `components/home/home-tabs-client.tsx`
- Create: `components/home/home-tab-panel.tsx`
- Modify: `app/page.tsx`
- Test: `components/home/home-tabs-client.test.tsx`

- [ ] Write a failing client rendering test for default tab selection, loading state, and one-fetch-per-tab caching.
- [ ] Implement the shell and client tab logic.
- [ ] Re-run the test and keep the implementation minimal until it passes.

### Task 5: Homepage panel components and styling

**Files:**
- Create: `components/home/home-stars-panel.tsx`
- Create: `components/home/home-subscriptions-panel.tsx`
- Create: `components/home/home-run-list-panel.tsx`
- Create: `components/home/home-groups-panel.tsx`
- Modify: `app/globals.css`
- Test: `components/home/home-panels.test.tsx`

- [ ] Add rendering tests for panel empty states and representative populated states.
- [ ] Implement the parity-focused panel components and homepage-specific CSS hooks.
- [ ] Re-run the panel tests until green.

### Task 6: Final verification

**Files:**
- Modify: `app/page.tsx`
- Modify: `components/home/*`
- Modify: `lib/home/*`

- [ ] Run `pnpm test`.
- [ ] Run `pnpm typecheck`.
- [ ] Run `pnpm lint`.
- [ ] Fix any failures and rerun until all three commands succeed.
