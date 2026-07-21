"use client"

import type { ReactNode } from "react"
import { cn } from "@/lib/utils"

export type GroupTabId = "details" | "users" | "reports" | "activity" | "run-list" | "analytics"

const TAB_LABELS: Record<GroupTabId, string> = {
  details: "Details",
  users: "Users In Group",
  reports: "Reports In Group",
  activity: "Run Analytics",
  "run-list": "Report Runs",
  analytics: "Analytics",
}

export function GroupSectionNav({
  activeTab,
  onTabChange,
  showAnalytics,
}: {
  activeTab: GroupTabId
  onTabChange: (tab: GroupTabId) => void
  showAnalytics: boolean
}) {
  const tabs: GroupTabId[] = [
    "details",
    "users",
    "reports",
    "activity",
    "run-list",
    ...(showAnalytics ? (["analytics"] as GroupTabId[]) : []),
  ]

  return (
    <nav aria-label="Group sections" className="border-b border-border/60">
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

export function GroupTabPanel({
  tab,
  activeTab,
  children,
}: {
  tab: GroupTabId
  activeTab: GroupTabId
  children: ReactNode
}) {
  if (tab !== activeTab) return null
  return <section aria-labelledby={`group-tab-${tab}`}>{children}</section>
}
