import Link from "next/link"
import { redirect } from "next/navigation"
import { CollectionForm } from "@/components/collections/collection-form"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { getCurrentUser, getToken, hasPermission } from "@/lib/auth"
import { getCollectionById } from "@/lib/collections/api"

type EditSearchParams = { id?: string }

function getSingleValue(value: string | string[] | undefined): string | undefined {
  if (typeof value === "string") return value
  if (Array.isArray(value)) return value[0]
  return undefined
}

export default async function EditCollectionPage({
  searchParams,
}: {
  searchParams: EditSearchParams
}) {
  const token = await getToken()
  if (!token) redirect("/auth/login")

  const user = await getCurrentUser()
  if (!user || !hasPermission(user, "Edit Collection")) {
    return (
      <div className="mx-auto max-w-3xl px-4 py-10">
        <Card>
          <CardHeader>
            <CardTitle className="text-xl">Permission required</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <p className="text-sm text-muted-foreground">
              You need the Edit Collection permission to update collections.
            </p>
            <Button asChild variant="outline">
              <Link href="/collections">Back to collections</Link>
            </Button>
          </CardContent>
        </Card>
      </div>
    )
  }

  const idRaw = getSingleValue(searchParams.id)
  const id = idRaw ? Number(idRaw) : NaN
  if (!Number.isFinite(id) || id <= 0) {
    return (
      <div className="mx-auto max-w-3xl px-4 py-10">
        <h1 className="text-2xl font-bold">Missing collection</h1>
        <p className="mt-2 text-sm text-muted-foreground">
          Provide a valid collection id in the URL.
        </p>
        <Button asChild className="mt-6" variant="outline">
          <Link href="/collections">Back to collections</Link>
        </Button>
      </div>
    )
  }

  const result = await getCollectionById(id)
  const collection = result.data
  if (!collection) {
    return (
      <div className="mx-auto max-w-3xl px-4 py-10">
        <Card>
          <CardHeader>
            <CardTitle className="text-xl">Unable to load collection</CardTitle>
          </CardHeader>
          <CardContent>
            <Button asChild variant="outline">
              <Link href="/collections">Back to collections</Link>
            </Button>
          </CardContent>
        </Card>
      </div>
    )
  }

  if (!collection.canEditCollection) {
    return (
      <div className="mx-auto max-w-3xl px-4 py-10">
        <Card>
          <CardHeader>
            <CardTitle className="text-xl">Editing not available</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <p className="text-sm text-muted-foreground">
              You do not have access to edit this collection.
            </p>
            <Button asChild variant="outline">
              <Link href={`/collections?id=${collection.id}`}>Back to collection</Link>
            </Button>
          </CardContent>
        </Card>
      </div>
    )
  }

  return (
    <div className="mx-auto max-w-5xl space-y-6 px-4 py-8">
      <div className="text-sm text-muted-foreground">
        <Link href="/collections" className="hover:underline">
          Collections
        </Link>
        <span className="px-1">/</span>
        <Link href={`/collections?id=${collection.id}`} className="hover:underline">
          {collection.name}
        </Link>
        <span className="px-1">/</span>
        <span>Edit</span>
      </div>
      <CollectionForm
        mode="edit"
        collectionId={collection.id}
        initial={collection}
        cancelHref={`/collections?id=${collection.id}`}
      />
    </div>
  )
}
