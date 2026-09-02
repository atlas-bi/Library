import type { Metadata } from "next"
import { redirect } from "next/navigation"
import type { ReactNode } from "react"
import { LibraryShell } from "@/components/layout/library-shell"
import { SettingsTabController } from "@/components/settings/settings-tab-controller"
import { getCurrentUser, getToken, hasPermission } from "@/lib/auth"

export const metadata: Metadata = {
  title: "Settings",
  description: "Admin settings for Atlas Library.",
}

export default async function SettingsLayout({ children }: { children: ReactNode }) {
  const token = await getToken()
  if (!token) redirect("/auth/login")

  const user = await getCurrentUser()
  const canEditRoles = !!user && hasPermission(user, "Edit Role Permissions")
  const canEditUsers = !!user && hasPermission(user, "Edit User Permissions")
  const canEditGroups = !!user && hasPermission(user, "Edit Group Permissions")

  const navItems = [
    { href: "#roles", label: "Role Configuration", icon: "fa-lock", show: canEditRoles },
    { href: "#user-roles", label: "User Roles", icon: "fa-user-lock", show: canEditUsers },
    { href: "#user-groups", label: "Group Roles", icon: "fa-users", show: canEditGroups },
    { href: "#meta-fields", label: "Meta Fields", icon: "fa-list-ul", show: true },
    { href: "#site-message", label: "Site Message", icon: "fa-comment", show: true },
    { href: "#search", label: "Search", icon: "fa-search", show: true },
    { href: "#theme", label: "Theme", icon: "fa-palette", show: true },
    { href: "#etl", label: "ETL", icon: "fa-database", show: true },
  ].filter((item) => item.show)

  const defaultTab = canEditRoles ? "roles" : canEditUsers ? "user-roles" : "meta-fields"

  return (
    <LibraryShell
      displayName={user?.fullname?.trim() || user?.username?.trim() || "Guest"}
      isSignedIn={Boolean(user)}
      isAdministrator={user?.roles?.includes("Administrator") ?? false}
      adminEnabled={user?.adminEnabled ?? false}
    >
      <div className="max-w-[1280px] mx-auto py-8 px-4">
        <div className="text-[14px] font-medium mb-4 flex items-center">
          <span className="text-[#363636]">Settings</span>
          <span className="text-[#dbdbdb] mx-2">/</span>
          <span className="text-[#485fc7]">Home</span>
        </div>

        <h1 className="text-[40px] font-bold font-serif text-[#363636] mb-6 tracking-tight leading-tight">
          Settings
        </h1>

        <div className="flex flex-col md:flex-row gap-[1.5rem] items-start">
          <div className="w-full md:w-[280px] shrink-0">
            <nav className="bg-white rounded-[6px] shadow-[0_0.5em_1em_-0.125em_rgba(10,10,10,0.1),0_0_0_1px_rgba(10,10,10,0.02)] overflow-hidden flex flex-col">
              {navItems.map((item) => (
                <a
                  key={item.href}
                  href={item.href}
                  className="panel-tab flex items-center px-4 py-[0.875rem] text-[15px] font-medium transition-colors border-b border-[#ededed] last:border-b-0 text-[#363636] hover:bg-[#f5f5f5]"
                >
                  <span className="w-7 text-center mr-3 text-lg text-[#4a4a4a]">
                    <i className={`fas ${item.icon}`} aria-hidden="true" />
                  </span>
                  {item.label}
                </a>
              ))}
            </nav>
          </div>

          <div className="flex-1 min-w-0 w-full">
            <div className="bg-white rounded-[6px] shadow-[0_0.5em_1em_-0.125em_rgba(10,10,10,0.1),0_0_0_1px_rgba(10,10,10,0.02)] p-8">
              <SettingsTabController defaultTab={defaultTab}>{children}</SettingsTabController>
            </div>
          </div>
        </div>
      </div>
    </LibraryShell>
  )
}
