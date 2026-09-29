import Link from "next/link"
import { redirect } from "next/navigation"
import { LibraryShell } from "@/components/layout/library-shell"
import { TermForm } from "@/components/terms/term-form"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { type AuthUser, getCurrentUser, getToken } from "@/lib/auth"
import { getUserFriendlyErrorMessage } from "@/lib/errors"
import { getTermById } from "@/lib/terms/api"

type TermEditSearchParams = {
  id?: string
}

function getSingleValue(value: string | string[] | undefined): string | undefined {
  if (typeof value === "string") return value
  if (Array.isArray(value)) return value[0]
  return undefined
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

export default async function EditTermPage({
  searchParams,
}: {
  searchParams: Promise<TermEditSearchParams>
}) {
  const token = await getToken()
  if (!token) redirect("/auth/login")

  const resolvedSearchParams = await searchParams
  const user = await getCurrentUser()
  const shellProps = getShellProps(user)

  const idRaw = getSingleValue(resolvedSearchParams.id)
  const id = idRaw ? Number(idRaw) : NaN
  if (!Number.isFinite(id) || id <= 0) {
    return (
      <LibraryShell {...shellProps}>
        <h1 className="atlas-home-heading">Term not found</h1>
        <p className="text-sm text-[var(--atlas-home-muted)]">Missing or invalid term id.</p>
        <Button asChild className="mt-6" variant="outline">
          <Link href="/terms">Back to terms</Link>
        </Button>
      </LibraryShell>
    )
  }

  const result = await getTermById(id)
  const term = result.data

  if (!term) {
    const message = getUserFriendlyErrorMessage(result.error ?? "unknown")
    return (
      <LibraryShell {...shellProps}>
        <Card>
          <CardHeader>
            <CardTitle className="text-xl">Unable to load term</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <p className="text-sm text-muted-foreground">{message}</p>
            <Button asChild variant="outline">
              <Link href="/terms">Back to terms</Link>
            </Button>
          </CardContent>
        </Card>
      </LibraryShell>
    )
  }

  if (!term.permissions?.canEditTerm) {
    return (
      <LibraryShell {...shellProps}>
        <h1 className="atlas-home-heading text-destructive border-transparent pb-0 mb-2">Access Denied</h1>
        <p className="text-sm text-[var(--atlas-home-muted)] mb-6">
          You do not have permission to edit this term.
        </p>
        <Button asChild variant="outline">
          <Link href={`/terms?id=${term.id}`}>Back to term</Link>
        </Button>
      </LibraryShell>
    )
  }

  return (
    <LibraryShell {...shellProps}>
      <div className="py-6 xl:py-10">
        <TermForm
          mode="edit"
          termId={term.id}
          initial={term}
          cancelHref={`/terms?id=${term.id}`}
          canApproveTerm={term.permissions?.canApproveTerm ?? false}
        />
      </div>
    </LibraryShell>
  )
}