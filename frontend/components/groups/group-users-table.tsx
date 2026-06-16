import Link from "next/link"
import type { GroupUser } from "@/lib/groups/types"

export function GroupUsersTable({ users }: { users: GroupUser[] }) {
  if (users.length === 0) {
    return <h3 className="text-lg font-semibold">No Users With This Group</h3>
  }

  return (
    <div className="space-y-4">
      <h3 className="text-lg font-semibold">Users With This Group</h3>
      <div className="overflow-x-auto rounded-md border">
        <table
          className="w-full min-w-[640px] text-left text-sm"
          aria-label="Users with this group"
        >
          <thead className="border-b bg-muted/30 text-muted-foreground">
            <tr>
              <th scope="col" className="px-4 py-3 font-semibold">
                Name
              </th>
              <th scope="col" className="px-4 py-3 font-semibold">
                Email
              </th>
              <th scope="col" className="px-4 py-3 font-semibold">
                Phone
              </th>
              <th scope="col" className="px-4 py-3 font-semibold">
                Epic Id
              </th>
              <th scope="col" className="px-4 py-3 font-semibold">
                Employee Id
              </th>
            </tr>
          </thead>
          <tbody>
            {users.map((user) => (
              <tr key={user.id} className="border-b last:border-b-0">
                <td className="px-4 py-3 font-medium">
                  {user.canOpenUserProfile && user.url ? (
                    <Link href={user.url} className="text-link hover:underline">
                      {user.name?.trim() || `User ${user.id}`}
                    </Link>
                  ) : (
                    user.name?.trim() || `User ${user.id}`
                  )}
                </td>
                <td className="px-4 py-3">
                  {user.email?.trim() ? (
                    <a href={`mailto:${user.email.trim()}`} className="text-link hover:underline">
                      {user.email.trim()}
                    </a>
                  ) : null}
                </td>
                <td className="px-4 py-3">{user.phone?.trim() || "—"}</td>
                <td className="px-4 py-3">{user.epicId?.trim() || "—"}</td>
                <td className="px-4 py-3">{user.employeeId?.trim() || "—"}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}
