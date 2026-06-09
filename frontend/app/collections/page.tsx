import Link from "next/link"
import { redirect } from "next/navigation"
import { CollectionActionRail } from "@/components/collections/collection-action-rail"
import {
  CollectionDetailSection,
  CollectionSubsection,
} from "@/components/collections/collection-detail-section"
import { CollectionMetadataTable } from "@/components/collections/collection-metadata-table"
import { CollectionSectionNav } from "@/components/collections/collection-section-nav"
import { CollectionsListCard } from "@/components/collections/collections-list-card"
import { MarkdownContent } from "@/components/content/markdown-content"
import { LibraryShell } from "@/components/layout/library-shell"
import { ProfileAnalyticsPanel } from "@/components/profile/profile-analytics-panel"
import { InitiativeSnippetCard } from "@/components/snippets/initiative-snippet-card"
import { ReportSnippetCard } from "@/components/snippets/report-snippet-card"
import { TermSnippetCard } from "@/components/snippets/term-snippet-card"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { type AuthUser, getCurrentUser, getToken, hasPermission } from "@/lib/auth"
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

function resolveDisplayName(user: AuthUser | null): string {
  if (!user) return "Guest"
  if (user.fullname && user.fullname !== "Guest") return user.fullname
  return user.username || "Guest"
}

function getShellProps(user: AuthUser | null) {
  return {
    displayName: resolveDisplayName(user),
    isSignedIn: !!user,
    isAdministrator: !!user && user.roles.includes("Administrator"),
    adminEnabled: user?.adminEnabled ?? false,
  }
}

export default async function CollectionsPage({
  searchParams,
}: {
  searchParams: Promise<CollectionsSearchParams>
}) {
  const token = await getToken()
  if (!token) redirect("/auth/login")

  const resolvedSearchParams = await searchParams
  const user = await getCurrentUser()
  const shellProps = getShellProps(user)
  const canCreateCollection = !!user && hasPermission(user, "Create Collection")

  const idRaw = getSingleValue(resolvedSearchParams.id)
  if (idRaw) {
    const id = Number(idRaw)
    if (!Number.isFinite(id) || id <= 0) {
      return (
        <LibraryShell {...shellProps} searchPlaceholder="search for collections..">
          <h1 className="atlas-home-heading">Collection not found</h1>
          <p className="text-sm text-[var(--atlas-home-muted)]">
            Missing or invalid collection id.
          </p>
          <Button asChild className="mt-6" variant="outline">
            <Link href="/collections">Back to collections</Link>
          </Button>
        </LibraryShell>
      )
    }

    const result = await getCollectionById(id)
    const collection = result.data
    if (!collection) {
      const message = getUserFriendlyErrorMessage(result.error ?? "unknown")
      return (
        <LibraryShell {...shellProps} searchPlaceholder="search for collections..">
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
        </LibraryShell>
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
      <LibraryShell {...shellProps} searchPlaceholder="search for collections..">
        <div className="mb-6 space-y-4 border-b border-[var(--atlas-home-border-soft)] pb-6">
          <div className="text-sm text-[var(--atlas-home-muted)]">
            <Link href="/collections" className="text-[var(--atlas-home-link)] hover:underline">
              Collections
            </Link>
          </div>
          <div className="space-y-3">
            <h1 className="atlas-home-heading mb-0">{collection.name}</h1>
            {collection.hidden === "Y" ? (
              <Badge variant="outline" className="text-muted-foreground">
                Hidden from search
              </Badge>
            ) : null}
            <CollectionSectionNav hasReports={hasReports} hasTerms={hasTerms} />
          </div>
        </div>

        <div className="grid gap-10 xl:grid-cols-[4.75rem_minmax(0,1fr)]">
          <CollectionActionRail
            collection={collection}
            profilePanel={<ProfileAnalyticsPanel id={collection.id} type="collection" />}
          />

          <div className="min-w-0 space-y-12">
            {hasInitiative && collection.initiative ? (
              <CollectionDetailSection id="initiative" title="Owning Initiative">
                <div className="grid gap-4 md:grid-cols-2">
                  <InitiativeSnippetCard initiative={collection.initiative} />
                </div>
              </CollectionDetailSection>
            ) : null}

            {hasReports ? (
              <CollectionDetailSection id="reports" title="Reports">
                <div className="grid gap-4 md:grid-cols-2">
                  {sortedReports.map((report) => (
                    <ReportSnippetCard key={report.id} report={report} />
                  ))}
                </div>
              </CollectionDetailSection>
            ) : null}

            {hasTerms ? (
              <CollectionDetailSection id="terms" title="Terms">
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
              </CollectionDetailSection>
            ) : null}

            <CollectionDetailSection id="details" title="Details">
              <div className="content space-y-8">
                {collection.description ? (
                  <CollectionSubsection title="Description">
                    <MarkdownContent content={collection.description} />
                  </CollectionSubsection>
                ) : null}

                {collection.purpose ? (
                  <CollectionSubsection title="Search Summary">
                    <MarkdownContent content={collection.purpose} />
                  </CollectionSubsection>
                ) : null}
              </div>

              <CollectionMetadataTable collection={collection} />
            </CollectionDetailSection>
          </div>
        </div>
      </LibraryShell>
    )
  }

  const page = asPositiveInt(getSingleValue(resolvedSearchParams.page), 1)
  const pageSize = Math.min(100, asPositiveInt(getSingleValue(resolvedSearchParams.pageSize), 20))
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
      <LibraryShell {...shellProps} searchPlaceholder="search for collections..">
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
      </LibraryShell>
    )
  }

  const totalPages = Math.max(1, Math.ceil(list.total / list.pageSize))

  return (
    <LibraryShell {...shellProps} searchPlaceholder="search for collections..">
      <header className="space-y-3">
        <h1 className="atlas-home-heading mb-0">Collections</h1>
        {canCreateCollection ? (
          <div>
            <Link
              href="/collections/new"
              className="inline-flex items-center gap-1 rounded-md border border-[var(--atlas-home-border)] bg-white px-3 py-2 text-sm text-[var(--atlas-home-text)] hover:bg-[var(--atlas-home-surface-muted)]"
            >
              <span className="font-bold">+</span>
              Create a Collection
            </Link>
          </div>
        ) : null}
      </header>

      {list.collections.length === 0 ? (
        <p className="mt-4 text-sm text-[var(--atlas-home-muted)]">No collections found.</p>
      ) : (
        <div className="mt-4 grid gap-4 md:grid-cols-2">
          {list.collections.map((item) => (
            <CollectionsListCard key={item.id} collection={item} />
          ))}
        </div>
      )}

      {list.total > list.pageSize ? (
        <div className="mt-6 flex items-center justify-between gap-2 border-t border-[var(--atlas-home-border-soft)] pt-4 text-sm text-[var(--atlas-home-muted)]">
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
    </LibraryShell>
  )
}
