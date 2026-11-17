"use client"

import Link from "next/link"
import { useMemo, useState } from "react"
import type { ReportDetail } from "@/lib/reports/types"

type GroupRow = NonNullable<ReportDetail["groups"]>[number]
type SortKey = "name" | "type" | "source"
type SortDirection = "asc" | "desc"

function compareGroups(left: GroupRow, right: GroupRow, key: SortKey): number {
  const leftValue = (left[key] ?? "").toString()
  const rightValue = (right[key] ?? "").toString()
  return leftValue.localeCompare(rightValue, undefined, { sensitivity: "base" })
}

export function ReportRelationshipGroupsTable({ groups }: { groups: GroupRow[] }) {
  const [open, setOpen] = useState(true)
  const [sortKey, setSortKey] = useState<SortKey | null>(null)
  const [sortDirection, setSortDirection] = useState<SortDirection>("asc")

  const displayRows = useMemo(() => {
    if (sortKey === null) return groups
    const copy = [...groups]
    copy.sort((left, right) => {
      const comparison = compareGroups(left, right, sortKey)
      return sortDirection === "asc" ? comparison : -comparison
    })
    return copy
  }, [groups, sortDirection, sortKey])

  const toggleSort = (key: SortKey) => {
    if (sortKey === key) {
      setSortDirection((current) => (current === "asc" ? "desc" : "asc"))
      return
    }
    setSortKey(key)
    setSortDirection("asc")
  }

  const sortLabel = (label: string, key: SortKey) => {
    const indicator = sortKey === key ? (sortDirection === "asc" ? " ↑" : " ↓") : ""
    return `${label}${indicator}`
  }

  return (
    <div className="overflow-hidden rounded-md border border-[var(--atlas-home-border-soft)]">
      <button
        type="button"
        className="flex w-full items-center justify-between bg-muted/30 px-4 py-3 text-left text-sm font-semibold"
        aria-expanded={open}
        onClick={() => setOpen((value) => !value)}
      >
        Report Groups
        <span className="text-muted-foreground">{open ? "−" : "+"}</span>
      </button>
      {open ? (
        <div className="overflow-x-auto">
          <table className="min-w-full text-sm" aria-label="Related report groups">
            <thead>
              <tr className="border-b border-[var(--atlas-home-border-soft)]">
                <th className="px-3 py-2 text-left">
                  <button type="button" className="font-medium hover:underline" onClick={() => toggleSort("name")}>
                    {sortLabel("Group Name", "name")}
                  </button>
                </th>
                <th className="px-3 py-2 text-left">
                  <button type="button" className="font-medium hover:underline" onClick={() => toggleSort("type")}>
                    {sortLabel("Type", "type")}
                  </button>
                </th>
                <th className="px-3 py-2 text-left">
                  <button type="button" className="font-medium hover:underline" onClick={() => toggleSort("source")}>
                    {sortLabel("Source", "source")}
                  </button>
                </th>
              </tr>
            </thead>
            <tbody>
              {displayRows.map((group) => (
                <tr key={group.id} className="border-b border-[var(--atlas-home-border-soft)] last:border-b-0">
                  <td className="px-3 py-2">
                    <Link href={`/groups?id=${group.id}`} className="text-link hover:underline">
                      {group.name ?? group.email ?? `Group ${group.id}`}
                    </Link>
                  </td>
                  <td className="px-3 py-2">{group.type ?? "—"}</td>
                  <td className="px-3 py-2">{group.source ?? "—"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : null}
    </div>
  )
}
