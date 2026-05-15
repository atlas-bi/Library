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
  lastStatus?: string | null
  subscriptionTo?: string | null
}
