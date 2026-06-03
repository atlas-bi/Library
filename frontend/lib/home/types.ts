export type HomeTabId = "stars" | "subscriptions" | "report-runs" | "groups"

export type HomeTabDefinition = {
  id: HomeTabId
  label: string
}

export type HomeTabsVisibility = Record<HomeTabId, boolean>

export type HomeUserPageSummary = {
  userId: number
  displayName: string
  defaultReportTypeIds: number[]
  visibility: HomeTabsVisibility
}

export type HomeTabRequestContext = {
  userId: number
  defaultReportTypeIds: number[]
}

export type HomePanelStatus = "idle" | "loading" | "ready" | "error"

export type HomePanelBase = {
  title?: string
  emptyMessage?: string
}

export type HomeStarCard = {
  id: number
  href: string
  title: string
  typeLabel: string
  description: string
  thumbnailUrl?: string
  fullImageUrl?: string
  placeholderImageUrl?: string
  tags?: Array<{ name: string; slug?: string; showInHeader?: boolean }>
  isCertified?: boolean
  starCount?: number
  canOpenDetails?: boolean
  isStarred?: boolean
  canRun?: boolean
  runUrl?: string
  runDisabledReason?: string
  canEdit?: boolean
  editUrl?: string
  canManage?: boolean
  manageUrl?: string
  canOpenProfile?: boolean
  canShare?: boolean
  canRequestAccess?: boolean
}

export type HomeStarsPanel = HomePanelBase & {
  kind: "stars"
  folders: Array<{ id: string; label: string; count: number }>
  filters: Array<{ id: string; label: string }>
  cards: HomeStarCard[]
}

export type HomeSubscriptionsPanel = HomePanelBase & {
  kind: "subscriptions"
  rows: Array<{
    id: string
    name: string
    description?: string
    lastStatus?: string
    lastRun?: string
    sentTo?: string
  }>
}

export type HomeRunListPanel = HomePanelBase & {
  kind: "report-runs"
  rows: Array<{
    id: string
    name: string
    type?: string
    href?: string | null
    runs?: number
    lastRun?: string | null
  }>
}

export type HomeGroupsPanel = HomePanelBase & {
  kind: "groups"
  rows: Array<{
    id: string
    name: string
    type?: string
    source?: string
    href?: string
  }>
}

export type HomePanelData =
  | HomeStarsPanel
  | HomeSubscriptionsPanel
  | HomeRunListPanel
  | HomeGroupsPanel

export type HomePanelResponse = { ok: true; data: HomePanelData } | { ok: false; error: string }
