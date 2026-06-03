import type { HomeTabDefinition, HomeTabId } from "@/lib/home/types"

export const HOME_TABS: HomeTabDefinition[] = [
  { id: "stars", label: "Stars" },
  { id: "subscriptions", label: "Subscriptions" },
  { id: "report-runs", label: "Report Runs" },
  { id: "groups", label: "Groups" },
]

export function getHomeTabById(id: string): HomeTabDefinition | undefined {
  return HOME_TABS.find((tab) => tab.id === id)
}

export function isHomeTabId(value: string): value is HomeTabId {
  return HOME_TABS.some((tab) => tab.id === value)
}
