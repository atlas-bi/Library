import Link from "next/link"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import type { ProfileRunListItemDto } from "@/lib/profile/types"

export function ProfileRunListTable({ rows }: { rows: ProfileRunListItemDto[] }) {
  if (rows.length === 0) {
    return <p className="text-sm text-muted-foreground">No run data to show.</p>
  }

  const showType = rows.some((row) => row.type?.trim())

  return (
    <Table aria-label="Profile report runs">
      <TableHeader>
        <TableRow>
          <TableHead>Name</TableHead>
          {showType ? <TableHead>Type</TableHead> : null}
          <TableHead>Runs</TableHead>
          <TableHead>Last Run</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {rows.map((row) => (
          <TableRow key={`${row.name}-${row.url ?? ""}-${row.lastRun ?? ""}`}>
            <TableCell className="font-medium">
              {row.url ? (
                <Link href={row.url} className="text-link hover:underline">
                  {row.name}
                </Link>
              ) : (
                row.name
              )}
            </TableCell>
            {showType ? <TableCell>{row.type?.trim() || "—"}</TableCell> : null}
            <TableCell>{typeof row.runs === "number" ? row.runs : "—"}</TableCell>
            <TableCell>{row.lastRun?.trim() || "—"}</TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  )
}
