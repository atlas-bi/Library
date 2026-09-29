import type { HomeSharedWithMeItem, HomeStarCard, HomeStarsPanel } from "@/lib/home/types"

export type UserStarsPayload = {
  summary: {
    totalCount: number
    unsortedCount: number
  }
  filters: {
    hasReports: boolean
    hasCollections: boolean
    hasInitiatives: boolean
    hasTerms: boolean
    hasUsers: boolean
    hasGroups: boolean
    hasSearches: boolean
  }
  folders: Array<{
    id: number
    name: string
    itemCount: number
  }>
  items: Array<{
    starId: number
    type?: string | null
    typeLabel?: string | null
    folderId?: number | null
    itemId?: number | null
    url?: string | null
    name: string
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
}

function mapStarItem(item: UserStarsPayload["items"][number]): HomeStarCard {
  return {
    id: item.itemId ?? item.starId,
    href: item.url || "#",
    title: item.name,
    itemType: item.type ?? item.typeLabel?.toLowerCase() ?? "item",
    folderId: item.folderId ?? null,
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
  }
}

export function mapUserStarsPayloadToPanel(
  dto: UserStarsPayload,
  sharedWithMe: HomeSharedWithMeItem[] = [],
): HomeStarsPanel {
  const cards =
    dto.items.length > 0
      ? dto.items.map(mapStarItem)
      : dto.suggestedReports.map((item) => ({
          id: item.id,
          href: item.url || "#",
          title: item.name,
          itemType: item.type?.toLowerCase() ?? "report",
          folderId: null,
          typeLabel: item.type || "Report",
          description: item.description || "Open to view details.",
          starCount: 0,
          canOpenDetails: Boolean(item.url),
          isStarred: false,
        }))

  const isSuggestionFallback = dto.items.length === 0 && dto.suggestedReports.length > 0

  const filters = [
    dto.filters.hasReports ? { id: "report", label: "Reports" } : null,
    dto.filters.hasCollections ? { id: "collection", label: "Collections" } : null,
    dto.filters.hasInitiatives ? { id: "initiative", label: "Initiatives" } : null,
    dto.filters.hasTerms ? { id: "term", label: "Terms" } : null,
    dto.filters.hasUsers ? { id: "user", label: "Users" } : null,
    dto.filters.hasGroups ? { id: "group", label: "Groups" } : null,
    dto.filters.hasSearches ? { id: "search", label: "Searches" } : null,
  ].filter(Boolean) as HomeStarsPanel["filters"]

  return {
    kind: "stars",
    title: "Stars",
    emptyMessage: "You don't have any favorites! Search to get started.",
    isSuggestionFallback,
    suggestionHeading: isSuggestionFallback
      ? "You don't have any favorites! Here's some reports you've used."
      : undefined,
    folders: [
      { id: "all", label: "All", count: cards.length },
      ...(dto.summary.unsortedCount > 0
        ? [{ id: "unsorted", label: "Unsorted", count: dto.summary.unsortedCount }]
        : []),
      ...dto.folders.map((folder) => ({
        id: String(folder.id),
        label: folder.name,
        count: folder.itemCount,
      })),
    ],
    filters,
    cards,
    sharedWithMe,
  }
}
