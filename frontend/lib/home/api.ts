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
import { mapUserStarsPayloadToPanel, type UserStarsPayload } from "@/lib/home/stars-mapper"
import { getProfileRunList } from "@/lib/profile/api"
import type { UserSharedObjects } from "@/lib/users/types"

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
  const [starsResult, sharedResult] = await Promise.all([
    authorizedGet<UserStarsPayload>(`/api/users/${userId}/stars`),
    authorizedGet<UserSharedObjects>("/api/users/me/shared-objects"),
  ])

  if (!starsResult.data) return { data: null, error: starsResult.error }

  const sharedWithMe =
    sharedResult.data?.sharedToMe.map((item) => ({
      id: item.id,
      name: item.name?.trim() || `Shared item ${item.id}`,
      href: item.url ?? undefined,
      sharedFrom: item.sharedFrom ?? undefined,
      shareDate: item.shareDate ?? undefined,
    })) ?? []

  return {
    data: mapUserStarsPayloadToPanel(starsResult.data, sharedWithMe),
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
