import Link from "next/link"
import { redirect } from "next/navigation"
import { LibraryShell } from "@/components/layout/library-shell"
import { ProfileFullView } from "@/components/profile/profile-full-view"
import { Button } from "@/components/ui/button"
import { loadProfileAnalyticsAction } from "@/app/profile/actions"
import { getCurrentUser, getToken } from "@/lib/auth"

type ProfileSearchParams = {
  id?: string
  type?: string
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

export const metadata = {
  title: "Report Activity | Atlas",
  description: "View profile report activity, run history, stars, and subscriptions.",
}

export default async function ProfilePage({
  searchParams,
}: {
  searchParams: Promise<ProfileSearchParams>
}) {
  const token = await getToken()
  if (!token) redirect("/auth/login")

  const currentUser = await getCurrentUser()
  const resolvedSearchParams = await searchParams

  const idRaw = getSingleValue(resolvedSearchParams.id)
  const typeRaw = getSingleValue(resolvedSearchParams.type)

  const resolvedId = idRaw ? Number(idRaw) : Number(currentUser?.userId ?? -1)
  const resolvedType = typeRaw ?? "user"

  // Redirect bare /profile to /profile?id=<currentUserId>&type=user
  if (!idRaw && currentUser?.userId) {
    redirect(`/profile?id=${currentUser.userId}&type=user`)
  }

  // Fetch initial data server-side for fast first paint
  const analyticsResult = await loadProfileAnalyticsAction(resolvedId, resolvedType)

  if (analyticsResult.error === "auth_required") {
    redirect("/auth/login")
  }

  const shellProps = {
    displayName: getShellDisplayName(currentUser),
    isSignedIn: Boolean(currentUser),
    isAdministrator: currentUser?.roles.includes("Administrator") ?? false,
    adminEnabled: currentUser?.adminEnabled ?? false,
  }

  if (!analyticsResult.data) {
    return (
      <LibraryShell {...shellProps}>
        <div className="space-y-4">
          <h1 className="text-2xl font-bold tracking-tight">Unable to load profile</h1>
          <p className="text-sm text-muted-foreground">
            Profile analytics could not be loaded. The profile may not exist or you may not have
            access.
          </p>
          <Button asChild variant="outline">
            <Link href="/">Back to home</Link>
          </Button>
        </div>
      </LibraryShell>
    )
  }

  return (
    <LibraryShell {...shellProps}>
      <ProfileFullView
        id={resolvedId}
        type={resolvedType}
        initialData={analyticsResult.data}
        variant="page"
      />
    </LibraryShell>
  )
}
