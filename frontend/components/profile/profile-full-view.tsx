"use client"

import { useCallback, useEffect, useMemo, useRef, useState, useTransition } from "react"
import {
  loadProfileAnalyticsAction,
  loadProfileFiltersAction,
  type ProfileAnalyticsData,
  type ProfileFiltersData,
} from "@/app/profile/actions"
import { ProfileBarDataGrid } from "@/components/profile/profile-bar-data-section"
import { ProfileDateRangeSelect } from "@/components/profile/profile-date-range-select"
import {
  EMPTY_SIDEBAR_FILTERS,
  ProfileFilterSidebar,
  type ProfileSidebarFilters,
} from "@/components/profile/profile-filter-sidebar"
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
  const [sidebarFilters, setSidebarFilters] = useState<ProfileSidebarFilters>(EMPTY_SIDEBAR_FILTERS)
  const [data, setData] = useState<ProfileAnalyticsData | null>(initialData ?? null)
  const [filterOptions, setFilterOptions] = useState<ProfileFiltersData | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(!initialData)
  const [isPending, startTransition] = useTransition()

  const skipInitialFetch = useRef(!!initialData)

  const loadData = useCallback(
    (nextRangeId: ProfileDateRangeId, nextFilters?: ProfileSidebarFilters) => {
      const range = getProfileDateRangeById(nextRangeId)
      const activeFilters = nextFilters ?? sidebarFilters
      setIsLoading(true)
      startTransition(() => {
        const payload = {
          start_at: range.start_at,
          end_at: range.end_at,
          server: activeFilters.server.length ? activeFilters.server : undefined,
          database: activeFilters.database.length ? activeFilters.database : undefined,
          masterFile: activeFilters.masterFile.length ? activeFilters.masterFile : undefined,
          visible: activeFilters.visible.length ? activeFilters.visible : undefined,
          certification: activeFilters.certification.length ? activeFilters.certification : undefined,
          availability: activeFilters.availability.length ? activeFilters.availability : undefined,
          reportType: activeFilters.reportType.length ? activeFilters.reportType : undefined,
        }

        Promise.all([
          loadProfileAnalyticsAction(id, type, payload),
          loadProfileFiltersAction(id, type, payload),
        ]).then(([analyticsResult, filtersResult]) => {
          if (!analyticsResult.data) {
            setError(analyticsResult.error ?? null)
            setData(null)
            setFilterOptions(null)
            setIsLoading(false)
            return
          }
          setError(null)
          setData(analyticsResult.data)
          setFilterOptions(filtersResult)
          setIsLoading(false)
        })
      })
    },
    // sidebarFilters intentionally excluded – callers pass the latest value directly
    // to avoid stale-closure issues when multiple state updates fire together.
    // eslint-disable-next-line react-hooks/exhaustive-deps
    [id, type],
  )


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

  const handleFiltersChange = useCallback(
    (patch: Partial<ProfileSidebarFilters>) => {
      setSidebarFilters((prev) => {
        const next = { ...prev, ...patch }
        loadData(dateRangeId, next)
        return next
      })
    },
    [dateRangeId, loadData],
  )

  if (!data && isLoading) {
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
          filters={sidebarFilters}
          onFiltersChange={handleFiltersChange}
          filterOptions={filterOptions}
        />

        <div className="min-w-0 space-y-4">
          <ProfileSectionNav activeTab={activeTab} tabs={tabs} onTabChange={setActiveTab} />

          <ProfileTabPanel tab="runs" activeTab={activeTab}>
            <div className="flex flex-wrap items-start justify-between gap-4">
              <ProfileSummaryStats
                runs={chart?.runs ?? null}
                users={chart?.users ?? null}
                runTime={chart?.runTime ?? null}
              />
              <ProfileDateRangeSelect
                value={dateRangeId}
                options={dateRangeOptions}
                onChange={handleDateRangeChange}
              />
            </div>

            <ProfileHistoryChart history={chart?.history ?? []} />

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
