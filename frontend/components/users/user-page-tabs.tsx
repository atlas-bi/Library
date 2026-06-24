"use client"

import type { ReactNode } from "react"
import { useMemo, useState } from "react"
import {
  getDefaultUserTab,
  getUserTabs,
  UserSectionNav,
  type UserTabId,
  UserTabPanel,
} from "@/components/users/user-section-nav"
import type { UserPageTabs as UserPageTabsConfig } from "@/lib/users/types"

export function UserPageTabs({
  isCurrentUser,
  tabs,
  stars,
  subscriptions,
  groups,
  activity,
  runList,
  atlasHistory,
  analytics,
}: {
  isCurrentUser: boolean
  tabs: UserPageTabsConfig
  stars: ReactNode
  subscriptions: ReactNode
  groups: ReactNode
  activity: ReactNode
  runList: ReactNode
  atlasHistory: ReactNode
  analytics: ReactNode
}) {
  const visibleTabs = useMemo(() => getUserTabs({ tabs }), [tabs])
  const [activeTab, setActiveTab] = useState<UserTabId>(() =>
    getDefaultUserTab(isCurrentUser, visibleTabs),
  )

  return (
    <div className="space-y-4">
      <UserSectionNav activeTab={activeTab} tabs={visibleTabs} onTabChange={setActiveTab} />
      <UserTabPanel tab="stars" activeTab={activeTab}>
        {stars}
      </UserTabPanel>
      <UserTabPanel tab="subscriptions" activeTab={activeTab}>
        {subscriptions}
      </UserTabPanel>
      <UserTabPanel tab="groups" activeTab={activeTab}>
        {groups}
      </UserTabPanel>
      <UserTabPanel tab="activity" activeTab={activeTab}>
        {activity}
      </UserTabPanel>
      <UserTabPanel tab="run-list" activeTab={activeTab}>
        {runList}
      </UserTabPanel>
      <UserTabPanel tab="atlas-history" activeTab={activeTab}>
        {atlasHistory}
      </UserTabPanel>
      <UserTabPanel tab="analytics" activeTab={activeTab}>
        {analytics}
      </UserTabPanel>
    </div>
  )
}
