export type AnalyticsAccessHistoryPoint = {
  date: string
  pages: number
  sessions: number
  loadTime: number
}

export type AnalyticsVisitsResponseDto = {
  views: number
  visitors: number
  loadTime: number
  accessHistory: AnalyticsAccessHistoryPoint[]
}

export type AnalyticsBarItemDto = {
  key: string
  href: string | null
  titleOne: string
  titleTwo: string
  count: number
  percent: number | null
}

export type AnalyticsLiveUserDto = {
  fullname: string
  userId: number
  sessionTime: string
  pageTime: string
  href: string
  accessDateTime: string
  updateTime: string
  pages: number
  sessionId: string
}

export type AnalyticsLiveUsersResponseDto = {
  activeUsers: number
  items: AnalyticsLiveUserDto[]
}

export type AnalyticsTraceDto = {
  id: number
  userId: number
  userName: string
  level: number | null
  message: string
  logger: string
  logDateTime: string | null
  handled: number | null
  userAgent: string
  referer: string
}

export type AnalyticsErrorDto = {
  id: number
  userId: number
  userName: string
  statusCode: number | null
  message: string
  trace: string
  logDateTime: string | null
  handled: number | null
  userAgent: string
  referrer: string
}

export type AnalyticsTraceListResponseDto = {
  pages: number
  currentPage: number
  totalCount: number
  unresolvedCount: number
  items: AnalyticsTraceDto[]
}

export type AnalyticsErrorListResponseDto = {
  pages: number
  currentPage: number
  totalCount: number
  unresolvedCount: number
  items: AnalyticsErrorDto[]
}

export type AnalyticsQueryFilters = {
  startAt: number
  endAt: number
  userId?: number
  groupId?: number
}

export type AnalyticsLogFilters = AnalyticsQueryFilters & {
  page: number
}

import type { AnalyticsRangeId } from "./date-ranges"

export type AnalyticsPageFilters = {
  rangeId: AnalyticsRangeId
  userId?: number
  groupId?: number
  tracePage: number
  errorPage: number
}

export type AnalyticsDashboardData = {
  visits: AnalyticsVisitsResponseDto | null
  browsers: AnalyticsBarItemDto[]
  os: AnalyticsBarItemDto[]
  resolution: AnalyticsBarItemDto[]
  users: AnalyticsBarItemDto[]
  loadTimes: AnalyticsBarItemDto[]
  liveUsers: AnalyticsLiveUsersResponseDto | null
  traces: AnalyticsTraceListResponseDto | null
  errors: AnalyticsErrorListResponseDto | null
}

/** Client beacon payload (camelCase), matches legacy tracker.js and AnalyticsBeaconRequest. */
export type AnalyticsBeaconPayload = {
  language: string
  userAgent: string
  host: string
  hostname: string
  href: string
  protocol: string
  search: string
  pathname: string
  screenHeight: string
  screenWidth: string
  origin: string
  referrer: string
  loadTime: string
  zoom: number
  sessionId: string
  pageId: string
  pageTime: number
}

export type AnalyticsTraceEntryRequest = {
  l: number
  m: string
  n: string
}

export type AnalyticsTraceIngestRequest = {
  lg: AnalyticsTraceEntryRequest[]
}
