const fallbackOrigin = "https://atlas.invalid"

export function safeReturnPath(value: string | null | undefined): string {
  if (!value) return "/"

  const trimmed = value.trim()
  if (
    !trimmed.startsWith("/") ||
    trimmed.startsWith("//") ||
    trimmed.includes("\\") ||
    trimmed.includes("\0")
  ) {
    return "/"
  }

  let parsed: URL
  try {
    parsed = new URL(trimmed, fallbackOrigin)
  } catch {
    return "/"
  }

  if (parsed.origin !== fallbackOrigin) return "/"
  return `${parsed.pathname}${parsed.search}${parsed.hash}` || "/"
}

export function loginPath(returnTo: string): string {
  const safe = safeReturnPath(returnTo)
  if (safe === "/") return "/auth/login"
  return `/auth/login?returnTo=${encodeURIComponent(safe)}`
}

export function buildLoginCallbackUrl(origin: string, returnTo: string | null): string {
  const callback = new URL("/auth/callback", origin.replace(/\/+$/, ""))
  const safe = safeReturnPath(returnTo)
  if (safe !== "/") callback.searchParams.set("returnTo", safe)
  return callback.toString()
}
