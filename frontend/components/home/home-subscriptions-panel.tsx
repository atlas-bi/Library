import type { HomeSubscriptionsPanel } from "@/lib/home/types"

export function HomeSubscriptionsPanelView({ panel }: { panel: HomeSubscriptionsPanel }) {
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
            <th className="px-4 py-3 font-semibold">Subscription Description</th>
            <th className="px-4 py-3 font-semibold">Last Run</th>
            <th className="px-4 py-3 font-semibold">Message</th>
            <th className="px-4 py-3 font-semibold">Subscribed As</th>
          </tr>
        </thead>
        <tbody>
          {panel.rows.map((row) => (
            <tr key={row.id} className="align-top">
              <td className="px-4 py-3">
                <div className="font-semibold text-[var(--atlas-home-title)]">{row.name}</div>
              </td>
              <td className="px-4 py-3">{row.description || "-"}</td>
              <td className="px-4 py-3">{row.lastRun || "Never"}</td>
              <td className="px-4 py-3">{row.lastStatus || "-"}</td>
              <td className="px-4 py-3">{row.sentTo || "-"}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
