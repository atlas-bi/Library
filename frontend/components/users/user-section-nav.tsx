"use client"

import type { ReactNode } from "react"
import { cn } from "@/lib/utils"

export type UserTabId =
  | "stars"
  | "subscriptions"
  | "groups"
  | "activity"
  | "run-list"
  | "atlas-history"
  | "analytics"

const TAB_LABELS: Record<UserTabId, string> = {
  stars: "Stars",
  subscriptions: "Subscriptions",
  groups: "Groups",
  activity: "Run Analytics",
  "run-list": "Report Runs",
  "atlas-history": "Atlas History",
  analytics: "Analytics",
}

export function getUserTabs(options: {
  tabs: {
    starsVisible: boolean
    subscriptionsVisible: boolean
    groupsVisible: boolean
    activityVisible: boolean
    runListVisible: boolean
    atlasHistoryVisible: boolean
    analyticsVisible: boolean
  }
}): UserTabId[] {
  const visible: UserTabId[] = []
  if (options.tabs.starsVisible) visible.push("stars")
  if (options.tabs.subscriptionsVisible) visible.push("subscriptions")
  if (options.tabs.groupsVisible) visible.push("groups")
  if (options.tabs.activityVisible) visible.push("activity")
  if (options.tabs.runListVisible) visible.push("run-list")
  if (options.tabs.atlasHistoryVisible) visible.push("atlas-history")
  if (options.tabs.analyticsVisible) visible.push("analytics")
  return visible
}

export function getDefaultUserTab(isCurrentUser: boolean, tabs: UserTabId[]): UserTabId {
  if (isCurrentUser && tabs.includes("stars")) return "stars"
  if (!isCurrentUser && tabs.includes("activity")) return "activity"
  return tabs[0] ?? "stars"
}

export function UserSectionNav({
  activeTab,
  tabs,
  onTabChange,
}: {
  activeTab: UserTabId
  tabs: UserTabId[]
  onTabChange: (tab: UserTabId) => void
}) {
  return (
    <nav aria-label="User profile sections" className="border-b border-border/60">
      <ul className="flex flex-wrap gap-1 text-sm">
        {tabs.map((tab) => (
          <li key={tab}>
            <button
              type="button"
              onClick={() => onTabChange(tab)}
              className={cn(
                "rounded-t-md px-3 py-2 font-medium transition-colors",
                activeTab === tab
                  ? "border border-b-0 border-border/60 bg-background text-foreground"
                  : "text-muted-foreground hover:bg-muted/50 hover:text-foreground",
              )}
              aria-current={activeTab === tab ? "page" : undefined}
            >
              {TAB_LABELS[tab]}
            </button>
          </li>
        ))}
      </ul>
    </nav>
  )
}

export function UserTabPanel({
  tab,
  activeTab,
  children,
}: {
  tab: UserTabId
  activeTab: UserTabId
  children: ReactNode
}) {
  if (tab !== activeTab) return null
  return <section className="space-y-6 py-2">{children}</section>
}
