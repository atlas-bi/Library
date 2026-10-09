"use client"

import Link from "next/link"
import { useState, useTransition } from "react"
import { setAnalyticsErrorResolvedAction } from "@/app/analytics/actions"
import type { AnalyticsErrorListResponseDto } from "@/lib/analytics/types"
import { getUserFriendlyErrorMessage } from "@/lib/errors"
import { summarizeUserAgent } from "@/lib/analytics/user-agent"
import { AnalyticsLogPagination } from "./analytics-log-pagination"

export function AnalyticsErrorsTable({
  errors,
  onPageChange,
  onResolvedChange,
}: {
  errors: AnalyticsErrorListResponseDto | null
  onPageChange: (page: number) => void
  onResolvedChange: () => void
}) {
  const [pending, startTransition] = useTransition()
  const [actionError, setActionError] = useState<string | null>(null)

  if (!errors) return null

  const toggleResolved = (id: number, nextResolved: boolean) => {
    startTransition(async () => {
      const result = await setAnalyticsErrorResolvedAction(id, nextResolved)
      if (!result.ok) {
        setActionError(getUserFriendlyErrorMessage(result.error ?? "unknown"))
        return
      }
      setActionError(null)
      onResolvedChange()
    })
  }

  return (
    <section className="mt-8 space-y-3">
      {actionError ? (
        <p className="text-sm text-red-500" role="alert">
          {actionError}
        </p>
      ) : null}
      <div className="flex flex-wrap items-center justify-between gap-3">
        <strong className="text-base text-[#363636]">
          Server Errors -{" "}
          <span className={errors.unresolvedCount > 0 ? "text-[#f14668]" : ""}>
            {errors.unresolvedCount} unresolved
          </span>{" "}
          of {errors.totalCount} Messages
        </strong>
        <AnalyticsLogPagination
          currentPage={errors.currentPage}
          totalPages={errors.pages}
          onPageChange={onPageChange}
        />
      </div>

      <div className="overflow-x-auto rounded-md border">
        <table className="w-full min-w-[960px] border-collapse text-sm">
          <thead>
            <tr className="border-b bg-muted/30 text-left">
              <th className="p-2 font-medium">Status</th>
              <th className="p-2 font-medium">Date</th>
              <th className="p-2 font-medium">Message</th>
              <th className="p-2 font-medium">User</th>
              <th className="p-2 font-medium">OS</th>
              <th className="p-2 font-medium">Browser</th>
              <th className="p-2 font-medium">Resolved</th>
            </tr>
          </thead>
          <tbody>
            {errors.items.map((error) => {
              const { os, browser } = summarizeUserAgent(error.userAgent)
              const resolved = error.handled === 1
              return (
                <tr
                  key={error.id}
                  className={`border-b align-top last:border-0 ${resolved ? "text-muted-foreground" : ""}`}
                >
                  <td className="p-2 whitespace-nowrap">{error.statusCode ?? "—"}</td>
                  <td className="p-2 whitespace-nowrap">{error.logDateTime ?? "—"}</td>
                  <td className="max-w-md p-2">
                    <Link href={error.referrer} className="text-primary hover:underline">
                      {error.referrer}
                    </Link>
                    <p className="mt-1">{error.message}</p>
                    {error.trace ? (
                      <pre
                        className={`mt-2 overflow-x-auto rounded-md p-2 text-xs ${
                          resolved ? "bg-muted/40" : "bg-destructive/10 text-destructive"
                        }`}
                      >
                        {error.trace}
                      </pre>
                    ) : null}
                  </td>
                  <td className="p-2 whitespace-nowrap">
                    <Link href={`/users?id=${error.userId}`} className="hover:underline">
                      {error.userName}
                    </Link>
                  </td>
                  <td className="p-2 whitespace-nowrap">{os}</td>
                  <td className="p-2 whitespace-nowrap">{browser}</td>
                  <td className="p-2">
                    <input
                      type="checkbox"
                      checked={resolved}
                      disabled={pending}
                      aria-label={`Mark error ${error.id} resolved`}
                      onChange={(event) => toggleResolved(error.id, event.target.checked)}
                    />
                  </td>
                </tr>
              )
            })}
          </tbody>
        </table>
      </div>
      <p className="text-xs text-muted-foreground">
        Page {errors.currentPage} of {errors.pages}
      </p>
    </section>
  )
}
