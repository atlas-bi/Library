const internalHosts = new Set(["0.0.0.0", "127.0.0.1", "localhost"])

function stripTrailingSlashes(value: string): string {
  let end = value.length
  while (end > 0 && value.charCodeAt(end - 1) === 47) {
    end -= 1
  }
  return value.slice(0, end)
}

export function getAuthRedirectOrigin(requestUrl: string): string {
  const configuredOrigin = process.env.AUTH_RETURN_URL_ORIGIN
    ? stripTrailingSlashes(process.env.AUTH_RETURN_URL_ORIGIN)
    : undefined
  if (configuredOrigin) return configuredOrigin

  const url = new URL(requestUrl)
  if (internalHosts.has(url.hostname)) {
    return "http://localhost:3000"
  }

  return url.origin
}
