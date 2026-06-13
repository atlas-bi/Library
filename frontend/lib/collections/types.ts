/** Types aligned with Collections API (CollectionsApiController). */

export interface CollectionListItemDto {
  id: number
  name: string
  description?: string | null
  isStarred?: boolean
  starCount?: number
}

export interface CollectionsListResponseDto {
  collections: CollectionListItemDto[]
  total: number
  page: number
  pageSize: number
}

export interface CollectionFeatureFlagsDto {
  termsEnabled?: boolean
  feedbackEnabled?: boolean
  sharingEnabled?: boolean
  [key: string]: unknown
}

export interface CollectionUserSummaryDto {
  id?: number
  username?: string | null
  fullName?: string | null
}

export interface InitiativeSummaryDto {
  id?: number
  name?: string | null
  description?: string | null
}

export interface CollectionTermDto {
  id?: number
  termId?: number
  name?: string | null
  summary?: string | null
  rank?: number
}

export interface CollectionReportDto {
  id: number
  name?: string | null
  description?: string | null
  rank?: number
  isStarred?: boolean
  canRun?: boolean
}

export interface CollectionDetailDto {
  id: number
  name: string
  description?: string | null
  purpose?: string | null
  hidden?: string | null
  lastModified?: string | null
  lastModifiedDisplay?: string | null
  isStarred?: boolean
  starCount?: number
  canCreateCollection?: boolean
  canEditCollection?: boolean
  canDeleteCollection?: boolean
  features?: CollectionFeatureFlagsDto | null
  lastUpdatedBy?: CollectionUserSummaryDto | null
  initiative?: InitiativeSummaryDto | null
  terms?: CollectionTermDto[]
  reports?: CollectionReportDto[]
}

export interface CollectionWriteBody {
  name: string
  description?: string | null
  purpose?: string | null
  hidden: string
  termIds: number[]
  reportIds: number[]
}

export interface CollectionTypeaheadItemDto {
  id: number
  name: string
  description?: string | null
}
