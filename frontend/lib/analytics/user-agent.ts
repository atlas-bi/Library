/** Lightweight UA hints for analytics tables (Razor uses UAParser). */
export function summarizeUserAgent(userAgent: string | null | undefined): {
  os: string
  browser: string
} {
  const ua = userAgent?.trim() ?? ""
  if (!ua) return { os: "Unknown", browser: "Unknown" }

  let os = "Unknown"
  if (/Windows NT/i.test(ua)) os = "Windows"
  else if (/Mac OS X/i.test(ua)) os = "macOS"
  else if (/Android/i.test(ua)) os = "Android"
  else if (/iPhone|iPad/i.test(ua)) os = "iOS"
  else if (/Linux/i.test(ua)) os = "Linux"

  let browser = "Unknown"
  if (/Edg\//i.test(ua)) browser = "Edge"
  else if (/Chrome\//i.test(ua) && !/Edg\//i.test(ua)) browser = "Chrome"
  else if (/Firefox\//i.test(ua)) browser = "Firefox"
  else if (/Safari\//i.test(ua) && !/Chrome\//i.test(ua)) browser = "Safari"

  return { os, browser }
}
