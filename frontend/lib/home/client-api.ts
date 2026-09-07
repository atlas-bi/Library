import type {
  HomeGroupsPanel,
  HomeRunListPanel,
  HomeStarsPanel,
  HomeSubscriptionsPanel,
  HomeTabId,
} from "@/lib/home/types"

function normalizeStarsPanel(payload: {
  summary: { totalCount: number; unsortedCount: number }
  filters: {
    hasReports: boolean
    hasCollections: boolean
    hasInitiatives: boolean
    hasTerms: boolean
    hasUsers: boolean
    hasGroups: boolean
    hasSearches: boolean
  }
  folders: Array<{ id: number; name: string; itemCount: number }>
  items: Array<{
    starId: number
    itemId?: number | null
    url?: string | null
    name: string
    typeLabel?: string | null
    description?: string | null
    bodyText?: string | null
    placeholderImageUrl?: string | null
    thumbnailUrl?: string | null
    fullImageUrl?: string | null
    isCertified?: boolean
    isStarred?: boolean
    starCount?: number
    canRun?: boolean
    runUrl?: string | null
    runDisabledReason?: string | null
    canEditInEditor?: boolean
    editUrl?: string | null
    canManageInEditor?: boolean
    manageUrl?: string | null
    canOpenProfile?: boolean
    canShare?: boolean
    canRequestAccess?: boolean
    tags?: Array<{ name: string; slug?: string | null; showInHeader?: boolean }>
  }>
  suggestedReports: Array<{
    id: number
    name: string
    description?: string | null
    url?: string | null
    type?: string | null
  }>
}): HomeStarsPanel {
  const isSuggestionFallback = payload.items.length === 0 && payload.suggestedReports.length > 0
  const cards =
    payload.items.length > 0
      ? payload.items.map((item) => ({
          id: item.itemId ?? item.starId,
          href: item.url || "#",
          title: item.name,
          typeLabel: item.typeLabel || "Item",
          description: item.bodyText || item.description || "Open to view details.",
          thumbnailUrl: item.thumbnailUrl || undefined,
          fullImageUrl: item.fullImageUrl || undefined,
          placeholderImageUrl: item.placeholderImageUrl || undefined,
          tags:
            item.tags
              ?.filter((tag) => tag.showInHeader)
              .map((tag) => ({
                name: tag.name,
                slug: tag.slug || undefined,
                showInHeader: tag.showInHeader,
              })) ?? [],
          isCertified: item.isCertified ?? false,
          starCount: item.starCount ?? 0,
          canOpenDetails: Boolean(item.url),
          isStarred: item.isStarred ?? true,
          canRun: item.canRun ?? false,
          runUrl: item.runUrl || undefined,
          runDisabledReason: item.runDisabledReason || undefined,
          canEdit: item.canEditInEditor ?? false,
          editUrl: item.editUrl || undefined,
          canManage: item.canManageInEditor ?? false,
          manageUrl: item.manageUrl || undefined,
          canOpenProfile: item.canOpenProfile ?? false,
          canShare: item.canShare ?? false,
          canRequestAccess: item.canRequestAccess ?? false,
        }))
      : payload.suggestedReports.map((item) => ({
          id: item.id,
          href: item.url || "#",
          title: item.name,
          typeLabel: item.type || "Report",
          description: item.description || "Open to view details.",
          starCount: 0,
          canOpenDetails: Boolean(item.url),
          isStarred: false,
        }))

  return {
    kind: "stars",
    title: "Stars",
    emptyMessage: "You don't have any favorites! Search to get started.",
    isSuggestionFallback,
    suggestionHeading: isSuggestionFallback
      ? "You don't have any favorites! Here's some reports you've used."
      : undefined,
    folders: [
      { id: "all", label: "All", count: payload.summary.totalCount },
      ...(payload.summary.unsortedCount > 0
        ? [{ id: "unsorted", label: "Unsorted", count: payload.summary.unsortedCount }]
        : []),
      ...payload.folders.map((folder) => ({
        id: String(folder.id),
        label: folder.name,
        count: folder.itemCount,
      })),
    ],
    filters: [
      payload.filters.hasReports ? { id: "reports", label: "Reports" } : null,
      payload.filters.hasCollections ? { id: "collections", label: "Collections" } : null,
      payload.filters.hasInitiatives ? { id: "initiatives", label: "Initiatives" } : null,
      payload.filters.hasTerms ? { id: "terms", label: "Terms" } : null,
      payload.filters.hasUsers ? { id: "users", label: "Users" } : null,
      payload.filters.hasGroups ? { id: "groups", label: "Groups" } : null,
      payload.filters.hasSearches ? { id: "searches", label: "Searches" } : null,
    ].filter(Boolean) as HomeStarsPanel["filters"],
    cards,
  }
}

function isHomeStarsPanel(payload: unknown): payload is HomeStarsPanel {
  if (!payload || typeof payload !== "object") return false
  const panel = payload as Partial<HomeStarsPanel>
  return (
    panel.kind === "stars" &&
    Array.isArray(panel.folders) &&
    Array.isArray(panel.filters) &&
    Array.isArray(panel.cards)
  )
}

function normalizeSubscriptionsPanel(
  payload: Array<{
    reportId?: number | null
    name: string
    description?: string | null
    lastStatus?: string | null
    lastRun?: string | null
    sentTo?: string | null
  }>,
): HomeSubscriptionsPanel {
  return {
    kind: "subscriptions",
    title: "Subscriptions",
    emptyMessage: "No subscriptions to show.",
    rows: payload.map((item, index) => ({
      id: String(item.reportId ?? index),
      name: item.name,
      description: item.description || undefined,
      lastStatus: item.lastStatus || undefined,
      lastRun: item.lastRun || undefined,
      sentTo: item.sentTo || undefined,
    })),
  }
}

function normalizeGroupsPanel(
  payload: Array<{
    id: number
    name: string
    type?: string | null
    source?: string | null
  }>,
): HomeGroupsPanel {
  return {
    kind: "groups",
    title: "Groups",
    emptyMessage: "No groups to show.",
    rows: payload.map((item) => ({
      id: String(item.id),
      name: item.name,
      type: item.type || undefined,
      source: item.source || undefined,
      href: `/groups?id=${item.id}`,
    })),
  }
}

function normalizeRunListPanel(
  payload: Array<{
    name: string
    type?: string | null
    url?: string | null
    runs?: number
    lastRun?: string | null
  }>,
): HomeRunListPanel {
  return {
    kind: "report-runs",
    title: "Report Runs",
    emptyMessage: "No run data to show.",
    rows: payload.map((item, index) => ({
      id: `${item.name}-${index}`,
      name: item.name,
      type: item.type || undefined,
      href: item.url || undefined,
      runs: item.runs,
      lastRun: item.lastRun || undefined,
    })),
  }
}

export async function fetchHomeTabPanel(tabId: HomeTabId) {
  const response = await fetch(`/api/home/${tabId}`, {
    credentials: "include",
  })

  if (!response.ok) {
    return { ok: false as const, error: `http_${response.status}` }
  }

  const payload = (await response.json()) as { ok?: boolean; data?: unknown }
  const panelPayload = payload && "data" in payload ? payload.data : payload

  switch (tabId) {
    case "stars":
      return {
        ok: true as const,
        data: isHomeStarsPanel(panelPayload)
          ? panelPayload
          : normalizeStarsPanel(panelPayload as Parameters<typeof normalizeStarsPanel>[0]),
      }
    case "subscriptions":
      return {
        ok: true as const,
        data: normalizeSubscriptionsPanel(
          panelPayload as Parameters<typeof normalizeSubscriptionsPanel>[0],
        ),
      }
    case "groups":
      return {
        ok: true as const,
        data: normalizeGroupsPanel(panelPayload as Parameters<typeof normalizeGroupsPanel>[0]),
      }
    case "report-runs":
      return {
        ok: true as const,
        data: normalizeRunListPanel(panelPayload as Parameters<typeof normalizeRunListPanel>[0]),
      }
  }
}
