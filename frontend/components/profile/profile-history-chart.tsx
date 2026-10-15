import type { ProfileChartHistoryPoint } from "@/lib/profile/types"

export function ProfileHistoryChart({ history }: { history: ProfileChartHistoryPoint[] }) {
  if (history.length === 0) {
    return <p className="text-sm text-muted-foreground">No history data available.</p>
  }

  const maxRuns = Math.max(...history.map((point) => point.runs), 1)
  const maxUsers = Math.max(...history.map((point) => point.users), 1)

  return (
    <div className="space-y-3 rounded-md border bg-card/40 p-4">
      <div className="flex flex-wrap items-center gap-4 text-xs text-muted-foreground">
        <span className="inline-flex items-center gap-2">
          <span className="size-2 rounded-sm bg-[rgba(38,128,235,0.35)]" />
          Runs
        </span>
        <span className="inline-flex items-center gap-2">
          <span className="size-2 rounded-sm bg-[rgba(38,128,235,0.55)]" />
          Users
        </span>
        <span className="inline-flex items-center gap-2">
          <span className="h-0.5 w-4 bg-[rgba(38,128,235,0.9)]" />
          Run time
        </span>
      </div>

      <div className="overflow-x-auto">
        <div
          className="grid min-w-[640px] items-end gap-2"
          style={{ gridTemplateColumns: `repeat(${history.length}, minmax(2.5rem, 1fr))` }}
        >
          {history.map((point) => {
            const runsHeight = Math.max(8, Math.round((point.runs / maxRuns) * 180))
            const usersHeight = Math.max(8, Math.round((point.users / maxUsers) * 180))

            return (
              <div key={point.date} className="flex min-w-10 flex-col items-center gap-2">
                <div className="flex h-48 w-full items-end justify-center gap-0.5">
                  <div
                    className="w-2 rounded-t-sm bg-[rgba(38,128,235,0.35)]"
                    style={{ height: `${runsHeight}px` }}
                    title={`${point.runs} runs`}
                  />
                  <div
                    className="w-2 rounded-t-sm bg-[rgba(38,128,235,0.55)]"
                    style={{ height: `${usersHeight}px` }}
                    title={`${point.users} users, ${point.runTime}s avg runtime`}
                  />
                </div>
                <div className="max-w-12 truncate text-center text-[10px] text-muted-foreground">
                  {point.date}
                </div>
              </div>
            )
          })}
        </div>
      </div>
    </div>
  )
}

function ProfileStatCard({
  label,
  value,
  suffix,
}: {
  label: string
  value: number | null
  suffix?: string
}) {
  return (
    <div className="space-y-1">
      <div className="text-3xl font-semibold tracking-tight">
        {value === null ? "-" : value.toLocaleString()}
        {value !== null && suffix ? <span className="text-xl">{suffix}</span> : null}
      </div>
      <div className="text-sm text-muted-foreground">{label}</div>
    </div>
  )
}

export function ProfileSummaryStats({
  runs,
  users,
  runTime,
}: {
  runs: number | null
  users: number | null
  runTime: number | null
}) {
  return (
    <div className="flex flex-wrap gap-8">
      <ProfileStatCard label="Runs" value={runs} />
      <ProfileStatCard label="Users" value={users} />
      <ProfileStatCard label="Run Time" value={runTime} suffix="s" />
    </div>
  )
}
