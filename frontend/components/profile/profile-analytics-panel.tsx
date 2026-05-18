import Link from "next/link"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import {
  getProfileChart,
  getProfileFails,
  getProfileReports,
  getProfileRunList,
  getProfileStars,
  getProfileSubscriptions,
  getProfileUsers,
} from "@/lib/profile/api"
import type { ProfileBarItemDto, ProfileFilters } from "@/lib/profile/types"

export async function ProfileAnalyticsPanel({ id, type }: { id: number; type: string }) {
  const filters: ProfileFilters = { id, type }

  const [
    chartResult,
    usersResult,
    reportsResult,
    failsResult,
    runListResult,
    starsResult,
    subsResult,
  ] = await Promise.all([
    getProfileChart(filters),
    getProfileUsers(filters),
    getProfileReports(filters),
    getProfileFails(filters),
    getProfileRunList(filters),
    getProfileStars(filters),
    getProfileSubscriptions(filters),
  ])

  const chart = chartResult.data

  return (
    <Card>
      <CardHeader>
        <CardTitle className="text-base">Profile analytics</CardTitle>
      </CardHeader>
      <CardContent className="space-y-6">
        {chart ? (
          <div className="grid gap-4 sm:grid-cols-3">
            <Stat label="Runs" value={chart.runs} />
            <Stat label="Users" value={chart.users} />
            <Stat label="Avg runtime" value={chart.runTime} />
          </div>
        ) : (
          <p className="text-sm text-muted-foreground">Chart data unavailable.</p>
        )}

        {chart && chart.history.length > 0 ? (
          <div>
            <div className="mb-2 text-sm font-medium">History</div>
            <ul className="space-y-1 text-sm text-muted-foreground">
              {chart.history.map((point) => (
                <li key={point.date}>
                  {point.date}: {point.runs} runs, {point.users} users, {point.runTime} avg runtime
                </li>
              ))}
            </ul>
          </div>
        ) : null}

        <BarSection title="Top users" items={usersResult.data ?? []} />
        <BarSection title="Top reports" items={reportsResult.data ?? []} />
        <BarSection title="Failures" items={failsResult.data ?? []} />

        {runListResult.data && runListResult.data.length > 0 ? (
          <div>
            <div className="mb-2 text-sm font-medium">Run list</div>
            <ul className="space-y-2 text-sm">
              {runListResult.data.map((item) => (
                <li key={`${item.name}-${item.url ?? item.lastRun ?? "run"}`}>
                  {item.url ? (
                    <Link href={item.url} className="hover:underline">
                      {item.name}
                    </Link>
                  ) : (
                    item.name
                  )}
                  {typeof item.runs === "number" ? (
                    <span className="ml-2 text-muted-foreground">{item.runs} runs</span>
                  ) : null}
                </li>
              ))}
            </ul>
          </div>
        ) : null}

        {starsResult.data && starsResult.data.length > 0 ? (
          <div>
            <div className="mb-2 text-sm font-medium">Stars</div>
            <ul className="space-y-1 text-sm text-muted-foreground">
              {starsResult.data.map((user) => (
                <li key={user.id}>{user.fullName ?? user.email ?? `User ${user.id}`}</li>
              ))}
            </ul>
          </div>
        ) : null}

        {subsResult.data && subsResult.data.length > 0 ? (
          <div>
            <div className="mb-2 text-sm font-medium">Subscriptions</div>
            <ul className="space-y-1 text-sm text-muted-foreground">
              {subsResult.data.map((sub) => (
                <li key={sub.id}>
                  {sub.userName ?? `Subscription ${sub.id}`}
                  {sub.lastStatus ? ` ? ${sub.lastStatus}` : null}
                </li>
              ))}
            </ul>
          </div>
        ) : null}
      </CardContent>
    </Card>
  )
}

function Stat({ label, value }: { label: string; value: number }) {
  return (
    <div className="rounded-md border p-3 text-center">
      <div className="text-2xl font-semibold">{value}</div>
      <div className="text-xs text-muted-foreground">{label}</div>
    </div>
  )
}

function BarSection({ title, items }: { title: string; items: ProfileBarItemDto[] }) {
  if (items.length === 0) return null

  return (
    <div>
      <div className="mb-2 text-sm font-medium">{title}</div>
      <ul className="space-y-2 text-sm">
        {items.map((item) => (
          <li key={item.key} className="flex items-center justify-between gap-2">
            <span>
              {item.href ? (
                <Link href={item.href} className="hover:underline">
                  {item.title ?? item.key}
                </Link>
              ) : (
                (item.title ?? item.key)
              )}
            </span>
            <span className="text-muted-foreground">
              {item.count} ({item.percent}%)
            </span>
          </li>
        ))}
      </ul>
    </div>
  )
}
