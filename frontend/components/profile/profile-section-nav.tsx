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
    <nav aria-label="Profile sections" className="mb-4">
      <ul className="flex flex-wrap items-center gap-2 text-sm font-medium">
        {tabs.map((tab, index) => (
          <li key={tab} className="flex items-center gap-2">
            <button
              type="button"
              onClick={() => onTabChange(tab)}
              className={cn(
                "transition-colors",
                activeTab === tab
                  ? "text-[var(--atlas-home-link,theme(colors.blue.600))] cursor-default"
                  : "text-[var(--atlas-home-link,theme(colors.blue.600))] hover:underline",
              )}
              aria-current={activeTab === tab ? "page" : undefined}
            >
              {TAB_LABELS[tab]}
            </button>
            {index < tabs.length - 1 && (
              <span className="text-muted-foreground select-none">/</span>
            )}
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
