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
    return <p className="text-sm text-muted-foreground">No groups to show.</p>
  }

  return (
    <Table aria-label="User groups">
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
                <Link href={`/groups?id=${group.id}`} className="text-link hover:underline">
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
  )
}
