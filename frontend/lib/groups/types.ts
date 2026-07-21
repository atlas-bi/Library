export type GroupFeatureFlags = {
  userProfilesEnabled?: boolean
}

export type GroupPermissions = {
  canViewGroups?: boolean
  canViewUserProfiles?: boolean
  canViewSiteAnalytics?: boolean
}

export type GroupListItem = {
  id: number
  name?: string | null
  email?: string | null
  type?: string | null
  source?: string | null
  url?: string | null
}

export type GroupListResponse = {
  features?: GroupFeatureFlags | null
  permissions?: GroupPermissions | null
  items?: GroupListItem[]
}

export type GroupDetail = {
  id: number
  name?: string | null
  email?: string | null
  type?: string | null
  source?: string | null
  features?: GroupFeatureFlags | null
  permissions?: GroupPermissions | null
}

export type GroupUser = {
  id: number
  name?: string | null
  email?: string | null
  phone?: string | null
  epicId?: string | null
  employeeId?: string | null
  canOpenUserProfile?: boolean
  url?: string | null
}

export type GroupReport = {
  id: number
  name?: string | null
  lastUpdated?: string | null
  subscriptionCount?: number
  favoriteCount?: number
  runCount?: number
  url?: string | null
}
