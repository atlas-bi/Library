import Link from "next/link"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import type { UserGroup } from "@/lib/users/types"

export function UserGroupsTable({
  rows,
  canViewGroups,
}: {
  rows: UserGroup[]
  canViewGroups: boolean
}) {
  if (rows.length === 0) {
    return (
      <div className="atlas-home-card p-6 text-sm text-[var(--atlas-home-text)]">
        No groups to show.
      </div>
    )
  }

  return (
    <div className="atlas-home-card overflow-hidden">
      <Table aria-label="User groups" className="atlas-home-table">
        <TableHeader>
          <TableRow>
            <TableHead>Group Name</TableHead>
            <TableHead>Type</TableHead>
            <TableHead>Source</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {rows.map((group) => (
            <TableRow key={group.id}>
              <TableCell className="font-medium">
                {canViewGroups ? (
                  <Link
                    href={`/groups?id=${group.id}`}
                    className="text-[var(--atlas-home-link)] hover:underline"
                  >
                    {group.name?.trim() || `Group ${group.id}`}
                  </Link>
                ) : (
                  group.name?.trim() || `Group ${group.id}`
                )}
              </TableCell>
              <TableCell>{group.type?.trim() || "—"}</TableCell>
              <TableCell>{group.source?.trim() || "—"}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  )
}
