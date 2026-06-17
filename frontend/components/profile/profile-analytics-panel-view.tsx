import Link from "next/link"
import type { ProfileAnalyticsData } from "@/app/profile/actions"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import type { ProfileBarItemDto } from "@/lib/profile/types"

export function ProfileAnalyticsPanelView({ data }: { data: ProfileAnalyticsData }) {
  const chart = data.chart

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

        <BarSection title="Top users" items={data.users} />
        <BarSection title="Top reports" items={data.reports} />
        <BarSection title="Failures" items={data.fails} />

        {data.runList.length > 0 ? (
          <div>
            <div className="mb-2 text-sm font-medium">Run list</div>
            <ul className="space-y-2 text-sm">
              {data.runList.map((item) => (
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

        {data.stars.length > 0 ? (
          <div>
            <div className="mb-2 text-sm font-medium">Stars</div>
            <ul className="space-y-1 text-sm text-muted-foreground">
              {data.stars.map((user) => (
                <li key={user.id}>{user.fullName ?? user.email ?? `User ${user.id}`}</li>
              ))}
            </ul>
          </div>
        ) : null}

        {data.subscriptions.length > 0 ? (
          <div>
            <div className="mb-2 text-sm font-medium">Subscriptions</div>
            <ul className="space-y-1 text-sm text-muted-foreground">
              {data.subscriptions.map((sub) => (
                <li key={sub.id}>
                  {sub.userName ?? `Subscription ${sub.id}`}
                  {sub.lastStatus ? ` · ${sub.lastStatus}` : null}
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
