"use client"

import { useCallback, useEffect, useMemo, useRef, useState, useTransition } from "react"
import { loadProfileAnalyticsAction, type ProfileAnalyticsData } from "@/app/profile/actions"
import { ProfileBarDataGrid } from "@/components/profile/profile-bar-data-section"
import { ProfileDateRangeSelect } from "@/components/profile/profile-date-range-select"
import { ProfileFilterSidebar } from "@/components/profile/profile-filter-sidebar"
import {
  ProfileHistoryChart,
  ProfileSummaryStats,
} from "@/components/profile/profile-history-chart"
import { ProfileRunListTable } from "@/components/profile/profile-run-list-table"
import {
  getProfileTabs,
  ProfileSectionNav,
  type ProfileTabId,
  ProfileTabPanel,
} from "@/components/profile/profile-section-nav"
import { ProfileStarsTable } from "@/components/profile/profile-stars-table"
import { ProfileSubscriptionsTable } from "@/components/profile/profile-subscriptions-table"
import {
  DEFAULT_PROFILE_DATE_RANGE_ID,
  getProfileDateRangeById,
  getProfileDateRanges,
  type ProfileDateRangeId,
} from "@/lib/profile/date-ranges"
import { cn } from "@/lib/utils"

export function ProfileFullView({
  id,
  type,
  initialData,
  variant = "embedded",
  userProfilesEnabled = true,
}: {
  id: number
  type: string
  initialData?: ProfileAnalyticsData | null
  variant?: "embedded" | "page"
  userProfilesEnabled?: boolean
}) {
  const dateRangeOptions = useMemo(
    () => getProfileDateRanges().map((range) => ({ id: range.id, label: range.label })),
    [],
  )
  const tabs = useMemo(() => getProfileTabs(type, id), [id, type])

  const [activeTab, setActiveTab] = useState<ProfileTabId>("runs")
  const [dateRangeId, setDateRangeId] = useState<ProfileDateRangeId>(DEFAULT_PROFILE_DATE_RANGE_ID)
  const [data, setData] = useState<ProfileAnalyticsData | null>(initialData ?? null)
  const [error, setError] = useState<string | null>(null)
  const [isPending, startTransition] = useTransition()

  const loadData = useCallback(
    (nextRangeId: ProfileDateRangeId) => {
      const range = getProfileDateRangeById(nextRangeId)
      startTransition(() => {
        void loadProfileAnalyticsAction(id, type, {
          start_at: range.start_at,
          end_at: range.end_at,
        }).then((result) => {
          if (!result.data) {
            setError(result.error ?? "unknown")
            setData(null)
            return
          }
          setError(null)
          setData(result.data)
        })
      })
    },
    [id, type],
  )

  const skipInitialFetch = useRef(Boolean(initialData))

  useEffect(() => {
    if (skipInitialFetch.current) {
      skipInitialFetch.current = false
      return
    }
    loadData(dateRangeId)
  }, [dateRangeId, loadData])

  const handleDateRangeChange = (nextRangeId: ProfileDateRangeId) => {
    setDateRangeId(nextRangeId)
  }

  if (!data && isPending) {
    return <p className="text-sm text-muted-foreground">Loading profile...</p>
  }

  if (!data) {
    return (
      <p className="text-sm text-muted-foreground">
        {error ? "Unable to load profile analytics." : "Profile analytics unavailable."}
      </p>
    )
  }

  const chart = data.chart
  const showTitle = type !== "user" && id !== -1

  return (
    <div
      className={cn(
        "space-y-4",
        variant === "page" ? "min-h-screen" : "min-h-0",
        isPending && "opacity-70 transition-opacity",
      )}
    >
      {showTitle ? (
        <h2 className="text-2xl font-semibold tracking-tight">Report Activity</h2>
      ) : null}

      <div className="grid gap-6 lg:grid-cols-[minmax(220px,1fr)_minmax(0,3fr)]">
        <ProfileFilterSidebar
          dateRangeId={dateRangeId}
          dateRangeOptions={dateRangeOptions}
          onDateRangeChange={handleDateRangeChange}
        />

        <div className="min-w-0 space-y-4">
          <ProfileSectionNav activeTab={activeTab} tabs={tabs} onTabChange={setActiveTab} />

          <ProfileTabPanel tab="runs" activeTab={activeTab}>
            <div className="flex flex-wrap items-start justify-between gap-4">
              {chart ? (
                <ProfileSummaryStats
                  runs={chart.runs}
                  users={chart.users}
                  runTime={chart.runTime}
                />
              ) : null}
              <ProfileDateRangeSelect
                value={dateRangeId}
                options={dateRangeOptions}
                onChange={handleDateRangeChange}
              />
            </div>

            {chart ? <ProfileHistoryChart history={chart.history} /> : null}

            <ProfileBarDataGrid
              type={type}
              id={id}
              users={data.users}
              reports={data.reports}
              fails={data.fails}
            />
          </ProfileTabPanel>

          <ProfileTabPanel tab="report-runs" activeTab={activeTab}>
            <ProfileRunListTable rows={data.runList} />
          </ProfileTabPanel>

          <ProfileTabPanel tab="stars" activeTab={activeTab}>
            <ProfileStarsTable rows={data.stars} userProfilesEnabled={userProfilesEnabled} />
          </ProfileTabPanel>

          <ProfileTabPanel tab="subscriptions" activeTab={activeTab}>
            <ProfileSubscriptionsTable
              rows={data.subscriptions}
              userProfilesEnabled={userProfilesEnabled}
            />
          </ProfileTabPanel>
        </div>
      </div>
    </div>
  )
}
