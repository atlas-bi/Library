const internalHosts = new Set(["0.0.0.0", "127.0.0.1", "localhost"])

export function getAuthRedirectOrigin(requestUrl: string): string {
  const configuredOrigin = process.env.AUTH_RETURN_URL_ORIGIN?.replace(/\/+$/, "")
  if (configuredOrigin) return configuredOrigin

  const url = new URL(requestUrl)
  if (internalHosts.has(url.hostname)) {
    return "http://localhost:3000"
  }

  return url.origin
}
