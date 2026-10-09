"use client"

import Link from "next/link"
import { useTransition } from "react"
import { setAnalyticsTraceResolvedAction } from "@/app/analytics/actions"
import type { AnalyticsTraceListResponseDto } from "@/lib/analytics/types"
import { summarizeUserAgent } from "@/lib/analytics/user-agent"
import { AnalyticsLogPagination } from "./analytics-log-pagination"
import { AnalyticsTraceMessage, analyticsTraceLevelLabel } from "./analytics-trace-message"

export function AnalyticsTracesTable({
  traces,
  onPageChange,
  onResolvedChange,
}: {
  traces: AnalyticsTraceListResponseDto | null
  onPageChange: (page: number) => void
  onResolvedChange: () => void
}) {
  const [pending, startTransition] = useTransition()

  if (!traces) return null

  const toggleResolved = (id: number, nextResolved: boolean) => {
    startTransition(async () => {
      await setAnalyticsTraceResolvedAction(id, nextResolved)
      onResolvedChange()
    })
  }

  return (
    <section className="mt-8 space-y-3">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <strong className="text-base text-[#363636]">
          Browser Errors -{" "}
          <span className={traces.unresolvedCount > 0 ? "text-[#f14668]" : ""}>
            {traces.unresolvedCount} unresolved
          </span>{" "}
          of {traces.totalCount} Messages
        </strong>
        <AnalyticsLogPagination
          currentPage={traces.currentPage}
          totalPages={traces.pages}
          onPageChange={onPageChange}
        />
      </div>

      <div className="overflow-x-auto rounded-md border">
        <table className="w-full min-w-[960px] border-collapse text-sm">
          <thead>
            <tr className="border-b bg-muted/30 text-left">
              <th className="p-2 font-medium">Severity</th>
              <th className="p-2 font-medium">Date</th>
              <th className="p-2 font-medium">Message</th>
              <th className="p-2 font-medium">User</th>
              <th className="p-2 font-medium">OS</th>
              <th className="p-2 font-medium">Browser</th>
              <th className="p-2 font-medium">Resolved</th>
            </tr>
          </thead>
          <tbody>
            {traces.items.map((trace) => {
              const { os, browser } = summarizeUserAgent(trace.userAgent)
              const resolved = trace.handled === 1
              return (
                <tr
                  key={trace.id}
                  className={`border-b align-top last:border-0 ${resolved ? "text-muted-foreground" : ""}`}
                >
                  <td className="p-2 whitespace-nowrap">{analyticsTraceLevelLabel(trace.level)}</td>
                  <td className="p-2 whitespace-nowrap">{trace.logDateTime ?? "—"}</td>
                  <td className="max-w-md p-2">
                    <AnalyticsTraceMessage
                      message={trace.message}
                      referer={trace.referer}
                      handled={trace.handled}
                    />
                  </td>
                  <td className="p-2 whitespace-nowrap">
                    <Link href={`/users?id=${trace.userId}`} className="hover:underline">
                      {trace.userName}
                    </Link>
                  </td>
                  <td className="p-2 whitespace-nowrap">{os}</td>
                  <td className="p-2 whitespace-nowrap">{browser}</td>
                  <td className="p-2">
                    <input
                      type="checkbox"
                      checked={resolved}
                      disabled={pending}
                      aria-label={`Mark trace ${trace.id} resolved`}
                      onChange={(event) => toggleResolved(trace.id, event.target.checked)}
                    />
                  </td>
                </tr>
              )
            })}
          </tbody>
        </table>
      </div>
      <p className="text-xs text-muted-foreground">
        Page {traces.currentPage} of {traces.pages}
      </p>
    </section>
  )
}
