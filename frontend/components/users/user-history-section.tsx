import Link from "next/link"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import type { UserHistoryItem, UserHistorySection } from "@/lib/users/types"

function HistoryLinks({ items }: { items: UserHistoryItem[] }) {
  if (items.length === 0) return null

  return (
    <ul className="space-y-2 text-sm">
      {items.map((item, index) => (
        <li key={`${item.url ?? item.name ?? index}`}>
          {item.url ? (
            <Link href={item.url} className="text-link hover:underline">
              {item.name?.trim() || item.url}
            </Link>
          ) : (
            item.name?.trim() || "Untitled"
          )}
          {item.date ? <span className="text-muted-foreground"> · {item.date}</span> : null}
        </li>
      ))}
    </ul>
  )
}

export function UserHistorySectionView({ history }: { history: UserHistorySection }) {
  const hasEdits =
    history.reportEdits.length > 0 ||
    history.initiativeEdits.length > 0 ||
    history.collectionEdits.length > 0 ||
    history.termEdits.length > 0
  const hasAtlasHistory = history.atlasHistory.length > 0

  if (!hasEdits && !hasAtlasHistory) {
    return <p className="text-sm text-muted-foreground">No activity to show ¯\_(ツ)_/¯</p>
  }

  return (
    <div className="space-y-8">
      {hasEdits ? (
        <section className="space-y-4">
          <h3 className="text-lg font-semibold">Recent Edits - Last 30 Days</h3>
          {history.reportEdits.length > 0 ? (
            <div className="space-y-2">
              <h4 className="text-sm font-medium">Reports</h4>
              <HistoryLinks items={history.reportEdits} />
            </div>
          ) : null}
          {history.initiativeEdits.length > 0 ? (
            <div className="space-y-2">
              <h4 className="text-sm font-medium">Initiatives</h4>
              <HistoryLinks items={history.initiativeEdits} />
            </div>
          ) : null}
          {history.collectionEdits.length > 0 ? (
            <div className="space-y-2">
              <h4 className="text-sm font-medium">Collections</h4>
              <HistoryLinks items={history.collectionEdits} />
            </div>
          ) : null}
          {history.termEdits.length > 0 ? (
            <div className="space-y-2">
              <h4 className="text-sm font-medium">Terms</h4>
              <HistoryLinks items={history.termEdits} />
            </div>
          ) : null}
        </section>
      ) : null}

      {hasAtlasHistory ? (
        <section className="space-y-4">
          <h3 className="text-lg font-semibold">Atlas Browsing History - Last 7 Days</h3>
          <Table aria-label="Atlas browsing history">
            <TableHeader>
              <TableRow>
                <TableHead>Area</TableHead>
                <TableHead>Page</TableHead>
                <TableHead>Date</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {history.atlasHistory.map((item, index) => (
                <TableRow key={`${item.url ?? item.type ?? index}`}>
                  <TableCell>{item.type?.trim() || "—"}</TableCell>
                  <TableCell>
                    {item.url ? (
                      <Link href={item.url} className="text-link hover:underline">
                        {item.url}
                      </Link>
                    ) : (
                      "—"
                    )}
                  </TableCell>
                  <TableCell>{item.date?.trim() || "—"}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </section>
      ) : null}
    </div>
  )
}
