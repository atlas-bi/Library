import Link from "next/link"
import { redirect } from "next/navigation"
import { CollectionForm } from "@/components/collections/collection-form"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { getCurrentUser, getToken, hasPermission } from "@/lib/auth"

export default async function NewCollectionPage() {
  const token = await getToken()
  if (!token) redirect("/auth/login")

  const user = await getCurrentUser()
  if (!user || !hasPermission(user, "Create Collection")) {
    return (
      <div className="mx-auto max-w-3xl px-4 py-10">
        <Card>
          <CardHeader>
            <CardTitle className="text-xl">Permission required</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <p className="text-sm text-muted-foreground">
              You need the Create Collection permission to add collections.
            </p>
            <Button asChild variant="outline">
              <Link href="/collections">Back to collections</Link>
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
        <span>New</span>
      </div>
      <CollectionForm mode="create" cancelHref="/collections" />
    </div>
  )
}
