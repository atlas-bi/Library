import Link from "next/link"
import { redirect } from "next/navigation"
import type { ReactNode } from "react"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { getCurrentUser, getToken, hasPermission } from "@/lib/auth"
import { getUserFriendlyErrorMessage } from "@/lib/errors"
import { searchLibrary } from "@/lib/search/api"
import { SEARCH_TYPES, type SearchResultDto, type SearchType } from "@/lib/search/types"

type SearchPageParams = Record<string, string | string[] | undefined>

const RESERVED_KEYS = new Set(["q", "type", "page", "pageSize", "field", "advanced"])

function getSingleValue(value: string | string[] | undefined): string | undefined {
  if (typeof value === "string") return value
  if (Array.isArray(value)) return value[0]
  return undefined
}

function asPositiveInt(raw: string | undefined, fallback: number): number {
  const parsed = Number(raw)
  return Number.isFinite(parsed) && parsed > 0 ? parsed : fallback
}

function normalizeSearchType(raw: string | undefined): SearchType {
  if (!raw) return "query"
  return SEARCH_TYPES.includes(raw as SearchType) ? (raw as SearchType) : "query"
}

function toHrefFromParams(params: URLSearchParams): string {
  const query = params.toString()
  return query ? `/search?${query}` : "/search"
}

function buildResultHref(result: SearchResultDto): string {
  switch (result.type) {
    case "reports":
      return `/reports?id=${result.atlasId}`
    case "collections":
      return `/collections?id=${result.atlasId}`
    case "terms":
      return `/terms?id=${result.atlasId}`
    case "initiatives":
      return `/initiatives?id=${result.atlasId}`
    case "users":
      return `/users?id=${result.atlasId}`
    case "groups":
      return `/groups?id=${result.atlasId}`
    case "external":
      return result.url ?? "#"
  }
}

function getFacetValues(params: SearchPageParams, key: string): string[] {
  const value = params[key]
  if (typeof value === "string") return value ? [value] : []
  if (Array.isArray(value)) return value.filter(Boolean)
  return []
}

function renderResultMeta(result: SearchResultDto) {
  if (result.type === "reports") {
    return (
      <div className="mt-2 flex flex-wrap items-center gap-2 text-xs text-muted-foreground">
        {result.reportType ? <Badge variant="outline">{result.reportType}</Badge> : null}
        {result.epicMasterFile ? (
          <Badge variant="secondary">Epic: {result.epicMasterFile}</Badge>
        ) : null}
        {result.documented ? (
          <Badge variant="outline">Documented: {result.documented}</Badge>
        ) : null}
        {result.certifications.map((value) => (
          <Badge key={value} variant="secondary">
            {value}
          </Badge>
        ))}
      </div>
    )
  }

  if (result.type === "users") {
    return result.email ? (
      <div className="mt-2 text-xs text-muted-foreground">{result.email}</div>
    ) : null
  }

  if (result.type === "groups") {
    return (
      <div className="mt-2 flex flex-wrap items-center gap-2 text-xs text-muted-foreground">
        {result.email ? <span>{result.email}</span> : null}
        {result.groupType ? <Badge variant="outline">{result.groupType}</Badge> : null}
      </div>
    )
  }

  if (result.type === "external" && result.url) {
    return (
      <div className="mt-2 text-xs text-muted-foreground">
        <a href={result.url} target="_blank" rel="noreferrer" className="underline">
          {result.url}
        </a>
      </div>
    )
  }

  return null
}

function renderHighlightSnippet(snippet: string) {
  const parts = snippet.split(/(<\/?em>)/g)
  let isEmphasized = false
  let nodeIndex = 0
  const nodes: ReactNode[] = []

  parts.forEach((part) => {
    if (part === "<em>") {
      isEmphasized = true
      return
    }
    if (part === "</em>") {
      isEmphasized = false
      return
    }

    const sanitized = part.replace(/<[^>]+>/g, "")
    if (!sanitized) return

    if (isEmphasized) {
      const key = `em-${nodeIndex}`
      nodeIndex += 1
      nodes.push(
        <em key={key} className="bg-yellow-100 font-semibold not-italic">
          {sanitized}
        </em>,
      )
      return
    }

    const key = `txt-${nodeIndex}`
    nodeIndex += 1
    nodes.push(<span key={key}>{sanitized}</span>)
  })

  return nodes
}

export default async function SearchPage({ searchParams }: { searchParams: SearchPageParams }) {
  const token = await getToken()
  if (!token) redirect("/auth/login")

  const user = await getCurrentUser()
  const canUseAdvancedSearch = !!user && hasPermission(user, "Show Advanced Search")

  const q = getSingleValue(searchParams.q) ?? ""
  const type = normalizeSearchType(getSingleValue(searchParams.type))
  const page = asPositiveInt(getSingleValue(searchParams.page), 1)
  const pageSize = asPositiveInt(getSingleValue(searchParams.pageSize), 20)
  const field = getSingleValue(searchParams.field)
  const advancedRequested = getSingleValue(searchParams.advanced) === "Y"

  const requestParams: SearchPageParams = {
    q,
    type,
    page: String(page),
    pageSize: String(Math.min(100, pageSize)),
  }

  if (field) requestParams.field = field
  if (canUseAdvancedSearch && advancedRequested) requestParams.advanced = "Y"

  // Preserve all dynamic facet filters by passing every non-reserved param through.
  Object.entries(searchParams).forEach(([key, value]) => {
    if (!RESERVED_KEYS.has(key)) {
      requestParams[key] = value
    }
  })

  const result = await searchLibrary(requestParams)
  if (!result.data) {
    const message = getUserFriendlyErrorMessage(result.error ?? "unknown")
    return (
      <div className="mx-auto max-w-6xl px-4 py-10">
        <Card>
          <CardHeader>
            <CardTitle className="text-xl">Unable to load search</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <p className="text-sm text-muted-foreground">{message}</p>
            <Button asChild variant="outline">
              <Link href="/">Back to home</Link>
            </Button>
          </CardContent>
        </Card>
      </div>
    )
  }

  const data = result.data
  const totalPages = Math.max(1, Math.ceil(data.total / data.pageSize))

  const prevParams = new URLSearchParams()
  Object.entries(requestParams).forEach(([key, value]) => {
    if (typeof value === "string") {
      prevParams.set(key, value)
    }
    if (Array.isArray(value)) {
      value.forEach((v) => {
        prevParams.append(key, v)
      })
    }
  })
  prevParams.set("page", String(Math.max(1, data.page - 1)))

  const nextParams = new URLSearchParams(prevParams.toString())
  nextParams.set("page", String(Math.min(totalPages, data.page + 1)))

  return (
    <div className="mx-auto grid min-h-screen w-full max-w-7xl gap-6 px-4 py-8 lg:grid-cols-[280px_1fr]">
      <aside className="space-y-4">
        <Card>
          <CardHeader>
            <CardTitle className="text-base">Search</CardTitle>
          </CardHeader>
          <CardContent>
            <form method="get" className="space-y-3">
              <input
                name="q"
                defaultValue={q}
                placeholder="Search..."
                className="w-full rounded-md border bg-background px-3 py-2 text-sm"
              />
              <select
                name="type"
                defaultValue={type}
                className="w-full rounded-md border bg-background px-3 py-2 text-sm"
              >
                {SEARCH_TYPES.map((searchType) => (
                  <option key={searchType} value={searchType}>
                    {searchType}
                  </option>
                ))}
              </select>
              <input
                name="pageSize"
                type="number"
                min={1}
                max={100}
                defaultValue={String(data.pageSize)}
                className="w-full rounded-md border bg-background px-3 py-2 text-sm"
              />
              {type === "reports" && data.filterFields.length > 0 ? (
                <select
                  name="field"
                  defaultValue={field ?? ""}
                  className="w-full rounded-md border bg-background px-3 py-2 text-sm"
                >
                  <option value="">All fields</option>
                  {data.filterFields.map((item) => (
                    <option key={item.key} value={item.key}>
                      {item.label}
                    </option>
                  ))}
                </select>
              ) : null}
              {canUseAdvancedSearch ? (
                <label className="flex items-center gap-2 text-sm text-muted-foreground">
                  <input
                    type="checkbox"
                    name="advanced"
                    value="Y"
                    defaultChecked={advancedRequested}
                  />
                  Advanced search
                </label>
              ) : null}
              <Button type="submit" className="w-full">
                Search
              </Button>
            </form>
          </CardContent>
        </Card>

        {data.facets.map((facet) => {
          const selected = new Set(getFacetValues(searchParams, facet.key))
          return (
            <Card key={facet.key}>
              <CardHeader>
                <CardTitle className="text-sm">{facet.key}</CardTitle>
              </CardHeader>
              <CardContent className="space-y-2">
                {facet.values.map((value) => {
                  const isActive = selected.has(value.value)
                  const params = new URLSearchParams()
                  Object.entries(requestParams).forEach(([key, requestValue]) => {
                    if (key === facet.key) {
                      return
                    }
                    if (typeof requestValue === "string") {
                      params.append(key, requestValue)
                    }
                    if (Array.isArray(requestValue)) {
                      requestValue.forEach((v) => {
                        params.append(key, v)
                      })
                    }
                  })

                  const nextValues = isActive
                    ? Array.from(selected).filter((item) => item !== value.value)
                    : [...Array.from(selected), value.value]

                  nextValues.forEach((v) => {
                    params.append(facet.key, v)
                  })
                  params.set("page", "1")

                  return (
                    <div
                      key={value.value}
                      className="flex items-center justify-between gap-2 text-sm"
                    >
                      <Link
                        href={toHrefFromParams(params)}
                        className={
                          isActive ? "font-semibold text-primary underline" : "hover:underline"
                        }
                      >
                        {value.value}
                      </Link>
                      <span className="text-xs text-muted-foreground">{value.count}</span>
                    </div>
                  )
                })}
              </CardContent>
            </Card>
          )
        })}
      </aside>

      <main className="space-y-4">
        <div className="flex flex-wrap items-center justify-between gap-3">
          <div className="text-sm text-muted-foreground">
            {data.total} result{data.total === 1 ? "" : "s"} • page {data.page} of {totalPages} •
            qTime {data.qTime}
            ms
          </div>
          {data.isAdvancedSearch ? <Badge>Advanced mode active</Badge> : null}
        </div>

        {data.results.length === 0 ? (
          <Card>
            <CardContent className="py-10 text-center text-sm text-muted-foreground">
              No search results found.
            </CardContent>
          </Card>
        ) : (
          data.results.map((item) => {
            const highlight = data.highlights.find((h) => h.id === item.id)
            const descriptionSnippet = highlight?.fields.find(
              (f) => f.field === "description",
            )?.snippet
            const href = buildResultHref(item)
            const isExternal = item.type === "external"

            return (
              <Card key={item.id}>
                <CardHeader className="space-y-2">
                  <div className="flex items-center justify-between gap-3">
                    <CardTitle className="text-lg">
                      {isExternal ? (
                        <a href={href} target="_blank" rel="noreferrer" className="hover:underline">
                          {item.name}
                        </a>
                      ) : (
                        <Link href={href} className="hover:underline">
                          {item.name}
                        </Link>
                      )}
                    </CardTitle>
                    <div className="flex items-center gap-2">
                      <Badge variant="outline">{item.type}</Badge>
                      {typeof item.isStarred === "boolean" ? (
                        <Badge variant={item.isStarred ? "default" : "secondary"}>
                          {item.isStarred ? "Starred" : "Not starred"}
                        </Badge>
                      ) : null}
                    </div>
                  </div>
                </CardHeader>
                <CardContent className="space-y-2">
                  {descriptionSnippet ? (
                    <p className="text-sm text-muted-foreground">
                      {renderHighlightSnippet(descriptionSnippet)}
                    </p>
                  ) : item.description ? (
                    <p className="text-sm text-muted-foreground">{item.description}</p>
                  ) : null}
                  {renderResultMeta(item)}
                </CardContent>
              </Card>
            )
          })
        )}

        {data.total > data.pageSize ? (
          <div className="flex items-center justify-end gap-2">
            <Button asChild variant="outline" disabled={data.page <= 1}>
              <Link href={toHrefFromParams(prevParams)}>Previous</Link>
            </Button>
            <Button asChild variant="outline" disabled={data.page >= totalPages}>
              <Link href={toHrefFromParams(nextParams)}>Next</Link>
            </Button>
          </div>
        ) : null}
      </main>
    </div>
  )
}
