# Homepage Parity Design

## Goal

Re-implement the Next.js homepage so it follows the current C# homepage structure rather than the existing simplified approximation.

## Scope

- Keep the homepage as a single route at `/`.
- Keep in-page tab switching for `Stars`, `Subscriptions`, `Report Runs`, and `Groups`.
- Load tab content lazily after interaction, mirroring the C# `data-ajax` pattern.
- Use the existing API as the source of truth for authorization and available actions.
- Match the C# homepage layout, spacing, typography, icon scale, and card structure closely enough that the page reads as the same product.

## Non-Goals

- Re-implement Razor-specific permission checks in Next.js.
- Change the report run behavior.
- Rebuild unrelated pages to full parity in the same change.

## Architecture

The homepage becomes a server-rendered shell with a client-side tab controller. The server shell renders the greeting, top navigation, and tab list. Each tab panel is fetched lazily from a dedicated internal Next.js route that composes permission-aware data from the existing backend APIs.

The default `Stars` tab is selected on page load but not pre-expanded with the old simplified UI. Instead, the tab system owns loading and rendering of the panel content. Reusable presentation components render the homepage cards and table panels so that `Stars`, `Subscriptions`, `Report Runs`, and `Groups` remain isolated and easy to evolve.

## Data Flow

1. `app/page.tsx` fetches the current user and renders the homepage shell.
2. A client tab component switches tabs without route navigation.
3. On first activation of a tab, the client fetches `/api/home/<tab>`.
4. The route handler calls existing server helpers or new thin wrappers around the C# API.
5. The client renders the returned JSON into parity-focused panel components.

## Tab Mapping

- `Stars`: new homepage-focused server helper using `/api/reports` for initial favorites fallback, with a structure that supports folder rail and richer cards.
- `Subscriptions`: use existing profile/user-oriented data fetch support where possible, normalize into a table-like panel.
- `Report Runs`: use existing profile run list support.
- `Groups`: add a thin user/groups fetch wrapper against the C# API and render a simple sortable table-style panel.

## Authorization Strategy

Authorization remains API-driven. Next.js should use returned data and returned URLs to decide whether to show links, disabled controls, or empty states. The UI may gate obvious shell actions on auth presence, but it should not duplicate backend permission rules.

## Testing

- Add unit tests for tab metadata and panel normalization helpers.
- Add rendering tests for the lazy tab shell and loading behavior.
- Verify the new homepage route compiles, lints, and passes test coverage for the introduced logic.
