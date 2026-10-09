import Link from "next/link"
import { redirect } from "next/navigation"
import { LibraryShell } from "@/components/layout/library-shell"
import { Button } from "@/components/ui/button"
import { getCurrentUser, getToken, hasPermission } from "@/lib/auth"
import { getInitiative } from "@/lib/initiatives/api"
import { getUserFriendlyErrorMessage } from "@/lib/errors"
import { InitiativeForm } from "@/components/initiatives/initiative-form"

type EditInitiativeSearchParams = {
  id?: string
}

function getSingleValue(value: string | string[] | undefined): string | undefined {
  if (typeof value === "string") return value
  if (Array.isArray(value)) return value[0]
  return undefined
}

export default async function EditInitiativePage({
  searchParams,
}: {
  searchParams: Promise<EditInitiativeSearchParams>
}) {
  const token = await getToken()
  if (!token) redirect("/auth/login")

  const currentUser = await getCurrentUser()
  const resolvedSearchParams = await searchParams
  
  const shellProps = {
    displayName: currentUser?.fullname || currentUser?.username || "Guest",
    isSignedIn: !!currentUser,
    isAdministrator: currentUser?.roles.includes("Administrator") ?? false,
    adminEnabled: currentUser?.adminEnabled ?? false,
  }

  const idRaw = getSingleValue(resolvedSearchParams.id)
  if (!idRaw) {
    return (
      <LibraryShell {...shellProps} searchPlaceholder="search for initiatives..">
        <h1 className="atlas-home-heading">Initiative not found</h1>
        <p className="text-sm text-[var(--atlas-home-muted)]">
          Missing or invalid initiative id.
        </p>
        <Button asChild className="mt-6" variant="outline">
          <Link href="/initiatives">Back to initiatives</Link>
        </Button>
      </LibraryShell>
    )
  }

  const id = Number(idRaw)
  if (!Number.isFinite(id) || id <= 0) {
    return (
      <LibraryShell {...shellProps} searchPlaceholder="search for initiatives..">
        <h1 className="atlas-home-heading">Initiative not found</h1>
        <p className="text-sm text-[var(--atlas-home-muted)]">
          Invalid initiative id format.
        </p>
        <Button asChild className="mt-6" variant="outline">
          <Link href="/initiatives">Back to initiatives</Link>
        </Button>
      </LibraryShell>
    )
  }

  const detailResult = await getInitiative(id)
  const initiative = detailResult.data
  
  if (!initiative) {
    const message = getUserFriendlyErrorMessage("unknown")
    return (
      <LibraryShell {...shellProps} searchPlaceholder="search for initiatives..">
        <h1 className="atlas-home-heading">Error</h1>
        <p className="text-sm text-[var(--atlas-home-muted)]">{message}</p>
        <Button asChild className="mt-6" variant="outline">
          <Link href="/initiatives">Back to initiatives</Link>
        </Button>
      </LibraryShell>
    )
  }

  // Permission Check: ensure the user has Edit permission for this specific initiative
  if (!initiative.canEditInitiative) {
    return (
      <LibraryShell {...shellProps} searchPlaceholder="search for initiatives..">
        <h1 className="atlas-home-heading">Access Denied</h1>
        <p className="text-sm text-[var(--atlas-home-muted)]">
          You do not have permission to edit this initiative.
        </p>
        <Button asChild className="mt-6" variant="outline">
          <Link href={`/initiatives?id=${id}`}>Back to initiative details</Link>
        </Button>
      </LibraryShell>
    )
  }

  return (
    <LibraryShell {...shellProps} searchPlaceholder="search for initiatives..">
      <InitiativeForm 
        mode="edit" 
        initial={initiative}
        cancelHref={`/initiatives?id=${id}`} 
      />
    </LibraryShell>
  )
}
