import Link from "next/link"
import { redirect } from "next/navigation"
import { LibraryShell } from "@/components/layout/library-shell"
import { Button } from "@/components/ui/button"
import { InitiativeDetail } from "@/components/initiatives/initiative-detail"
import { InitiativesIndex } from "@/components/initiatives/initiatives-index"
import { getCurrentUser, getToken, hasPermission } from "@/lib/auth"
import { getUserFriendlyErrorMessage } from "@/lib/errors"
import { getInitiative, getInitiatives } from "@/lib/initiatives/api"

type InitiativesSearchParams = {
  id?: string
}

function getSingleValue(value: string | string[] | undefined): string | undefined {
  if (typeof value === "string") return value
  if (Array.isArray(value)) return value[0]
  return undefined
}

function getShellDisplayName(
  user: {
    fullname?: string | null
    username?: string | null
  } | null,
) {
  return user?.fullname?.trim() || user?.username?.trim() || "Guest"
}

export default async function InitiativesPage({
  searchParams,
}: {
  searchParams: Promise<InitiativesSearchParams>
}) {
  const token = await getToken()
  if (!token) redirect("/auth/login")

  const currentUser = await getCurrentUser()
  const resolvedSearchParams = await searchParams
  const idRaw = getSingleValue(resolvedSearchParams.id)
  const resolvedId = idRaw ? Number(idRaw) : undefined

  // Detail View
  if (resolvedId !== undefined && Number.isFinite(resolvedId) && resolvedId > 0) {
    const detailResult = await getInitiative(resolvedId)
    const initiative = detailResult.data

    if (!initiative) {
      const message = getUserFriendlyErrorMessage("unknown")
      const denied = false

      return (
        <LibraryShell
          displayName={getShellDisplayName(currentUser)}
          isSignedIn={Boolean(currentUser)}
          isAdministrator={currentUser?.roles.includes("Administrator") ?? false}
          adminEnabled={currentUser?.adminEnabled ?? false}
        >
          <div className="space-y-4">
            <h1 className="text-2xl font-bold tracking-tight">
              {denied ? "Access denied" : "Unable to load initiative"}
            </h1>
            <p className="text-sm text-muted-foreground">
              {denied ? "You do not have access to this initiative." : message}
            </p>
            <Button asChild variant="outline">
              <Link href="/initiatives">Back to initiatives</Link>
            </Button>
          </div>
        </LibraryShell>
      )
    }

    const canViewOtherUser = !!currentUser && hasPermission(currentUser, "View Other User")
    const canCreateInitiative = !!currentUser && hasPermission(currentUser, "Create Initiative")

    return (
      <LibraryShell
        displayName={getShellDisplayName(currentUser)}
        isSignedIn={Boolean(currentUser)}
        isAdministrator={currentUser?.roles.includes("Administrator") ?? false}
        adminEnabled={currentUser?.adminEnabled ?? false}
      >
        <div className="mb-4">
          <Link href="/initiatives" className="text-sm text-muted-foreground hover:underline">
            &larr; Back to initiatives
          </Link>
        </div>
        <InitiativeDetail 
          data={initiative} 
          canViewOtherUser={canViewOtherUser} 
          canCreateInitiative={canCreateInitiative} 
        />
      </LibraryShell>
    )
  }

  // Index View
  const listResult = await getInitiatives()
  const listData = listResult.data

  if (!listData) {
    const message = getUserFriendlyErrorMessage("unknown")
    const denied = false

    return (
      <LibraryShell
        displayName={getShellDisplayName(currentUser)}
        isSignedIn={Boolean(currentUser)}
        isAdministrator={currentUser?.roles.includes("Administrator") ?? false}
        adminEnabled={currentUser?.adminEnabled ?? false}
      >
        <div className="space-y-4">
          <h1 className="text-2xl font-bold tracking-tight">
            {denied ? "Access denied" : "Unable to load initiatives"}
          </h1>
          <p className="text-sm text-muted-foreground">
            {denied ? "You do not have access to this page." : message}
          </p>
          <Button asChild variant="outline">
            <Link href="/">Back to home</Link>
          </Button>
        </div>
      </LibraryShell>
    )
  }

  const canCreateInitiative = !!currentUser && hasPermission(currentUser, "Create Initiative")

  return (
    <LibraryShell
      displayName={getShellDisplayName(currentUser)}
      isSignedIn={Boolean(currentUser)}
      isAdministrator={currentUser?.roles.includes("Administrator") ?? false}
      adminEnabled={currentUser?.adminEnabled ?? false}
    >
      <InitiativesIndex data={listData} canCreateInitiative={canCreateInitiative} />
    </LibraryShell>
  )
}
