import Link from "next/link"
import type { GroupReport } from "@/lib/groups/types"

export function GroupReportsTable({ reports }: { reports: GroupReport[] }) {
  if (reports.length === 0) {
    return <h3 className="text-lg font-semibold">No Reports With This Group</h3>
  }

  return (
    <div className="space-y-4">
      <h3 className="text-lg font-semibold">Reports With This Group</h3>
      <div className="overflow-x-auto rounded-md border">
        <table
          className="w-full min-w-[720px] text-left text-sm"
          aria-label="Reports with this group"
        >
          <thead className="border-b bg-muted/30 text-muted-foreground">
            <tr>
              <th scope="col" className="px-4 py-3 font-semibold">
                Name
              </th>
              <th scope="col" className="px-4 py-3 font-semibold">
                Last Updated
              </th>
              <th scope="col" className="px-4 py-3 font-semibold">
                # Subscriptions
              </th>
              <th scope="col" className="px-4 py-3 font-semibold">
                # Favorites
              </th>
              <th scope="col" className="px-4 py-3 font-semibold">
                # Runs (2yrs)
              </th>
            </tr>
          </thead>
          <tbody>
            {reports.map((report) => (
              <tr key={report.id} className="border-b last:border-b-0">
                <td className="px-4 py-3 font-medium">
                  <Link
                    href={report.url ?? `/reports?id=${report.id}`}
                    className="text-link hover:underline"
                  >
                    {report.name?.trim() || `Report ${report.id}`}
                  </Link>
                </td>
                <td className="px-4 py-3">{report.lastUpdated?.trim() || "—"}</td>
                <td className="px-4 py-3">{report.subscriptionCount ?? 0}</td>
                <td className="px-4 py-3">{report.favoriteCount ?? 0}</td>
                <td className="px-4 py-3">{report.runCount ?? 0}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}
