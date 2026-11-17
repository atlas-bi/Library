import type { Metadata } from "next"
import { redirect } from "next/navigation"
import { TasksPageTabs } from "@/components/tasks/tasks-page-tabs"
import { LibraryShell } from "@/components/layout/library-shell"
import { type AuthUser, getCurrentUser, getToken, hasPermission } from "@/lib/auth"
import { getUserFriendlyErrorMessage } from "@/lib/errors"
import { getTasks } from "@/lib/tasks/api"

export const metadata: Metadata = { title: "Tasks" }

function getShellDisplayName(user: AuthUser | null): string {
  if (!user) return "Guest"
  if (user.fullname && user.fullname !== "Guest") return user.fullname
  return user.username || "Guest"
}

export default async function TasksPage() {
  const token = await getToken()
  if (!token) redirect("/auth/login")

  const [user, tasksResult] = await Promise.all([getCurrentUser(), getTasks()])
  const canViewGroups = !!user && hasPermission(user, "View Groups")

  return (
    <LibraryShell
      displayName={getShellDisplayName(user)}
      isSignedIn={Boolean(user)}
      isAdministrator={user?.roles.includes("Administrator") ?? false}
      adminEnabled={user?.adminEnabled ?? false}
    >
      <h1 className="atlas-home-heading">Maintenance Tasks</h1>

      {tasksResult.error || !tasksResult.data ? (
        <p className="text-red-500">
          {getUserFriendlyErrorMessage(tasksResult.error ?? "unknown")}
        </p>
      ) : (
        <TasksPageTabs data={tasksResult.data} canViewGroups={canViewGroups} />
      )}
    </LibraryShell>
  )
}
