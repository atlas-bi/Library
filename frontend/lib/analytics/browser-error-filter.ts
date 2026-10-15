export type BrowserErrorPayload = {
  errorMsg?: string
  url?: string
  trace?: string
}

function isExtensionUrl(url: string): boolean {
  if (!url) return false
  return (
    url.startsWith("chrome-extension://") ||
    url.startsWith("moz-extension://") ||
    url.startsWith("safari-extension://") ||
    url.startsWith("edge-extension://")
  )
}

/** Mirrors _atlasShouldIgnoreBrowserError in Razor _Layout.cshtml. */
export function shouldIgnoreBrowserError(payload: BrowserErrorPayload): boolean {
  try {
    const msg = payload.errorMsg ? String(payload.errorMsg) : ""
    const url = payload.url ? String(payload.url) : ""
    const trace = payload.trace ? String(payload.trace) : ""

    if (isExtensionUrl(url) || isExtensionUrl(trace)) {
      return true
    }

    if (
      (msg === "Script error." || msg === "Script error") &&
      (!url || url === ":" || url === "<anonymous>" || url === "null")
    ) {
      return true
    }

    return false
  } catch {
    return false
  }
}
