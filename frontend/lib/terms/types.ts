export interface TermFeaturesDto {
  userProfilesEnabled?: boolean
  sharingEnabled?: boolean
  feedbackEnabled?: boolean
}

export interface TermPermissionsDto {
  canCreateTerm?: boolean
  canApproveTerm?: boolean
  canEditTerm?: boolean
  canDeleteTerm?: boolean
  canViewUserProfiles?: boolean
}

export interface TermUserSummaryDto {
  id?: number
  username?: string | null
  fullName?: string | null
  email?: string | null
}

export interface TermListItemDto {
  id: number
  name: string
  summary?: string | null
  technicalDefinition?: string | null
  url?: string | null
  isApproved?: boolean
  isStarred?: boolean
  starCount?: number
  bodyText?: string | null
}

export interface TermsListDto {
  features?: TermFeaturesDto | null
  permissions?: TermPermissionsDto | null
  items: TermListItemDto[]
}

export interface TermDetailDto {
  id: number
  name: string
  summary?: string | null
  technicalDefinition?: string | null
  isApproved?: boolean
  approvedYn?: string | null
  approvalDateDisplay?: string | null
  lastUpdatedDisplay?: string | null
  isStarred?: boolean
  starCount?: number
  features?: TermFeaturesDto | null
  permissions?: TermPermissionsDto | null
  approvedBy?: TermUserSummaryDto | null
  lastUpdatedBy?: TermUserSummaryDto | null
}

export interface TermRelatedReportDto {
  id: number
  name: string
  description?: string | null
  bodyText?: string | null
  type?: string | null
  url?: string | null
  attachmentCount?: number
  canRun?: boolean
  isStarred?: boolean
  starCount?: number
  isCertified?: boolean
}

export interface TermWriteBody {
  name: string
  summary?: string | null
  technicalDefinition?: string | null
  approvedYn?: string | null
}