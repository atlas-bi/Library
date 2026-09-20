import { mapUserStarsPayloadToPanel, type UserStarsPayload } from "@/lib/home/stars-mapper"
import type {
  HomeGroupsPanel,
  HomeRunListPanel,
  HomeStarsPanel,
  HomeSubscriptionsPanel,
  HomeTabId,
} from "@/lib/home/types"

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
          : mapUserStarsPayloadToPanel(panelPayload as UserStarsPayload),
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
