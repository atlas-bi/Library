import Link from "next/link"
import type { AnalyticsLiveUsersResponseDto } from "@/lib/analytics/types"

export function AnalyticsActiveUsersTable({
  liveUsers,
}: {
  liveUsers: AnalyticsLiveUsersResponseDto | null
}) {
  const activeCount = liveUsers?.activeUsers ?? 0
  const rows = liveUsers?.items ?? []

  return (
    <section className="mb-6" id="active-users">
      <h3 className="mb-3 text-xl font-bold text-[#363636]">Active Now! - {activeCount} users</h3>
      {rows.length === 0 ? (
        <p className="text-sm text-[#7a7a7a]">No active sessions in the last minute.</p>
      ) : (
        <div className="overflow-x-auto">
          <table className="w-full min-w-[960px] border-collapse text-sm">
            <thead>
              <tr className="border-b text-left">
                <th className="p-2 font-medium">User</th>
                <th className="p-2 font-medium">SessionTime</th>
                <th className="p-2 font-medium">PageTime</th>
                <th className="p-2 font-medium">Page Title</th>
                <th className="p-2 font-medium">Link</th>
                <th className="p-2 font-medium">Page Load Time</th>
                <th className="p-2 font-medium">Refreshed</th>
                <th className="p-2 font-medium">Pages Visited</th>
              </tr>
            </thead>
            <tbody>
              {rows.map((row) => (
                <tr key={`${row.userId}-${row.sessionId}`} className="border-b last:border-0">
                  <td className="p-2 whitespace-nowrap">
                    <Link
                      href={`/users?id=${row.userId}`}
                      className="text-[var(--atlas-home-link,theme(colors.blue.600))] hover:underline"
                    >
                      {row.fullname}
                    </Link>
                  </td>
                  <td className="p-2 whitespace-nowrap">{row.sessionTime}</td>
                  <td className="p-2 whitespace-nowrap">{row.pageTime}</td>
                  <td className="max-w-[12rem] truncate p-2 text-[#7a7a7a]">—</td>
                  <td className="max-w-xs truncate p-2">
                    <Link href={row.href} className="text-[#3273dc] hover:underline">
                      {row.href}
                    </Link>
                  </td>
                  <td className="p-2 whitespace-nowrap">{row.accessDateTime}</td>
                  <td className="p-2 whitespace-nowrap">{row.updateTime}</td>
                  <td className="p-2 text-center">{row.pages}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  )
}
