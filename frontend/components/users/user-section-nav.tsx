"use client"

import { useEffect, useState } from "react"
import type { ReactNode } from "react"

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

export function getHashUserTab(hash: string | null, tabs: UserTabId[]): UserTabId | null {
  const normalized = hash?.replace(/^#/, "")
  if (!normalized) return null
  return tabs.find((tab) => tab === normalized) ?? null
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
  const [currentHash, setCurrentHash] = useState<string>("")

  useEffect(() => {
    const syncHash = () => setCurrentHash(window.location.hash.replace(/^#/, ""))
    syncHash()
    window.addEventListener("hashchange", syncHash)
    return () => window.removeEventListener("hashchange", syncHash)
  }, [])

  return (
    <nav aria-label="User profile sections" className="atlas-home-tab-nav">
      <ul className="flex flex-wrap items-center text-[0.95rem]">
        {tabs.map((tab, index) => (
          <li key={tab}>
            {index > 0 ? (
              <span className="mx-1.5 text-[var(--atlas-home-muted-light)]">/</span>
            ) : null}
            <a
              href={`#${tab}`}
              onClick={(event) => {
                event.preventDefault()
                window.history.replaceState(null, "", `#${tab}`)
                setCurrentHash(tab)
                onTabChange(tab)
              }}
              className={
                activeTab === tab || currentHash === tab
                  ? "text-[0.9rem] font-medium text-[var(--atlas-home-link)] hover:text-[var(--atlas-home-link-hover)] hover:underline"
                  : "text-[0.9rem] text-[var(--atlas-home-link)] hover:text-[var(--atlas-home-link-hover)] hover:underline"
              }
              aria-current={activeTab === tab ? "page" : undefined}
            >
              {TAB_LABELS[tab]}
            </a>
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
  return (
    <section id={tab} className="space-y-6 pt-2">
      {children}
    </section>
  )
}
