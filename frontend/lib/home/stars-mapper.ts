import type { HomeSharedWithMeItem, HomeStarCard, HomeStarsPanel } from "@/lib/home/types"

export type UserStarsPayload = {
  summary: {
    totalCount: number
    unsortedCount: number
    hasFolders?: boolean
    showUnsortedBucket?: boolean
  }
  filters: {
    hasReports: boolean
    hasCollections: boolean
    hasInitiatives: boolean
    hasTerms: boolean
    hasUsers: boolean
    hasGroups: boolean
    hasSearches: boolean
    showQuickFilters?: boolean
  }
  folders: Array<{
    id: number
    name: string
    itemCount: number
  }>
  items?: Array<{
    starId: number
    type?: string | null
    typeLabel?: string | null
    folderId?: number | null
    rank?: number | null
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
  }> | null
  suggestedReports?: Array<{
    id: number
    name: string
    description?: string | null
    url?: string | null
    type?: string | null
  }> | null
}

export function normalizeHomeStarItemType(type?: string | null, typeLabel?: string | null): string {
  const normalizedType = type?.trim().toLowerCase()
  if (normalizedType) return normalizedType

  const normalizedLabel = typeLabel?.trim().toLowerCase()
  if (normalizedLabel === "collection" || normalizedLabel === "initiative") {
    return normalizedLabel
  }
  if (normalizedLabel === "report" || normalizedLabel === "term" || normalizedLabel === "user") {
    return normalizedLabel
  }
  if (normalizedLabel === "group" || normalizedLabel === "search") {
    return normalizedLabel
  }

  return normalizedLabel ?? "item"
}

function mapStarItem(item: NonNullable<UserStarsPayload["items"]>[number]): HomeStarCard {
  return {
    id: item.itemId ?? item.starId,
    href: item.url || "#",
    title: item.name,
    itemType: normalizeHomeStarItemType(item.type, item.typeLabel),
    folderId: item.folderId ?? null,
    rank: item.rank ?? null,
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

function sortStarCards(cards: HomeStarCard[]): HomeStarCard[] {
  return [...cards].sort((left, right) => {
    const leftRank = left.rank ?? Number.MAX_SAFE_INTEGER
    const rightRank = right.rank ?? Number.MAX_SAFE_INTEGER
    if (leftRank !== rightRank) return leftRank - rightRank
    return left.title.localeCompare(right.title)
  })
}

export function mapUserStarsPayloadToPanel(
  dto: UserStarsPayload,
  sharedWithMe: HomeSharedWithMeItem[] = [],
): HomeStarsPanel {
  const items = dto.items ?? []
  const suggestedReports = dto.suggestedReports ?? []

  const cards = sortStarCards(
    items.length > 0
      ? items.map(mapStarItem)
      : suggestedReports.map((item) => ({
          id: item.id,
          href: item.url || "#",
          title: item.name,
          itemType: normalizeHomeStarItemType(item.type, item.type),
          folderId: null,
          rank: null,
          typeLabel: item.type || "Report",
          description: item.description || "Open to view details.",
          starCount: 0,
          canOpenDetails: Boolean(item.url),
          isStarred: false,
        })),
  )

  const isSuggestionFallback = items.length === 0 && suggestedReports.length > 0
  const favoriteTypeCount = [
    dto.filters.hasReports,
    dto.filters.hasCollections,
    dto.filters.hasInitiatives,
    dto.filters.hasTerms,
    dto.filters.hasUsers,
    dto.filters.hasGroups,
    dto.filters.hasSearches,
  ].filter(Boolean).length
  const showTypeQuickFilters = dto.filters.showQuickFilters ?? favoriteTypeCount > 1

  const filters = showTypeQuickFilters
    ? ([
        dto.filters.hasReports ? { id: "report", label: "Reports" } : null,
        dto.filters.hasCollections ? { id: "collection", label: "Collections" } : null,
        dto.filters.hasInitiatives ? { id: "initiative", label: "Initiatives" } : null,
        dto.filters.hasTerms ? { id: "term", label: "Terms" } : null,
        dto.filters.hasUsers ? { id: "user", label: "Users" } : null,
        dto.filters.hasGroups ? { id: "group", label: "Groups" } : null,
        dto.filters.hasSearches ? { id: "search", label: "Searches" } : null,
      ].filter(Boolean) as HomeStarsPanel["filters"])
    : []

  const showUnsortedBucket =
    dto.summary.showUnsortedBucket ??
    (Boolean(dto.summary.hasFolders) &&
      dto.folders.length > 0 &&
      dto.summary.unsortedCount > 0)

  return {
    kind: "stars",
    title: "Stars",
    emptyMessage: "You don't have any favorites! Search to get started.",
    isSuggestionFallback,
    suggestionHeading: isSuggestionFallback
      ? "You don't have any favorites! Here's some reports you've used."
      : undefined,
    folders: [
      { id: "all", label: "All", count: dto.summary.totalCount },
      ...(showUnsortedBucket
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
