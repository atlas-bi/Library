import Link from "next/link"
import { redirect } from "next/navigation"
import { LibraryShell } from "@/components/layout/library-shell"
import { Button } from "@/components/ui/button"
import { getCurrentUser, getToken, hasPermission } from "@/lib/auth"

import { InitiativeForm } from "@/components/initiatives/initiative-form"

export default async function NewInitiativePage() {
  const token = await getToken()
  if (!token) redirect("/auth/login")

  const currentUser = await getCurrentUser()
  const canCreateInitiative = !!currentUser && hasPermission(currentUser, "Create Initiative")

  const shellProps = {
    displayName: currentUser?.fullname || currentUser?.username || "Guest",
    isSignedIn: !!currentUser,
    isAdministrator: currentUser?.roles.includes("Administrator") ?? false,
    adminEnabled: currentUser?.adminEnabled ?? false,
  }

  if (!canCreateInitiative) {
    return (
      <LibraryShell {...shellProps} searchPlaceholder="search for initiatives..">
        <h1 className="atlas-home-heading">Access Denied</h1>
        <p className="text-sm text-[var(--atlas-home-muted)]">
          You do not have permission to create initiatives.
        </p>
        <Button asChild className="mt-6" variant="outline">
          <Link href="/initiatives">Back to initiatives</Link>
        </Button>
      </LibraryShell>
    )
  }

  return (
    <LibraryShell {...shellProps} searchPlaceholder="search for initiatives..">
      <div className="mx-auto max-w-5xl px-4 py-8">
        <h1 className="mb-6 font-serif text-[3rem] font-bold leading-tight text-[#363636]">
          Create Initiative
        </h1>
        
        <InitiativeForm 
          mode="create" 
          cancelHref="/initiatives" 
        />
      </div>
    </LibraryShell>
  )
}
