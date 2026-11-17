import Link from "next/link"
import type { ReportListItem } from "@/lib/reports/types"

export function ReportsListTable({ reports }: { reports: ReportListItem[] }) {
  if (reports.length === 0) {
    return <p className="text-sm text-[var(--atlas-home-muted)]">No reports found.</p>
  }

  return (
    <div className="atlas-home-card overflow-hidden">
      <table className="atlas-home-table">
        <thead className="bg-transparent text-[var(--atlas-home-title)]">
          <tr>
            <th className="px-4 py-3 font-semibold">Name</th>
            <th className="px-4 py-3 font-semibold">Type</th>
            <th className="px-4 py-3 font-semibold">Last Modified</th>
          </tr>
        </thead>
        <tbody>
          {reports.map((report) => {
            const title = report.displayTitle || report.displayName || report.name
            return (
              <tr key={report.id}>
                <td className="px-4 py-3 font-semibold text-[var(--atlas-home-title)]">
                  <Link href={`/reports?id=${report.id}`} className="hover:underline">
                    {title}
                  </Link>
                </td>
                <td className="px-4 py-3">{report.type ?? report.typeShortName ?? "-"}</td>
                <td className="px-4 py-3">
                  {report.lastModified
                    ? new Date(report.lastModified).toLocaleDateString()
                    : "-"}
                </td>
              </tr>
            )
          })}
        </tbody>
      </table>
    </div>
  )
}
