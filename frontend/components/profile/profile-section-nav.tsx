"use client"

import type { ReactNode } from "react"
import { cn } from "@/lib/utils"

export type ProfileTabId = "runs" | "report-runs" | "stars" | "subscriptions"

const TAB_LABELS: Record<ProfileTabId, string> = {
  runs: "Run Analytics",
  "report-runs": "Report Runs",
  stars: "Stars",
  subscriptions: "Subscriptions",
}

export function getProfileTabs(type: string, id: number): ProfileTabId[] {
  const tabs: ProfileTabId[] = ["runs"]

  if (type === "report") {
    tabs.push("report-runs")
  }

  if (type !== "user" && id !== -1) {
    tabs.push("stars")
  }

  if (type === "report" && id !== -1) {
    tabs.push("subscriptions")
  }

  return tabs
}

export function ProfileSectionNav({
  activeTab,
  tabs,
  onTabChange,
}: {
  activeTab: ProfileTabId
  tabs: ProfileTabId[]
  onTabChange: (tab: ProfileTabId) => void
}) {
  if (tabs.length <= 1) return null

  return (
    <nav aria-label="Profile sections" className="border-b border-border/60">
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

export function ProfileTabPanel({
  tab,
  activeTab,
  children,
}: {
  tab: ProfileTabId
  activeTab: ProfileTabId
  children: ReactNode
}) {
  if (tab !== activeTab) return null
  return <section className="space-y-6">{children}</section>
}
