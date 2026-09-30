"use client"

import { useEffect, useState } from "react"
import {
  AuditPanel,
  CanMakeReportsPanel,
  MaintenanceRequiredPanel,
  MissingSchedulePanel,
  NewUndocumentedPanel,
  NotInAnalyticsPanel,
  RecommendRetirePanel,
  TopUndocumentedPanel,
  UnusedPanel,
} from "@/components/tasks/tasks-bucket-panels"
import {
  getDefaultTaskTab,
  getHashTaskTab,
  TaskTabPanel,
  TasksSectionNav,
  type TaskTabId,
} from "@/components/tasks/tasks-section-nav"
import type { TasksResponseDto } from "@/lib/tasks/types"

export function TasksPageTabs({
  data,
  canViewGroups,
}: {
  data: TasksResponseDto
  canViewGroups: boolean
}) {
  const [activeTab, setActiveTab] = useState<TaskTabId>(() => getDefaultTaskTab())

  useEffect(() => {
    const syncFromHash = () => {
      const hashTab = getHashTaskTab(window.location.hash)
      if (hashTab) {
        setActiveTab(hashTab)
        return
      }
      setActiveTab(getDefaultTaskTab())
    }

    syncFromHash()
    window.addEventListener("hashchange", syncFromHash)
    return () => window.removeEventListener("hashchange", syncFromHash)
  }, [])

  return (
    <div className="space-y-5">
      <TasksSectionNav activeTab={activeTab} onTabChangeAction={setActiveTab} />
      <TaskTabPanel tab="recommendretire" activeTab={activeTab}>
        <RecommendRetirePanel items={data.recommendRetire} />
      </TaskTabPanel>
      <TaskTabPanel tab="dead" activeTab={activeTab}>
        <UnusedPanel items={data.unused} />
      </TaskTabPanel>
      <TaskTabPanel tab="maintenancerequired" activeTab={activeTab}>
        <MaintenanceRequiredPanel items={data.maintenanceRequired} />
      </TaskTabPanel>
      <TaskTabPanel tab="auditonly" activeTab={activeTab}>
        <AuditPanel items={data.audit} />
      </TaskTabPanel>
      <TaskTabPanel tab="missingschedule" activeTab={activeTab}>
        <MissingSchedulePanel items={data.missingSchedule} />
      </TaskTabPanel>
      <TaskTabPanel tab="notanalytics" activeTab={activeTab}>
        <NotInAnalyticsPanel items={data.notInAnalytics} />
      </TaskTabPanel>
      <TaskTabPanel tab="topundocumented" activeTab={activeTab}>
        <TopUndocumentedPanel items={data.topUndocumented} />
      </TaskTabPanel>
      <TaskTabPanel tab="newundocumented" activeTab={activeTab}>
        <NewUndocumentedPanel items={data.newUndocumented} />
      </TaskTabPanel>
      <TaskTabPanel tab="canmakereports" activeTab={activeTab}>
        <CanMakeReportsPanel items={data.canMakeReports} canViewGroups={canViewGroups} />
      </TaskTabPanel>
    </div>
  )
}
