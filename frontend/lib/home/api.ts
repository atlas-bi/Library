import { getServerApiBase } from "@/lib/api-base"
import type { AppErrorCode } from "@/lib/app-error"
import { getToken } from "@/lib/auth"
import type {
  HomeGroupsPanel,
  HomeRunListPanel,
  HomeStarsPanel,
  HomeSubscriptionsPanel,
  HomeTabId,
  HomeTabsVisibility,
  HomeUserPageSummary,
} from "@/lib/home/types"
import { apiFetchJson } from "@/lib/http"
import { getProfileRunList } from "@/lib/profile/api"

type HomeResult<T> = {
  data: T | null
  error: AppErrorCode | null
}

type UserPageDto = {
  user: {
    id: number
    username: string
    fullName: string
    firstName: string
    displayName: string
  }
  tabs: {
    starsVisible: boolean
    subscriptionsVisible: boolean
    runListVisible: boolean
    groupsVisible: boolean
  }
  defaultReportTypeIds: number[]
}

type UserStarsDto = {
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
    showQuickFilters: boolean
  }
  folders: Array<{
    id: number
    name: string
    itemCount: number
  }>
  items: Array<{
    starId: number
    itemId?: number | null
    url?: string | null
    name: string
    typeLabel?: string | null
    description?: string | null
    bodyText?: string | null
    starCount?: number
  }>
  suggestedReports: Array<{
    id: number
    name: string
    description?: string | null
    url?: string | null
    type?: string | null
  }>
}

type UserSubscriptionDto = {
  reportId?: number | null
  name: string
  description?: string | null
  lastStatus?: string | null
  lastRun?: string | null
  sentTo?: string | null
}

type UserGroupDto = {
  id: number
  name: string
  type?: string | null
  source?: string | null
}

async function authorizedGet<T>(path: string): Promise<HomeResult<T>> {
  const token = await getToken()
  if (!token) return { data: null, error: "auth_required" }

  const apiBase = getServerApiBase()
  if (!apiBase) return { data: null, error: "service_unavailable" }

  const result = await apiFetchJson<T>(`${apiBase}${path}`, {
    headers: { Authorization: `Bearer ${token}` },
    cache: "no-store",
  })

  if (!result.ok) return { data: null, error: result.error.code }
  return { data: result.data, error: null }
}

export async function getHomeUserPageSummary(
  userId: number,
): Promise<HomeResult<HomeUserPageSummary>> {
  const result = await authorizedGet<UserPageDto>(`/api/users/${userId}`)
  if (!result.data) return { data: null, error: result.error }

  const dto = result.data
  const visibility: HomeTabsVisibility = {
    stars: dto.tabs.starsVisible,
    subscriptions: dto.tabs.subscriptionsVisible,
    "report-runs": dto.tabs.runListVisible,
    groups: dto.tabs.groupsVisible,
  }

  return {
    data: {
      userId: dto.user.id,
      displayName:
        dto.user.fullName || dto.user.displayName || dto.user.firstName || dto.user.username,
      defaultReportTypeIds: dto.defaultReportTypeIds ?? [],
      visibility,
    },
    error: null,
  }
}

export async function getHomeStarsPanel(userId: number): Promise<HomeResult<HomeStarsPanel>> {
  const result = await authorizedGet<UserStarsDto>(`/api/users/${userId}/stars`)
  if (!result.data) return { data: null, error: result.error }

  const dto = result.data
  const cards = dto.items.map((item) => ({
    id: item.itemId ?? item.starId,
    href: item.url || "#",
    title: item.name,
    typeLabel: item.typeLabel || "Item",
    description: item.bodyText || item.description || "Open to view details.",
    starCount: item.starCount ?? 0,
    canOpenDetails: Boolean(item.url),
  }))

  const fallbackCards =
    cards.length > 0
      ? cards
      : dto.suggestedReports.map((item) => ({
          id: item.id,
          href: item.url || "#",
          title: item.name,
          typeLabel: item.type || "Report",
          description: item.description || "Open to view details.",
          starCount: 0,
          canOpenDetails: Boolean(item.url),
        }))

  const filters = [
    dto.filters.hasReports ? { id: "reports", label: "Reports" } : null,
    dto.filters.hasCollections ? { id: "collections", label: "Collections" } : null,
    dto.filters.hasInitiatives ? { id: "initiatives", label: "Initiatives" } : null,
    dto.filters.hasTerms ? { id: "terms", label: "Terms" } : null,
    dto.filters.hasUsers ? { id: "users", label: "Users" } : null,
    dto.filters.hasGroups ? { id: "groups", label: "Groups" } : null,
    dto.filters.hasSearches ? { id: "searches", label: "Searches" } : null,
  ].filter(Boolean) as HomeStarsPanel["filters"]

  return {
    data: {
      kind: "stars",
      title: "Stars",
      emptyMessage: "You don't have any favorites! Search to get started.",
      folders: [
        { id: "all", label: "All", count: dto.summary.totalCount },
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
      cards: fallbackCards,
    },
    error: null,
  }
}

export async function getHomeSubscriptionsPanel(
  userId: number,
): Promise<HomeResult<HomeSubscriptionsPanel>> {
  const result = await authorizedGet<UserSubscriptionDto[]>(`/api/users/${userId}/subscriptions`)
  if (!result.data) return { data: null, error: result.error }

  return {
    data: {
      kind: "subscriptions",
      title: "Subscriptions",
      emptyMessage: "No subscriptions to show.",
      rows: result.data.map((item, index) => ({
        id: String(item.reportId ?? index),
        name: item.name,
        description: item.description || undefined,
        lastStatus: item.lastStatus || undefined,
        lastRun: item.lastRun || undefined,
        sentTo: item.sentTo || undefined,
      })),
    },
    error: null,
  }
}

export async function getHomeRunListPanel(
  userId: number,
  reportTypeIds: number[],
): Promise<HomeResult<HomeRunListPanel>> {
  const result = await getProfileRunList({
    id: userId,
    type: "user",
    reportType: reportTypeIds,
  })

  if (!result.data) return { data: null, error: result.error }

  return {
    data: {
      kind: "report-runs",
      title: "Report Runs",
      emptyMessage: "No report runs to show.",
      rows: result.data.map((item, index) => ({
        id: `${item.name}-${index}`,
        name: item.name,
        type: item.type || undefined,
        href: item.url || undefined,
        runs: item.runs,
        lastRun: item.lastRun || undefined,
      })),
    },
    error: null,
  }
}

export async function getHomeGroupsPanel(userId: number): Promise<HomeResult<HomeGroupsPanel>> {
  const result = await authorizedGet<UserGroupDto[]>(`/api/users/${userId}/groups`)
  if (!result.data) return { data: null, error: result.error }

  return {
    data: {
      kind: "groups",
      title: "Groups",
      emptyMessage: "No groups to show.",
      rows: result.data.map((item) => ({
        id: String(item.id),
        name: item.name,
        type: item.type || undefined,
        source: item.source || undefined,
        href: `/groups?id=${item.id}`,
      })),
    },
    error: null,
  }
}

export async function getHomeTabPanel(userId: number, reportTypeIds: number[], tabId: HomeTabId) {
  switch (tabId) {
    case "stars":
      return getHomeStarsPanel(userId)
    case "subscriptions":
      return getHomeSubscriptionsPanel(userId)
    case "report-runs":
      return getHomeRunListPanel(userId, reportTypeIds)
    case "groups":
      return getHomeGroupsPanel(userId)
  }
}
