import Link from "next/link"
import { redirect } from "next/navigation"
import { DeleteCollectionButton } from "@/components/collections/delete-collection-button"
import { ShareMailDialog } from "@/components/interactions/share-mail-dialog"
import { StarToggleButton } from "@/components/interactions/star-toggle-button"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { getCurrentUser, getToken, hasPermission } from "@/lib/auth"
import { getCollectionById, getCollectionsList } from "@/lib/collections/api"
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

    const initiativeId = collection.initiative?.id

    return (
      <div className="mx-auto min-h-screen w-full max-w-5xl space-y-6 px-4 py-8">
        <div className="flex flex-wrap items-start justify-between gap-4">
          <div>
            <div className="text-sm text-muted-foreground">
              <Link href="/collections" className="hover:underline">
                Collections
              </Link>
              <span className="px-1">/</span>
              <span>Detail</span>
            </div>
            <h1 className="mt-2 text-3xl font-bold">{collection.name}</h1>
            <div className="mt-2 flex flex-wrap gap-2">
              {collection.hidden ? (
                <Badge variant="outline">Hidden: {collection.hidden}</Badge>
              ) : null}
            </div>
          </div>
          <div className="flex flex-wrap items-center gap-2">
            <StarToggleButton
              type="collection"
              id={collection.id}
              initialStarred={collection.isStarred ?? false}
              initialCount={collection.starCount ?? 0}
            />
            <ShareMailDialog
              shareName={collection.name}
              shareUrl={`/collections?id=${collection.id}`}
            />
            {collection.canEditCollection ? (
              <Button asChild variant="outline">
                <Link href={`/collections/edit?id=${collection.id}`}>Edit</Link>
              </Button>
            ) : null}
          </div>
        </div>

        {collection.description ? (
          <p className="text-sm text-muted-foreground">{collection.description}</p>
        ) : null}
        {collection.purpose ? (
          <Card>
            <CardHeader>
              <CardTitle className="text-base">Purpose</CardTitle>
            </CardHeader>
            <CardContent className="text-sm text-muted-foreground">
              {collection.purpose}
            </CardContent>
          </Card>
        ) : null}

        <div className="grid gap-4 text-sm text-muted-foreground md:grid-cols-2">
          {collection.lastModifiedDisplay ? (
            <div>Updated: {collection.lastModifiedDisplay}</div>
          ) : null}
          {collection.lastUpdatedBy?.fullName || collection.lastUpdatedBy?.username ? (
            <div>
              Last updated by:{" "}
              {collection.lastUpdatedBy.fullName?.trim() ||
                collection.lastUpdatedBy.username ||
                "—"}
            </div>
          ) : null}
        </div>

        {initiativeId ? (
          <Card>
            <CardHeader>
              <CardTitle className="text-base">Initiative</CardTitle>
            </CardHeader>
            <CardContent>
              <Link
                href={`/initiatives?id=${initiativeId}`}
                className="text-sm font-medium hover:underline"
              >
                {collection.initiative?.name ?? `Initiative ${initiativeId}`}
              </Link>
            </CardContent>
          </Card>
        ) : null}

        <div className="grid gap-6 lg:grid-cols-2">
          <Card>
            <CardHeader>
              <CardTitle className="text-base">Terms</CardTitle>
            </CardHeader>
            <CardContent>
              {collection.terms && collection.terms.length > 0 ? (
                <ul className="space-y-2 text-sm">
                  {collection.terms.map((term) => {
                    const termId = typeof term.id === "number" ? term.id : term.termId
                    const label = term.name?.trim() || (termId ? `Term ${termId}` : "Term")
                    return (
                      <li key={`${termId ?? label}-${term.rank ?? 0}`}>
                        {termId ? (
                          <Link href={`/terms?id=${termId}`} className="hover:underline">
                            {label}
                          </Link>
                        ) : (
                          label
                        )}
                        {typeof term.rank === "number" ? (
                          <span className="ml-2 text-xs text-muted-foreground">
                            rank {term.rank}
                          </span>
                        ) : null}
                      </li>
                    )
                  })}
                </ul>
              ) : (
                <p className="text-sm text-muted-foreground">No linked terms.</p>
              )}
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle className="text-base">Reports</CardTitle>
            </CardHeader>
            <CardContent>
              {collection.reports && collection.reports.length > 0 ? (
                <ul className="space-y-2 text-sm">
                  {collection.reports.map((report) => (
                    <li key={report.id}>
                      <Link href={`/reports?id=${report.id}`} className="hover:underline">
                        {report.name?.trim() || `Report ${report.id}`}
                      </Link>
                      {typeof report.rank === "number" ? (
                        <span className="ml-2 text-xs text-muted-foreground">
                          rank {report.rank}
                        </span>
                      ) : null}
                      {report.canRun ? (
                        <Badge className="ml-2" variant="outline">
                          Can run
                        </Badge>
                      ) : null}
                    </li>
                  ))}
                </ul>
              ) : (
                <p className="text-sm text-muted-foreground">No linked reports.</p>
              )}
            </CardContent>
          </Card>
        </div>

        {collection.canDeleteCollection ? (
          <Card>
            <CardHeader>
              <CardTitle className="text-base">Danger zone</CardTitle>
            </CardHeader>
            <CardContent>
              <DeleteCollectionButton
                collectionId={collection.id}
                collectionName={collection.name}
              />
            </CardContent>
          </Card>
        ) : null}
      </div>
    )
  }

  const page = asPositiveInt(getSingleValue(searchParams.page), 1)
  const pageSize = Math.min(100, asPositiveInt(getSingleValue(searchParams.pageSize), 20))
  const listResult = await getCollectionsList(page, pageSize)
  const list = listResult.data

  if (!list) {
    const message = getUserFriendlyErrorMessage(listResult.error ?? "unknown")
    return (
      <div className="mx-auto max-w-4xl px-4 py-10">
        <Card>
          <CardHeader>
            <CardTitle className="text-xl">Unable to load collections</CardTitle>
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

  const totalPages = Math.max(1, Math.ceil(list.total / list.pageSize))

  return (
    <div className="mx-auto min-h-screen w-full max-w-5xl space-y-6 px-4 py-8">
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
          <p className="mt-1 text-sm text-muted-foreground">
            {list.total} total • page {list.page} of {totalPages}
          </p>
        </div>
        {canCreateCollection ? (
          <Button asChild>
            <Link href="/collections/new">New collection</Link>
          </Button>
        ) : null}
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">All collections</CardTitle>
        </CardHeader>
        <CardContent className="space-y-3">
          {list.collections.length === 0 ? (
            <p className="text-sm text-muted-foreground">No collections found.</p>
          ) : (
            <ul className="divide-y rounded-md border">
              {list.collections.map((item) => (
                <li
                  key={item.id}
                  className="flex flex-wrap items-center justify-between gap-3 px-3 py-3"
                >
                  <div className="min-w-0">
                    <Link
                      href={`/collections?id=${item.id}`}
                      className="font-medium hover:underline"
                    >
                      {item.name}
                    </Link>
                    {item.description ? (
                      <p className="mt-1 line-clamp-2 text-sm text-muted-foreground">
                        {item.description}
                      </p>
                    ) : null}
                  </div>
                  <div className="flex items-center gap-2">
                    {typeof item.isStarred === "boolean" ? (
                      <Badge variant={item.isStarred ? "default" : "outline"}>
                        {item.isStarred ? "Starred" : "Unstarred"}
                      </Badge>
                    ) : null}
                  </div>
                </li>
              ))}
            </ul>
          )}
        </CardContent>
      </Card>

      {list.total > list.pageSize ? (
        <div className="flex items-center justify-end gap-2">
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
      ) : null}
    </div>
  )
}
