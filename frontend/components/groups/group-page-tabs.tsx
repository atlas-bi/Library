"use client"

import type { ReactNode } from "react"
import { useState } from "react"
import {
  GroupSectionNav,
  type GroupTabId,
  GroupTabPanel,
} from "@/components/groups/group-section-nav"

export function GroupPageTabs({
  showAnalytics,
  details,
  users,
  reports,
  activity,
  runList,
  analytics,
}: {
  showAnalytics: boolean
  details: ReactNode
  users: ReactNode
  reports: ReactNode
  activity: ReactNode
  runList: ReactNode
  analytics: ReactNode
}) {
  const [activeTab, setActiveTab] = useState<GroupTabId>("details")

  return (
    <div className="space-y-6">
      <GroupSectionNav
        activeTab={activeTab}
        onTabChange={setActiveTab}
        showAnalytics={showAnalytics}
      />
      <div className="py-2">
        <GroupTabPanel tab="details" activeTab={activeTab}>
          {details}
        </GroupTabPanel>
        <GroupTabPanel tab="users" activeTab={activeTab}>
          {users}
        </GroupTabPanel>
        <GroupTabPanel tab="reports" activeTab={activeTab}>
          {reports}
        </GroupTabPanel>
        <GroupTabPanel tab="activity" activeTab={activeTab}>
          {activity}
        </GroupTabPanel>
        <GroupTabPanel tab="run-list" activeTab={activeTab}>
          {runList}
        </GroupTabPanel>
        {showAnalytics ? (
          <GroupTabPanel tab="analytics" activeTab={activeTab}>
            {analytics}
          </GroupTabPanel>
        ) : null}
      </div>
    </div>
  )
}
