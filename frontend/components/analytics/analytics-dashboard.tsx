"use client"

import { useCallback, useEffect, useState, useTransition } from "react"
import {
  loadAnalyticsDashboardAction,
  refreshAnalyticsLiveUsersAction,
} from "@/app/analytics/actions"
import type { AnalyticsPageFilters } from "@/lib/analytics/types"
import { ANALYTICS_RANGE_OPTIONS, type AnalyticsRangeId } from "@/lib/analytics/date-ranges"
import { formatAnalyticsCount, formatAnalyticsLoadTime } from "@/lib/analytics/format"
import type { AnalyticsDashboardData } from "@/lib/analytics/types"
import { getUserFriendlyErrorMessage } from "@/lib/errors"
import { AnalyticsActiveUsersTable } from "./analytics-active-users-table"
import { AnalyticsBarDataTable } from "./analytics-bar-data-table"
import { AnalyticsErrorsTable } from "./analytics-errors-table"
import { AnalyticsRangeSelect } from "./analytics-range-select"
import { AnalyticsTracesTable } from "./analytics-traces-table"
import dynamic from "next/dynamic"
import "./analytics-dashboard.css"

const AnalyticsVisitsChart = dynamic(
  () =>
    import("./analytics-visits-chart").then((module) => ({
      default: module.AnalyticsVisitsChart,
    })),
  {
    ssr: false,
    loading: () => (
      <div className="flex h-[400px] items-center justify-center text-sm text-[#7a7a7a]">
        Loading chart…
      </div>
    ),
  },
)

export function AnalyticsDashboard({
  initialData,
  initialFilters,
  loadError,
}: {
  initialData: AnalyticsDashboardData | null
  initialFilters: AnalyticsPageFilters
  loadError: string | null
}) {
  const [data, setData] = useState(initialData)
  const [error, setError] = useState(loadError)
  const [filters, setFilters] = useState(initialFilters)
  const [pending, startTransition] = useTransition()

  const reload = useCallback((nextFilters: AnalyticsPageFilters) => {
    startTransition(async () => {
      const result = await loadAnalyticsDashboardAction(nextFilters)
      if (result.error) {
        setError(result.error)
        return
      }
      setError(null)
      setData(result.data)
      setFilters(nextFilters)
    })
  }, [])

  useEffect(() => {
    const timer = window.setInterval(() => {
      void refreshAnalyticsLiveUsersAction().then((result) => {
        if (result.data) {
          setData((current) => (current ? { ...current, liveUsers: result.data } : current))
        }
      })
    }, 60_000)
    return () => window.clearInterval(timer)
  }, [])

  const visits = data?.visits

  if (error && !data) {
    return <p className="text-red-500">{getUserFriendlyErrorMessage(error as "auth_required")}</p>
  }

  return (
    <div className={pending ? "opacity-70 transition-opacity" : ""}>
      <AnalyticsActiveUsersTable liveUsers={data?.liveUsers ?? null} />

      <div className="mb-4 flex flex-wrap items-end justify-between gap-4">
        <div className="flex flex-wrap gap-x-10 gap-y-4">
          <AnalyticsSummaryStat label="Views" value={visits?.views ?? null} />
          <AnalyticsSummaryStat label="Visitors" value={visits?.visitors ?? null} />
          <AnalyticsSummaryStat
            label="Load Time"
            value={visits?.loadTime ?? null}
            format="loadTime"
          />
        </div>

        <AnalyticsRangeSelect
          value={filters.rangeId}
          disabled={pending}
          onChange={(rangeId: AnalyticsRangeId) => {
            reload({ ...filters, rangeId, tracePage: 0, errorPage: 0 })
          }}
        />
      </div>

      <div className="mb-8 min-h-[400px]">
        <AnalyticsVisitsChart history={visits?.accessHistory ?? []} />
      </div>

      <div className="mb-8 grid gap-8 pt-2 md:grid-cols-3">
        <AnalyticsBarDataTable items={data?.browsers ?? []} />
        <AnalyticsBarDataTable items={data?.os ?? []} />
        <AnalyticsBarDataTable items={data?.resolution ?? []} />
      </div>

      <div className="mb-8 grid gap-8 md:grid-cols-3">
        {!filters.userId && !filters.groupId ? (
          <AnalyticsBarDataTable items={data?.users ?? []} />
        ) : null}
        <AnalyticsBarDataTable items={data?.loadTimes ?? []} />
      </div>

      <AnalyticsTracesTable
        traces={data?.traces ?? null}
        onPageChange={(tracePage) => reload({ ...filters, tracePage })}
        onResolvedChange={() => reload(filters)}
      />

      <AnalyticsErrorsTable
        errors={data?.errors ?? null}
        onPageChange={(errorPage) => reload({ ...filters, errorPage })}
        onResolvedChange={() => reload(filters)}
      />
    </div>
  )
}

function AnalyticsSummaryStat({
  label,
  value,
  format = "count",
}: {
  label: string
  value: number | null
  format?: "count" | "loadTime"
}) {
  const display =
    value === null
      ? "—"
      : format === "loadTime"
        ? formatAnalyticsLoadTime(value)
        : formatAnalyticsCount(value)

  return (
    <div className="flex flex-col">
      <div className="font-serif text-[2rem] leading-none text-[#363636]">
        {display}
        {format === "loadTime" && value !== null ? (
          <span className="font-serif text-[2rem]">s</span>
        ) : null}
      </div>
      <span className="mt-1 text-sm text-[#363636]">{label}</span>
    </div>
  )
}
