import Link from "next/link"
import type { HomeGroupsPanel } from "@/lib/home/types"

export function HomeGroupsPanelView({ panel }: { panel: HomeGroupsPanel }) {
  if (panel.rows.length === 0) {
    return (
      <div className="atlas-home-card p-6 text-sm text-[var(--atlas-home-text)]">
        {panel.emptyMessage}
      </div>
    )
  }

  return (
    <div className="atlas-home-card overflow-hidden">
      <table className="atlas-home-table">
        <thead className="bg-transparent text-[var(--atlas-home-title)]">
          <tr>
            <th className="px-4 py-3 font-semibold">Group Name</th>
            <th className="px-4 py-3 font-semibold">Type</th>
            <th className="px-4 py-3 font-semibold">Source</th>
          </tr>
        </thead>
        <tbody>
          {panel.rows.map((row) => (
            <tr key={row.id}>
              <td className="px-4 py-3 font-semibold text-[var(--atlas-home-title)]">
                {row.href ? (
                  <Link href={row.href} className="hover:underline">
                    {row.name}
                  </Link>
                ) : (
                  row.name
                )}
              </td>
              <td className="px-4 py-3">{row.type || "-"}</td>
              <td className="px-4 py-3">{row.source || "-"}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
