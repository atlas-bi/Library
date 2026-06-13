function stripTrailingSlash(value: string): string {
  return value.replace(/\/+$/, "")
}

export function getServerApiBase(fallbackOrigin?: string): string {
  const apiBase = process.env.API_URL ?? process.env.NEXT_PUBLIC_API_URL ?? fallbackOrigin
  if (!apiBase) return ""
  return stripTrailingSlash(apiBase)
}

export function getPublicApiBase(fallbackOrigin?: string): string {
  const apiBase = process.env.NEXT_PUBLIC_API_URL ?? fallbackOrigin
  if (!apiBase) return ""
  return stripTrailingSlash(apiBase)
}
