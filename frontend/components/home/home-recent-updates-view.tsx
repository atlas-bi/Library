import Link from "next/link"
import {
  collectionDetailHref,
  initiativeDetailHref,
  reportDetailHref,
} from "@/lib/home/recent-updates-client-api"
import type { RecentUpdatesBundle, RecentUpdatesSection } from "@/lib/home/recent-updates-types"

type HomeRecentUpdatesViewProps = {
  status: "loading" | "error" | "ready"
  data?: RecentUpdatesBundle | null
  error?: "auth_required" | "unknown"
}

function RecentUpdatesList({
  title,
  section,
  hrefForId,
  emptyLabel,
}: {
  title: string
  section: RecentUpdatesSection
  hrefForId: (id: number) => string
  emptyLabel: string
}) {
  return (
    <div className="space-y-2">
      <p className="text-xs font-medium uppercase tracking-wide text-[var(--atlas-home-muted)]">
        {title}
      </p>
      {section.failed ? (
        <p className="text-sm text-[var(--atlas-home-text)]">Unable to load {title.toLowerCase()}.</p>
      ) : section.items.length === 0 ? (
        <p className="text-sm text-[var(--atlas-home-muted)]">{emptyLabel}</p>
      ) : (
        <ul className="space-y-2 text-sm">
          {section.items.map((item) => (
            <li key={item.id}>
              <Link href={hrefForId(item.id)} className="text-[var(--atlas-home-link)] hover:underline">
                {item.name}
              </Link>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}

export function HomeRecentUpdatesView({ status, data, error }: HomeRecentUpdatesViewProps) {
  if (status === "loading") {
    return (
      <aside
        aria-label="Recent updates"
        className="sticky top-20 self-start rounded-md border border-[var(--atlas-home-border-soft)] bg-white p-4 text-sm text-[var(--atlas-home-muted)]"
      >
        Loading recent updates…
      </aside>
    )
  }

  if (status === "error") {
    return (
      <aside
        aria-label="Recent updates"
        className="sticky top-20 self-start rounded-md border border-[var(--atlas-home-border-soft)] bg-white p-4 text-sm text-[var(--atlas-home-text)]"
      >
        {error === "auth_required" ? (
          <>
            <a
              href="/auth/login"
              className="font-medium text-[var(--atlas-home-link)] hover:underline"
            >
              Sign in
            </a>{" "}
            to view recent updates.
          </>
        ) : (
          <>Unable to load recent updates.</>
        )}
      </aside>
    )
  }

  if (!data) {
    return (
      <aside
        aria-label="Recent updates"
        className="sticky top-20 self-start rounded-md border border-[var(--atlas-home-border-soft)] bg-white p-4 text-sm text-[var(--atlas-home-muted)]"
      >
        No recent updates to show.
      </aside>
    )
  }

  const allEmpty =
    !data.reports.failed &&
    !data.initiatives.failed &&
    !data.collections.failed &&
    data.reports.items.length === 0 &&
    data.initiatives.items.length === 0 &&
    data.collections.items.length === 0

  if (allEmpty) {
    return (
      <aside
        aria-label="Recent updates"
        className="sticky top-20 self-start rounded-md border border-[var(--atlas-home-border-soft)] bg-white p-4 text-sm text-[var(--atlas-home-muted)]"
      >
        No recent updates to show.
      </aside>
    )
  }

  return (
    <aside
      aria-label="Recent updates"
      className="sticky top-20 self-start space-y-4 rounded-md border border-[var(--atlas-home-border-soft)] bg-white p-4"
    >
      <h4 className="text-sm font-semibold text-[var(--atlas-home-text-strong)]">Recent Updates</h4>

      <RecentUpdatesList
        title="Reports"
        section={data.reports}
        hrefForId={reportDetailHref}
        emptyLabel="No recent reports."
      />
      <RecentUpdatesList
        title="Initiatives"
        section={data.initiatives}
        hrefForId={initiativeDetailHref}
        emptyLabel="No recent initiatives."
      />
      <RecentUpdatesList
        title="Collections"
        section={data.collections}
        hrefForId={collectionDetailHref}
        emptyLabel="No recent collections."
      />
    </aside>
  )
}
