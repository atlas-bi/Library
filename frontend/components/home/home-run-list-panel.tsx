import Link from "next/link"
import type { HomeRunListPanel } from "@/lib/home/types"

export function HomeRunListPanelView({ panel }: { panel: HomeRunListPanel }) {
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
            <th className="px-4 py-3 font-semibold">Report</th>
            <th className="px-4 py-3 font-semibold">Type</th>
            <th className="px-4 py-3 font-semibold">Runs</th>
            <th className="px-4 py-3 font-semibold">Last Run</th>
          </tr>
        </thead>
        <tbody>
          {panel.rows.map((row) => (
            <tr key={row.id} className="align-top">
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
              <td className="px-4 py-3">{typeof row.runs === "number" ? row.runs : "-"}</td>
              <td className="px-4 py-3">{row.lastRun || "-"}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
