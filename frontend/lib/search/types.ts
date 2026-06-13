export const SEARCH_TYPES = [
  "query",
  "reports",
  "terms",
  "collections",
  "initiatives",
  "users",
  "groups",
] as const

export type SearchType = (typeof SEARCH_TYPES)[number]

export interface SearchResultDto {
  id: string
  atlasId: number
  type: "reports" | "collections" | "terms" | "initiatives" | "users" | "groups" | "external"
  name: string
  description: string | null
  url: string | null
  reportType: string | null
  email: string | null
  epicMasterFile: string | null
  epicRecordId: string | null
  epicTemplateId: string | null
  reportServerPath: string | null
  executiveVisibility: string | null
  sourceServer: string | null
  groupType: string | null
  isStarred: boolean | null
  certifications: string[]
  documented: string | null
}

export interface FacetValueDto {
  value: string
  count: number
}

export interface FacetDto {
  key: string
  values: FacetValueDto[]
}

export interface HighlightFieldDto {
  field: string
  snippet: string
}

export interface HighlightDto {
  id: string
  fields: HighlightFieldDto[]
}

export interface FilterFieldDto {
  key: string
  label: string
}

export interface SearchResponseDto {
  results: SearchResultDto[]
  facets: FacetDto[]
  highlights: HighlightDto[]
  filterFields: FilterFieldDto[]
  total: number
  page: number
  pageSize: number
  qTime: number
  isAdvancedSearch: boolean
}
