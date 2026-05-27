import Link from "next/link"
import { redirect } from "next/navigation"
import type { ReactNode } from "react"
import { CollectionActionRail } from "@/components/collections/collection-action-rail"
import { CollectionSectionNav } from "@/components/collections/collection-section-nav"
import { MarkdownContent } from "@/components/content/markdown-content"
import { ProfileAnalyticsPanel } from "@/components/profile/profile-analytics-panel"
import { CollectionSnippetCard } from "@/components/snippets/collection-snippet-card"
import { InitiativeSnippetCard } from "@/components/snippets/initiative-snippet-card"
import { ReportSnippetCard } from "@/components/snippets/report-snippet-card"
import { TermSnippetCard } from "@/components/snippets/term-snippet-card"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { getCurrentUser, getToken, hasPermission } from "@/lib/auth"
import { getCollectionById, getCollectionsList } from "@/lib/collections/api"
import type { CollectionReportDto, CollectionTermDto } from "@/lib/collections/types"
import { getUserFriendlyErrorMessage } from "@/lib/errors"

type CollectionsSearchParams = {
  id?: string
  page?: string
  pageSize?: string
}

function getSingleValue(value: string | string[] | undefined): string | undefined {
  if (typeof value === "string") return value
  if (Array.isArray(value)) return value[0]
  return undefined
}

function asPositiveInt(raw: string | undefined, fallback: number): number {
  const parsed = Number(raw)
  return Number.isFinite(parsed) && parsed > 0 ? Math.floor(parsed) : fallback
}

function buildListHref(page: number, pageSize: number): string {
  const params = new URLSearchParams()
  params.set("page", String(page))
  params.set("pageSize", String(pageSize))
  return `/collections?${params.toString()}`
}

function sortByRank<T extends { rank?: number }>(items: T[]): T[] {
  return [...items].sort((a, b) => (a.rank ?? 0) - (b.rank ?? 0))
}

function SectionHeading({ id, children }: { id: string; children: ReactNode }) {
  return (
    <h2 id={id} className="scroll-mt-24 text-2xl font-semibold">
      {children}
    </h2>
  )
}

export default async function CollectionsPage({
  searchParams,
}: {
  searchParams: CollectionsSearchParams
}) {
  const token = await getToken()
  if (!token) redirect("/auth/login")

  const user = await getCurrentUser()
  const canCreateCollection = !!user && hasPermission(user, "Create Collection")

  const idRaw = getSingleValue(searchParams.id)
  if (idRaw) {
    const id = Number(idRaw)
    if (!Number.isFinite(id) || id <= 0) {
      return (
        <div className="mx-auto max-w-4xl px-4 py-10">
          <h1 className="text-2xl font-bold">Collection not found</h1>
          <p className="mt-2 text-sm text-muted-foreground">Missing or invalid collection id.</p>
          <Button asChild className="mt-6" variant="outline">
            <Link href="/collections">Back to collections</Link>
          </Button>
        </div>
      )
    }

    const result = await getCollectionById(id)
    const collection = result.data
    if (!collection) {
      const message = getUserFriendlyErrorMessage(result.error ?? "unknown")
      return (
        <div className="mx-auto max-w-4xl px-4 py-10">
          <Card>
            <CardHeader>
              <CardTitle className="text-xl">Unable to load collection</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <p className="text-sm text-muted-foreground">{message}</p>
              <Button asChild variant="outline">
                <Link href="/collections">Back to collections</Link>
              </Button>
            </CardContent>
          </Card>
        </div>
      )
    }

    const features = collection.features ?? {}
    const termsEnabled = features.termsEnabled !== false
    const sortedReports = sortByRank(collection.reports ?? []) as CollectionReportDto[]
    const sortedTerms = sortByRank(collection.terms ?? []) as CollectionTermDto[]
    const hasInitiative = !!collection.initiative?.id
    const hasReports = sortedReports.length > 0
    const hasTerms = termsEnabled && sortedTerms.length > 0

    return (
      <div className="mx-auto min-h-screen w-full max-w-7xl px-4 py-8">
        <div className="text-sm text-muted-foreground">
          <Link href="/collections" className="hover:underline">
            Collections
          </Link>
        </div>

        <div className="mt-4 flex flex-wrap items-start gap-4">
          <div className="min-w-0 flex-1 space-y-3">
            <h1 className="text-3xl font-bold">{collection.name}</h1>
            {collection.hidden === "Y" ? <Badge variant="outline">Hidden from search</Badge> : null}
            <CollectionSectionNav
              hasInitiative={hasInitiative}
              hasReports={hasReports}
              hasTerms={hasTerms}
            />
          </div>
        </div>

        <div className="mt-8 grid gap-8 lg:grid-cols-[auto_minmax(0,1fr)]">
          <CollectionActionRail
            collection={collection}
            profilePanel={<ProfileAnalyticsPanel id={collection.id} type="collection" />}
          />

          <div className="space-y-10">
            {hasInitiative && collection.initiative ? (
              <section className="space-y-4">
                <SectionHeading id="initiative">Owning Initiative</SectionHeading>
                <div className="grid gap-4 md:grid-cols-2">
                  <InitiativeSnippetCard initiative={collection.initiative} />
                </div>
              </section>
            ) : null}

            {hasReports ? (
              <section className="space-y-4">
                <SectionHeading id="reports">Reports</SectionHeading>
                <div className="grid gap-4 md:grid-cols-2">
                  {sortedReports.map((report) => (
                    <ReportSnippetCard key={report.id} report={report} />
                  ))}
                </div>
              </section>
            ) : null}

            {hasTerms ? (
              <section className="space-y-4">
                <SectionHeading id="terms">Terms</SectionHeading>
                <div className="grid gap-4 md:grid-cols-2">
                  {sortedTerms.map((term) => {
                    const key =
                      typeof term.id === "number"
                        ? term.id
                        : typeof term.termId === "number"
                          ? term.termId
                          : `${term.name ?? "term"}-${term.rank ?? 0}`
                    return <TermSnippetCard key={key} term={term} />
                  })}
                </div>
              </section>
            ) : null}

            <section id="details" className="scroll-mt-24 space-y-6">
              <SectionHeading id="details-heading">Details</SectionHeading>

              <div className="space-y-6">
                {collection.description ? (
                  <div>
                    <h3 className="text-lg font-semibold">Description</h3>
                    <MarkdownContent content={collection.description} className="mt-2" />
                  </div>
                ) : null}

                {collection.purpose ? (
                  <div>
                    <h3 className="text-lg font-semibold">Search Summary</h3>
                    <MarkdownContent content={collection.purpose} className="mt-2" />
                  </div>
                ) : null}
              </div>

              <div className="overflow-x-auto rounded-md border">
                <table className="w-full text-sm" aria-label="Collection metadata">
                  <tbody>
                    {collection.lastUpdatedBy?.fullName || collection.lastUpdatedBy?.username ? (
                      <tr className="border-b">
                        <th className="w-48 px-4 py-2 text-left font-medium text-muted-foreground">
                          Last Updated By
                        </th>
                        <td className="px-4 py-2">
                          {collection.lastUpdatedBy.fullName?.trim() ||
                            collection.lastUpdatedBy.username ||
                            "—"}
                        </td>
                      </tr>
                    ) : null}
                    {collection.lastModifiedDisplay ? (
                      <tr className="border-b">
                        <th className="w-48 px-4 py-2 text-left font-medium text-muted-foreground">
                          Last Updated
                        </th>
                        <td className="px-4 py-2">{collection.lastModifiedDisplay}</td>
                      </tr>
                    ) : null}
                    {collection.hidden === "Y" ? (
                      <tr>
                        <th className="w-48 px-4 py-2 text-left font-medium text-muted-foreground">
                          Hidden from Search?
                        </th>
                        <td className="px-4 py-2">Yes</td>
                      </tr>
                    ) : null}
                  </tbody>
                </table>
              </div>
            </section>
          </div>
        </div>
      </div>
    )
  }

  const page = asPositiveInt(getSingleValue(searchParams.page), 1)
  const pageSize = Math.min(100, asPositiveInt(getSingleValue(searchParams.pageSize), 20))
  const listResult = await getCollectionsList(page, pageSize)
  const list = listResult.data

  if (!list) {
    const message = getUserFriendlyErrorMessage(listResult.error ?? "unknown")
    const statusHint =
      listResult.status != null ? `The API responded with HTTP ${listResult.status}.` : null
    const backendHint =
      listResult.error === "server_error"
        ? "The Collections API on the C# backend returned an error. Confirm the API is running on the feat/collections-api branch (or newer) and check backend logs for GET /api/collections."
        : listResult.error === "service_unavailable"
          ? "API_URL is not configured. Set API_URL in .env.local to your Library backend (e.g. http://localhost:5000)."
          : null
    return (
      <div className="mx-auto max-w-4xl px-4 py-10">
        <Card>
          <CardHeader>
            <CardTitle className="text-xl">Unable to load collections</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <p className="text-sm text-muted-foreground">{message}</p>
            {statusHint ? <p className="text-sm text-muted-foreground">{statusHint}</p> : null}
            {backendHint ? <p className="text-sm text-muted-foreground">{backendHint}</p> : null}
            <Button asChild variant="outline">
              <Link href="/">Back to home</Link>
            </Button>
          </CardContent>
        </Card>
      </div>
    )
  }

  const totalPages = Math.max(1, Math.ceil(list.total / list.pageSize))

  return (
    <div className="mx-auto min-h-screen w-full max-w-7xl space-y-6 px-4 py-8">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div>
          <div className="text-sm text-muted-foreground">
            <Link href="/" className="hover:underline">
              Home
            </Link>
            <span className="px-1">/</span>
            <span>Collections</span>
          </div>
          <h1 className="mt-2 text-3xl font-bold">Collections</h1>
        </div>
        {canCreateCollection ? (
          <Button asChild>
            <Link href="/collections/new">
              <span className="mr-2">+</span>
              Create a Collection
            </Link>
          </Button>
        ) : null}
      </div>

      {list.collections.length === 0 ? (
        <p className="text-sm text-muted-foreground">No collections found.</p>
      ) : (
        <div className="grid gap-4 md:grid-cols-2">
          {list.collections.map((item) => (
            <CollectionSnippetCard key={item.id} collection={item} />
          ))}
        </div>
      )}

      {list.total > list.pageSize ? (
        <div className="flex items-center justify-between gap-2 text-sm text-muted-foreground">
          <span>
            {list.total} total • page {list.page} of {totalPages}
          </span>
          <div className="flex items-center gap-2">
            {page <= 1 ? (
              <Button type="button" variant="outline" disabled>
                Previous
              </Button>
            ) : (
              <Button asChild variant="outline">
                <Link href={buildListHref(page - 1, pageSize)}>Previous</Link>
              </Button>
            )}
            {page >= totalPages ? (
              <Button type="button" variant="outline" disabled>
                Next
              </Button>
            ) : (
              <Button asChild variant="outline">
                <Link href={buildListHref(page + 1, pageSize)}>Next</Link>
              </Button>
            )}
          </div>
        </div>
      ) : null}
    </div>
  )
}
