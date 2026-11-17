import { BadgeCheck } from "lucide-react"
import Link from "next/link"
import {
  formatRecentTermName,
  normalizeRecentTerms,
  termDetailHref,
} from "@/lib/terms/recent-terms-client-api"
import type { RecentTermsDto } from "@/lib/terms/types"

type HomeRecentTermsViewProps = {
  status: "loading" | "error" | "ready"
  data?: RecentTermsDto | null
  error?: "auth_required" | "unknown"
}

export function HomeRecentTermsView({ status, data, error }: HomeRecentTermsViewProps) {
  if (status === "loading") {
    return (
      <aside
        aria-label="Recent terms"
        className="sticky top-20 self-start rounded-md border border-[var(--atlas-home-border-soft)] bg-white p-4 text-sm text-[var(--atlas-home-muted)]"
      >
        Loading terms…
      </aside>
    )
  }

  if (status === "error") {
    return (
      <aside
        aria-label="Recent terms"
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
            to view recent terms.
          </>
        ) : (
          <>Unable to load recent terms.</>
        )}
      </aside>
    )
  }

  const { approved, pending } = normalizeRecentTerms(data)
  const hasAny = approved.length + pending.length > 0

  if (!hasAny) {
    return (
      <aside
        aria-label="Recent terms"
        className="sticky top-20 self-start rounded-md border border-[var(--atlas-home-border-soft)] bg-white p-4 text-sm text-[var(--atlas-home-muted)]"
      >
        No recent terms to show.
      </aside>
    )
  }

  return (
    <aside
      aria-label="Recent terms"
      className="sticky top-20 self-start space-y-4 rounded-md border border-[var(--atlas-home-border-soft)] bg-white p-4"
    >
      <h4 className="text-sm font-semibold text-[var(--atlas-home-text-strong)]">Terms</h4>

      {approved.length > 0 ? (
        <div className="space-y-2">
          <p className="text-xs font-medium uppercase tracking-wide text-[var(--atlas-home-muted)]">
            Approved
          </p>
          <ul className="space-y-2 text-sm">
            {approved.map((term) => (
              <li key={`approved-${term.id}`}>
                <Link
                  href={termDetailHref(term.id)}
                  className="inline-flex items-start gap-1.5 text-[var(--atlas-home-link)] hover:underline"
                >
                  <BadgeCheck
                    className="mt-0.5 h-4 w-4 shrink-0 text-[var(--atlas-home-muted)]"
                    strokeWidth={1.8}
                    aria-hidden
                  />
                  <span>{formatRecentTermName(term.name)}</span>
                </Link>
              </li>
            ))}
          </ul>
        </div>
      ) : null}

      {pending.length > 0 ? (
        <div className="space-y-2">
          <p className="text-xs font-medium uppercase tracking-wide text-[var(--atlas-home-muted)]">
            Pending
          </p>
          <ul className="space-y-2 text-sm">
            {pending.map((term) => (
              <li key={`pending-${term.id}`}>
                <Link
                  href={termDetailHref(term.id)}
                  className="text-[var(--atlas-home-link)] hover:underline"
                >
                  {formatRecentTermName(term.name)}
                </Link>
              </li>
            ))}
          </ul>
        </div>
      ) : null}
    </aside>
  )
}
