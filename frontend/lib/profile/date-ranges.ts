export type ProfileDateRangeId =
  | "yesterday"
  | "this-week"
  | "last-7-days"
  | "this-month"
  | "last-30-days"
  | "last-90-days"
  | "this-year"
  | "last-12-months"
  | "all-time"

export type ProfileDateRange = {
  id: ProfileDateRangeId
  label: string
  start_at: number
  end_at: number
}

function startOfDay(date: Date): Date {
  const next = new Date(date)
  next.setHours(0, 0, 0, 0)
  return next
}

function endOfDay(date: Date): Date {
  const next = new Date(date)
  next.setHours(23, 59, 59, 999)
  return next
}

function startOfWeek(date: Date): Date {
  const next = startOfDay(date)
  const day = next.getDay()
  const diff = day === 0 ? -6 : 1 - day
  next.setDate(next.getDate() + diff)
  return next
}

function endOfWeek(date: Date): Date {
  const next = startOfWeek(date)
  next.setDate(next.getDate() + 6)
  return endOfDay(next)
}

function startOfMonth(date: Date): Date {
  return new Date(date.getFullYear(), date.getMonth(), 1)
}

function endOfMonth(date: Date): Date {
  return endOfDay(new Date(date.getFullYear(), date.getMonth() + 1, 0))
}

function startOfYear(date: Date): Date {
  return new Date(date.getFullYear(), 0, 1)
}

function endOfYear(date: Date): Date {
  return endOfDay(new Date(date.getFullYear(), 11, 31))
}

function addDays(date: Date, days: number): Date {
  const next = new Date(date)
  next.setDate(next.getDate() + days)
  return next
}

function addYears(date: Date, years: number): Date {
  const next = new Date(date)
  next.setFullYear(next.getFullYear() + years)
  return next
}

function differenceInSeconds(target: Date, reference: Date): number {
  return Math.floor((target.getTime() - reference.getTime()) / 1000)
}

function buildRange(
  id: ProfileDateRangeId,
  label: string,
  start: Date,
  end: Date,
  now: Date,
): ProfileDateRange {
  return {
    id,
    label,
    start_at: differenceInSeconds(start, now),
    end_at: differenceInSeconds(end, now),
  }
}

export function getProfileDateRanges(now = new Date()): ProfileDateRange[] {
  return [
    buildRange(
      "yesterday",
      "Yesterday",
      startOfDay(addDays(now, -1)),
      endOfDay(addDays(now, -1)),
      now,
    ),
    buildRange("this-week", "This week", startOfWeek(now), endOfWeek(now), now),
    buildRange("last-7-days", "Last 7 days", startOfDay(addDays(now, -7)), now, now),
    buildRange("this-month", "This month", startOfMonth(now), endOfMonth(now), now),
    buildRange("last-30-days", "Last 30 days", startOfDay(addDays(now, -30)), now, now),
    buildRange("last-90-days", "Last 90 days", startOfDay(addDays(now, -90)), now, now),
    buildRange("this-year", "This year", startOfYear(now), endOfYear(now), now),
    buildRange("last-12-months", "Last 12 months", addYears(now, -1), now, now),
    buildRange("all-time", "All time", addYears(now, -10), now, now),
  ]
}

export const DEFAULT_PROFILE_DATE_RANGE_ID: ProfileDateRangeId = "last-12-months"

export function getProfileDateRangeById(
  id: ProfileDateRangeId,
  now = new Date(),
): ProfileDateRange {
  const ranges = getProfileDateRanges(now)
  return (
    ranges.find((range) => range.id === id) ??
    ranges.find((range) => range.id === DEFAULT_PROFILE_DATE_RANGE_ID) ??
    ranges[0]
  )
}
