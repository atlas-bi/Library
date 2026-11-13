/** Compact number formatting aligned with legacy Razor analytics.js */
export function formatAnalyticsCount(value: number): string {
  const n = Math.trunc(value)
  const d = 10 ** 0
  let i = 7
  while (i) {
    const s = 10 ** (i-- * 3)
    if (s <= n) {
      const scaled = Math.round((n * d) / s) / d
      return `${scaled}${"kMGTPE"[i]}`
    }
  }
  return String(n)
}

export function formatAnalyticsPercent(percent: number | null | undefined): string {
  if (percent == null || Number.isNaN(percent)) return ""
  return `${Math.round(percent * 100)}%`
}

/** Razor shows load time with one decimal (e.g. 1.4s), not compact count formatting. */
export function formatAnalyticsLoadTime(value: number | null | undefined): string {
  if (value == null || Number.isNaN(value)) return "—"
  return String(Math.round(value * 10) / 10)
}
