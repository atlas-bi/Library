import {
  addDays,
  addHours,
  addYears,
  differenceInSeconds,
  endOfDay,
  endOfMonth,
  endOfWeek,
  endOfYear,
  startOfDay,
  startOfMonth,
  startOfWeek,
  startOfYear,
} from "date-fns"

export type AnalyticsRangeId = "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"

export const ANALYTICS_RANGE_OPTIONS: { id: AnalyticsRangeId; label: string }[] = [
  { id: "1", label: "Today" },
  { id: "2", label: "Last 24 hours" },
  { id: "3", label: "This week" },
  { id: "4", label: "Last 7 days" },
  { id: "5", label: "This month" },
  { id: "6", label: "Last 30 days" },
  { id: "7", label: "Last 90 days" },
  { id: "8", label: "This year" },
  { id: "9", label: "All time" },
]

export const DEFAULT_ANALYTICS_RANGE_ID: AnalyticsRangeId = "2"

/** Mirrors legacy `analytics.js` range → `start_at` / `end_at` (seconds from now). */
export function getAnalyticsRangeParams(rangeId: AnalyticsRangeId): {
  startAt: number
  endAt: number
} {
  const now = new Date()
  const weekOptions = { weekStartsOn: 1 as const }

  switch (rangeId) {
    case "1":
      return {
        startAt: differenceInSeconds(startOfDay(now), now),
        endAt: differenceInSeconds(endOfDay(now), now),
      }
    case "3":
      return {
        startAt: differenceInSeconds(startOfWeek(now, weekOptions), now),
        endAt: differenceInSeconds(endOfWeek(now, weekOptions), now),
      }
    case "4":
      return {
        startAt: differenceInSeconds(startOfDay(addDays(now, -7)), now),
        endAt: 0,
      }
    case "5":
      return {
        startAt: differenceInSeconds(startOfMonth(now), now),
        endAt: differenceInSeconds(endOfMonth(now), now),
      }
    case "6":
      return {
        startAt: differenceInSeconds(startOfDay(addDays(now, -30)), now),
        endAt: 0,
      }
    case "7":
      return {
        startAt: differenceInSeconds(startOfDay(addDays(now, -90)), now),
        endAt: 0,
      }
    case "8":
      return {
        startAt: differenceInSeconds(startOfYear(now), now),
        endAt: differenceInSeconds(endOfYear(now), now),
      }
    case "9":
      return {
        startAt: differenceInSeconds(addYears(now, -10), now),
        endAt: 0,
      }
    case "2":
    default:
      return {
        startAt: differenceInSeconds(addHours(now, -24), now),
        endAt: 0,
      }
  }
}
