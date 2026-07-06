import Link from "next/link"
import { redirect } from "next/navigation"
import { LibraryShell } from "@/components/layout/library-shell"
import { TermForm } from "@/components/terms/term-form"
import { Button } from "@/components/ui/button"
import { type AuthUser, getCurrentUser, getToken } from "@/lib/auth"
import { getTermsList } from "@/lib/terms/api"

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

export default async function NewTermPage() {
  const token = await getToken()
  if (!token) redirect("/auth/login")

  const user = await getCurrentUser()
  const shellProps = getShellProps(user)

  const listResult = await getTermsList()
  const list = listResult.data

  if (!list?.permissions?.canCreateTerm) {
    return (
      <LibraryShell {...shellProps}>
        <h1 className="atlas-home-heading text-destructive border-transparent pb-0 mb-2">Access Denied</h1>
        <p className="text-sm text-[var(--atlas-home-muted)] mb-6">
          You do not have permission to create terms.
        </p>
        <Button asChild variant="outline">
          <Link href="/terms">Back to terms</Link>
        </Button>
      </LibraryShell>
    )
  }

  return (
    <LibraryShell {...shellProps}>
      <div className="py-6 xl:py-10">
        <TermForm
          mode="create"
          cancelHref="/terms"
          canApproveTerm={list.permissions.canApproveTerm ?? false}
        />
      </div>
    </LibraryShell>
  )
}