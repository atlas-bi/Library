export interface InitiativeListItemDto {
  id: number
  name: string
  description?: string | null
  isStarred?: boolean
  starCount?: number
}

export interface InitiativesListResponseDto {
  initiatives: InitiativeListItemDto[]
  total: number
  page: number
  pageSize: number
}

export interface InitiativeFeatureFlagsDto {
  userProfilesEnabled?: boolean
  feedbackEnabled?: boolean
  sharingEnabled?: boolean
  [key: string]: unknown
}

export interface InitiativeUserSummaryDto {
  id?: number
  username?: string | null
  fullName?: string | null
}

export interface InitiativeCollectionSummaryDto {
  id: number
  name: string
  description?: string | null
  isStarred?: boolean
  starCount?: number
}

export interface InitiativeDetailDto {
  id: number
  name: string
  description?: string | null
  purpose?: string | null
  lastModified?: string | null
  lastModifiedDisplay?: string | null
  isStarred?: boolean
  starCount?: number
  canCreateInitiative?: boolean
  canEditInitiative?: boolean
  canDeleteInitiative?: boolean
  features?: InitiativeFeatureFlagsDto | null
  lastUpdatedBy?: InitiativeUserSummaryDto | null
  collections?: InitiativeCollectionSummaryDto[]
  operationOwner?: InitiativeUserSummaryDto | null
  executiveOwner?: InitiativeUserSummaryDto | null
  financialImpact?: string | null
  strategicImportance?: string | null
  hidden?: string | null
}

export interface InitiativeWriteBody {
  name: string
  description?: string | null
  purpose?: string | null
  collectionIds: number[]
  operationOwnerId?: number | null
  executiveOwnerId?: number | null
  financialImpact?: string | null
  strategicImportance?: string | null
}

export interface InitiativeCollectionTypeaheadItemDto {
  id: number
  name: string
  description?: string | null
}
