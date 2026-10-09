import { FileText, BadgeCheck } from "lucide-react"
import Link from "next/link"
import { redirect } from "next/navigation"
import { MarkdownContent } from "@/components/content/markdown-content"
import { TechnicalDefinitionContent } from "@/components/content/technical-definition-content"
import { LibraryShell } from "@/components/layout/library-shell"
import { ProfileAnalyticsPanel } from "@/components/profile/profile-analytics-panel"
import { TermRelatedReportCard } from "@/components/snippets/term-related-report-card"
import { TermActionRail } from "@/components/terms/term-action-rail"
import { TermMetadataTable } from "@/components/terms/term-metadata-table"
import { TermSectionNav } from "@/components/terms/term-section-nav"
import { TermsListCard } from "@/components/terms/terms-list-card"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { type AuthUser, getCurrentUser, getToken } from "@/lib/auth"
import { getUserFriendlyErrorMessage } from "@/lib/errors"
import { getTermById, getTermReports, getTermsList } from "@/lib/terms/api"

type TermsSearchParams = {
  id?: string
}

function getSingleValue(value: string | string[] | undefined): string | undefined {
  if (typeof value === "string") return value
  if (Array.isArray(value)) return value[0]
  return undefined
}

function resolveDisplayName(user: AuthUser | null): string {
  if (!user) return "Guest"
  if (user.fullname && user.fullname !== "Guest") return user.fullname
  return user.username || "Guest"
}

function getShellProps(user: AuthUser | null) {
  return {
    displayName: resolveDisplayName(user),
    isSignedIn: !!user,
    isAdministrator: !!user && user.roles.includes("Administrator"),
    adminEnabled: user?.adminEnabled ?? false,
  }
}

export default async function TermsPage({
  searchParams,
}: {
  searchParams: Promise<TermsSearchParams>
}) {
  const token = await getToken()
  if (!token) redirect("/auth/login")

  const resolvedSearchParams = await searchParams
  const user = await getCurrentUser()
  const shellProps = getShellProps(user)

  const idRaw = getSingleValue(resolvedSearchParams.id)
  if (idRaw) {
    const id = Number(idRaw)
    if (!Number.isFinite(id) || id <= 0) {
      return (
        <LibraryShell {...shellProps} searchPlaceholder="search for terms..">
          <h1 className="atlas-home-heading">Term not found</h1>
          <p className="text-sm text-[var(--atlas-home-muted)]">Missing or invalid term id.</p>
          <Button asChild className="mt-6" variant="outline">
            <Link href="/terms">Back to terms</Link>
          </Button>
        </LibraryShell>
      )
    }

    const [termResult, reportsResult] = await Promise.all([
      getTermById(id),
      getTermReports(id),
    ])

    const term = termResult.data
    const reports = reportsResult.data ?? []

    if (!term) {
      const message = getUserFriendlyErrorMessage(termResult.error ?? "unknown")
      return (
        <LibraryShell {...shellProps} searchPlaceholder="search for terms..">
          <Card>
            <CardHeader>
              <CardTitle className="text-xl">Unable to load term</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <p className="text-sm text-muted-foreground">{message}</p>
              <Button asChild variant="outline">
                <Link href="/terms">Back to terms</Link>
              </Button>
            </CardContent>
          </Card>
        </LibraryShell>
      )
    }

    const hasSummary = !!term.summary?.trim()
    const hasTechnicalDefinition = !!term.technicalDefinition?.trim()
    const hasDescription = hasSummary || hasTechnicalDefinition
    const hasReports = reports.length > 0
    const features = term.features ?? undefined

    return (
      <LibraryShell {...shellProps} searchPlaceholder="search for terms..">
        <div className="mb-4 text-sm text-[var(--atlas-home-muted)]">
          <Link href={`/terms?id=${term.id}`} className="text-[var(--atlas-home-link)] hover:text-[var(--atlas-home-link-hover)]">
            {term.name}
          </Link>
          <span className="mx-2">/</span>
          <Link href="/terms" className="text-[var(--atlas-home-link)] hover:text-[var(--atlas-home-link-hover)]">
            Terms
          </Link>
          <span className="mx-2">/</span>
          <Link href="/" className="text-[var(--atlas-home-link)] hover:text-[var(--atlas-home-link-hover)]">
            Home
          </Link>
        </div>
        <div className="grid gap-10 xl:grid-cols-[4.75rem_minmax(0,1fr)]">
          <div className="pt-2">
            <TermActionRail
              term={term}
              profilePanel={
                <ProfileAnalyticsPanel
                  id={term.id}
                  type="term"
                  userProfilesEnabled={term.features?.userProfilesEnabled}
                />
              }
            />
          </div>

          <div className="min-w-0 space-y-12">
            <div className="space-y-4">
              <h1 className="atlas-home-heading mb-0 flex flex-wrap items-start justify-between gap-3 text-[2.5rem] leading-[1.125] font-bold text-[#363636]">
                <span>{term.name}</span>
                {term.isApproved ? (
                  <Badge className="mt-2 rounded-[4px] border-[#48c78e] bg-[#48c78e] text-white hover:bg-[#48c78e]">
                    Approved
                  </Badge>
                ) : null}
              </h1>
              <TermSectionNav hasDescription={hasDescription} hasReports={hasReports} />
            </div>
            <div className="content space-y-8">
              {(hasSummary || hasTechnicalDefinition) ? (
                <>
                  <h2 id="details" className="atlas-home-heading text-3xl mb-4">Description</h2>
                  {hasSummary ? (
                    <div className="space-y-4">
                      <h3 className="atlas-home-heading text-2xl mb-2">Summary</h3>
                      <div>
                        <MarkdownContent content={term.summary ?? ""} />
                      </div>
                    </div>
                  ) : null}
                  {hasTechnicalDefinition ? (
                    <div className="space-y-4">
                      <h3 className="atlas-home-heading text-2xl mb-2">Technical Definition</h3>
                      <div>
                        <TechnicalDefinitionContent content={term.technicalDefinition ?? ""} />
                      </div>
                    </div>
                  ) : null}
                </>
              ) : null}

              {hasReports ? (
                <>
                  <h2 id="reports" className="atlas-home-heading text-3xl mb-4 mt-8">Linked Reports</h2>
                  <div className="grid gap-4 md:grid-cols-2">
                    {reports.map((report) => (
                      <TermRelatedReportCard
                        key={report.id}
                        report={report}
                        features={features}
                      />
                    ))}
                  </div>
                </>
              ) : null}
            </div>

            <div className="mt-8">
              <h2 id="meta" className="atlas-home-heading text-3xl mb-4">Details</h2>
              <div>
                <TermMetadataTable term={term} />
              </div>
            </div>
          </div>
        </div>
      </LibraryShell>
    )
  }

  const listResult = await getTermsList()
  const list = listResult.data

  if (!list) {
    const message = getUserFriendlyErrorMessage(listResult.error ?? "unknown")
    const statusHint =
      listResult.status != null ? `The API responded with HTTP ${listResult.status}.` : null
    const backendHint =
      listResult.error === "server_error"
        ? "The Terms API on the C# backend returned an error. Check backend logs for GET /api/terms."
        : listResult.error === "service_unavailable"
          ? "API_URL is not configured. Set API_URL in .env.local to your Library backend."
          : null
    return (
      <LibraryShell {...shellProps} searchPlaceholder="search for terms..">
        <Card>
          <CardHeader>
            <CardTitle className="text-xl">Unable to load terms</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <p className="text-sm text-muted-foreground">{message}</p>
            {statusHint ? <p className="text-sm text-muted-foreground">{statusHint}</p> : null}
            {backendHint ? <p className="text-sm text-muted-foreground">{backendHint}</p> : null}
            <Button asChild variant="outline">
              <Link href="/">Back to home</Link>
            </Button>
          </CardContent>
        </Card>
      </LibraryShell>
    )
  }

  return (
    <LibraryShell {...shellProps} searchPlaceholder="search for terms..">
      <div className="mb-4 text-sm text-[var(--atlas-home-muted)]">
        <Link href="/terms" className="text-[var(--atlas-home-link)] hover:underline">
          Terms
        </Link>
        <span className="mx-1.5">/</span>
        <Link href="/" className="text-[var(--atlas-home-link)] hover:underline">
          Home
        </Link>
      </div>

      <header className="space-y-4">
        <h1 className="atlas-home-heading mb-0">Terms</h1>
        {list.permissions?.canCreateTerm ? (
          <div>
            <Button asChild>
              <Link href="/terms/new">
                <span className="mr-2 text-base font-bold leading-none">+</span>
                <span>Create a Term</span>
              </Link>
            </Button>
          </div>
        ) : null}
      </header>

      {list.items.length === 0 ? (
        <p className="mt-4 text-sm text-[var(--atlas-home-muted)]">No terms found.</p>
      ) : (
        <div className="mt-4 grid gap-4 md:grid-cols-2">
          {list.items.map((item) => (
            <TermsListCard
              key={item.id}
              term={item}
              features={list.features ?? undefined}
              canOpenProfile={list.permissions?.canViewUserProfiles ?? true}
            />
          ))}
        </div>
      )}
    </LibraryShell>
  )
}
