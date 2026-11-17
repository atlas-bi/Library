export type RecentUpdateItem = {
  id: number
  name: string
}

export type RecentUpdatesSection = {
  items: RecentUpdateItem[]
  failed: boolean
}

export type RecentUpdatesBundle = {
  reports: RecentUpdatesSection
  initiatives: RecentUpdatesSection
  collections: RecentUpdatesSection
}
