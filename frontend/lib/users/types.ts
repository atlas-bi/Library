export type UserPageUser = {
  id: number
  username?: string | null
  fullName?: string | null
  firstName?: string | null
  displayName?: string | null
  email?: string | null
  department?: string | null
  title?: string | null
  phone?: string | null
  profilePhoto?: string | null
}

export type UserPageViewer = {
  id: number
  isCurrentUser: boolean
  isAdministrator: boolean
  adminEnabled?: string | null
}

export type UserPagePermissions = {
  canViewOtherUsers: boolean
  canViewGroups: boolean
  canViewAnalytics: boolean
  canEditOtherUsers: boolean
  canToggleAdminMode: boolean
  canEditWorkspace: boolean
}

export type UserPageTabs = {
  starsVisible: boolean
  subscriptionsVisible: boolean
  activityVisible: boolean
  runListVisible: boolean
  atlasHistoryVisible: boolean
  groupsVisible: boolean
  analyticsVisible: boolean
}

export type UserPageFeatures = {
  userProfilesEnabled: boolean
}

export type UserPage = {
  user: UserPageUser
  viewer: UserPageViewer
  permissions: UserPagePermissions
  tabs: UserPageTabs
  features: UserPageFeatures
  defaultReportTypeIds: number[]
}

export type UserGroup = {
  id: number
  name?: string | null
  type?: string | null
  source?: string | null
}

export type UserSubscription = {
  reportId?: number | null
  name?: string | null
  emailList?: string | null
  description?: string | null
  lastStatus?: string | null
  lastRun?: string | null
  sentTo?: string | null
}

export type UserHistoryItem = {
  name?: string | null
  type?: string | null
  url?: string | null
  date?: string | null
}

export type UserHistorySection = {
  atlasHistory: UserHistoryItem[]
  reportEdits: UserHistoryItem[]
  initiativeEdits: UserHistoryItem[]
  collectionEdits: UserHistoryItem[]
  termEdits: UserHistoryItem[]
}

export type UserSharedObject = {
  id: number
  name?: string | null
  shareDate?: string | null
  sharedFrom?: string | null
  url?: string | null
}

export type UserSharedObjects = {
  sharedToMe: UserSharedObject[]
  sharedFromMe: UserSharedObject[]
}

export type UserSearchHistoryItem = {
  searchUrl?: string | null
  searchString?: string | null
}

export type UserWorkspacePermissions = {
  canCreateFolders: boolean
  canRenameFolders: boolean
  canDeleteFolders: boolean
  canReorderFolders: boolean
  canReorderFavorites: boolean
  canMoveFavoritesToFolders: boolean
  canToggleFavorites: boolean
}

export type UserWorkspaceSummary = {
  totalCount: number
  unsortedCount: number
  hasFolders: boolean
  showUnsortedBucket: boolean
}

export type UserWorkspaceFilterState = {
  hasReports: boolean
  hasCollections: boolean
  hasInitiatives: boolean
  hasTerms: boolean
  hasUsers: boolean
  hasGroups: boolean
  hasSearches: boolean
  showQuickFilters: boolean
}

export type UserFavoriteFolder = {
  id: number
  name?: string | null
  rank?: number | null
  itemCount: number
  canManage: boolean
  canReorder: boolean
}

export type UserFavoriteTag = {
  name?: string | null
  slug?: string | null
  showInHeader?: boolean
}

export type UserFavoriteItem = {
  starId: number
  type?: string | null
  typeLabel?: string | null
  folderId?: number | null
  rank?: number | null
  itemId?: number | null
  name?: string | null
  description?: string | null
  url?: string | null
  secondaryText?: string | null
  folderName?: string | null
  folderRank?: number | null
  searchString?: string | null
  canReorder: boolean
  isStarred: boolean
  starCount: number
  bodyText?: string | null
  placeholderImageUrl?: string | null
  thumbnailUrl?: string | null
  fullImageUrl?: string | null
  isCertified: boolean
  isApproved: boolean
  canOpenProfile: boolean
  profileTargetId?: string | null
  canShare: boolean
  shareTargetId?: string | null
  shareName?: string | null
  shareType?: string | null
  canRequestAccess: boolean
  requestAccessTargetId?: string | null
  canRun: boolean
  runUrl?: string | null
  opensRunModal: boolean
  runModalTargetId?: string | null
  runDisabledReason?: string | null
  canEditInEditor: boolean
  editUrl?: string | null
  canManageInEditor: boolean
  manageUrl?: string | null
  reportObjectUrl?: string | null
  reportServerPath?: string | null
  sourceServer?: string | null
  epicMasterFile?: string | null
  epicRecordId?: number | null
  epicReportTemplateId?: number | null
  enabledForHyperspace?: string | null
  tags: UserFavoriteTag[]
  relatedCollectionNames: string[]
}

export type UserSuggestedReport = {
  id: number
  name?: string | null
  description?: string | null
  url?: string | null
  type?: string | null
}

export type UserStars = {
  userId: number
  viewerUserId: number
  isCurrentUser: boolean
  canEditWorkspace: boolean
  permissions: UserWorkspacePermissions
  summary: UserWorkspaceSummary
  filters: UserWorkspaceFilterState
  folders: UserFavoriteFolder[]
  items: UserFavoriteItem[]
  suggestedReports: UserSuggestedReport[]
}

export type CreateUserFavoriteFolderRequest = {
  name: string
}

export type UpdateUserFavoriteFolderRequest = {
  name: string
}

export type ReorderUserFavoriteFolderItem = {
  folderId: string
  folderRank: number
}

export type ReorderUserFavoriteItem = {
  favoriteId: string
  favoriteType: string
  favoriteRank: number
}

export type UpdateUserFavoriteFolderAssignmentRequest = {
  favoriteId: number
  favoriteType: string
  folderId?: number | null
}

export type ToggleUserFavoriteRequest = {
  type: string
  id?: number | null
  search?: string | null
}

export type ToggleUserFavoriteResponse = {
  type?: string | null
  id?: number | null
  search?: string | null
  isStarred: boolean
  starCount: number
}

export type ToggleAdminModeResponse = {
  adminEnabled?: string | null
}
