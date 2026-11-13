export interface ProfileFilters {
  id: number
  type: string
  start_at?: number
  end_at?: number
  server?: string[]
  database?: string[]
  masterFile?: string[]
  visible?: string[]
  certification?: string[]
  availability?: string[]
  reportType?: number[]
}

export interface ProfileChartHistoryPoint {
  date: string
  runs: number
  users: number
  runTime: number
}

export interface ProfileChartResponseDto {
  runs: number
  users: number
  runTime: number
  history: ProfileChartHistoryPoint[]
}

export interface ProfileBarItemDto {
  key: string
  count: number
  percent: number
  href?: string | null
  title?: string | null
  titleOne?: string | null
  titleTwo?: string | null
  date?: string | null
  dateTitle?: string | null
  dateLabel?: string | null
  [key: string]: unknown
}

export interface ProfileRunListItemDto {
  name: string
  type?: string | null
  url?: string | null
  runs?: number
  lastRun?: string | null
}

export interface ProfileStarUserDto {
  id: number
  fullName?: string | null
  email?: string | null
}

export interface ProfileSubscriptionDto {
  id: number
  userId?: number
  userName?: string | null
  emailList?: string | null
  description?: string | null
  lastStatus?: string | null
  lastRunTime?: string | null
  subscriptionTo?: string | null
}

/** A single option in a dynamic filter list (populated from the backend). */
export interface ProfileFilterItemDto {
  value: string
  label: string
  count: number
}

/** Response shape for GET /api/profile/filters */
export interface ProfileFiltersResponseDto {
  server: ProfileFilterItemDto[]
  database: ProfileFilterItemDto[]
  masterFile: ProfileFilterItemDto[]
  visible: ProfileFilterItemDto[]
  certification: ProfileFilterItemDto[]
  availability: ProfileFilterItemDto[]
  reportType: ProfileFilterItemDto[]
}
