import Link from "next/link"
import { Table, TableBody, TableCell, TableRow } from "@/components/ui/table"
import type { ProfileStarUserDto } from "@/lib/profile/types"

export function ProfileStarsTable({
  rows,
  userProfilesEnabled = true,
}: {
  rows: ProfileStarUserDto[]
  userProfilesEnabled?: boolean
}) {
  if (rows.length === 0) {
    return <p className="text-sm font-medium">There are no stars.</p>
  }

  const sorted = [...rows].sort((a, b) =>
    (a.fullName ?? a.email ?? "").localeCompare(b.fullName ?? b.email ?? ""),
  )

  return (
    <Table aria-label="Profile stars">
      <TableBody>
        {sorted.map((user) => {
          const label = user.fullName ?? user.email ?? `User ${user.id}`
          return (
            <TableRow key={user.id}>
              <TableCell>
                {userProfilesEnabled ? (
                  <Link href={`/users?id=${user.id}`} className="text-link hover:underline">
                    {label}
                  </Link>
                ) : (
                  label
                )}
              </TableCell>
            </TableRow>
          )
        })}
      </TableBody>
    </Table>
  )
}
