"use client"

import Link from "next/link"
import { useMemo, useState } from "react"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import type { ProfileRunListItemDto } from "@/lib/profile/types"

type SortKey = "name" | "type" | "runs" | "lastRun"
type SortDirection = "asc" | "desc"

function parseSortableDate(value: string): number | null {
  const trimmed = value.trim()
  if (!trimmed) return null
  if (/^(?:\d{1,2}\/){2}\d{2,4}$/.test(trimmed)) {
    const parsed = Date.parse(trimmed)
    return Number.isNaN(parsed) ? null : parsed
  }
  const parsed = Date.parse(trimmed)
  return Number.isNaN(parsed) ? null : parsed
}

export function compareProfileRunListRows(
  left: ProfileRunListItemDto,
  right: ProfileRunListItemDto,
  key: SortKey,
): number {
  if (key === "runs") {
    const leftRuns = typeof left.runs === "number" ? left.runs : Number.NEGATIVE_INFINITY
    const rightRuns = typeof right.runs === "number" ? right.runs : Number.NEGATIVE_INFINITY
    return leftRuns - rightRuns
  }

  if (key === "lastRun") {
    const leftDate = parseSortableDate(left.lastRun ?? "")
    const rightDate = parseSortableDate(right.lastRun ?? "")
    if (leftDate !== null && rightDate !== null) return leftDate - rightDate
    if (leftDate !== null) return -1
    if (rightDate !== null) return 1
    return (left.lastRun ?? "").localeCompare(right.lastRun ?? "", undefined, { sensitivity: "base" })
  }

  if (key === "type") {
    return (left.type?.trim() || "").localeCompare(right.type?.trim() || "", undefined, {
      sensitivity: "base",
    })
  }

  return left.name.localeCompare(right.name, undefined, { sensitivity: "base" })
}

export function ProfileRunListTable({ rows }: { rows: ProfileRunListItemDto[] }) {
  const [sortKey, setSortKey] = useState<SortKey | null>(null)
  const [sortDirection, setSortDirection] = useState<SortDirection>("asc")

  const showType = rows.some((row) => row.type?.trim())

  const displayRows = useMemo(() => {
    if (sortKey === null) return rows
    const copy = [...rows]
    copy.sort((left, right) => {
      const comparison = compareProfileRunListRows(left, right, sortKey)
      return sortDirection === "asc" ? comparison : -comparison
    })
    return copy
  }, [rows, sortDirection, sortKey])

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

  if (rows.length === 0) {
    return <p className="text-sm text-muted-foreground">No run data to show.</p>
  }

  return (
    <Table aria-label="Profile report runs">
      <TableHeader>
        <TableRow>
          <TableHead>
            <button type="button" className="font-medium hover:underline" onClick={() => toggleSort("name")}>
              {sortLabel("Name", "name")}
            </button>
          </TableHead>
          {showType ? (
            <TableHead>
              <button type="button" className="font-medium hover:underline" onClick={() => toggleSort("type")}>
                {sortLabel("Type", "type")}
              </button>
            </TableHead>
          ) : null}
          <TableHead>
            <button type="button" className="font-medium hover:underline" onClick={() => toggleSort("runs")}>
              {sortLabel("Runs", "runs")}
            </button>
          </TableHead>
          <TableHead>
            <button type="button" className="font-medium hover:underline" onClick={() => toggleSort("lastRun")}>
              {sortLabel("Last Run", "lastRun")}
            </button>
          </TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {displayRows.map((row) => (
          <TableRow key={`${row.name}-${row.url ?? ""}-${row.lastRun ?? ""}`}>
            <TableCell className="font-medium">
              {row.url ? (
                <Link href={row.url} className="text-link hover:underline">
                  {row.name}
                </Link>
              ) : (
                row.name
              )}
            </TableCell>
            {showType ? <TableCell>{row.type?.trim() || "—"}</TableCell> : null}
            <TableCell>{typeof row.runs === "number" ? row.runs : "—"}</TableCell>
            <TableCell>{row.lastRun?.trim() || "—"}</TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  )
}
