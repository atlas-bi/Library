import Link from "next/link"
import { redirect } from "next/navigation"
import { ReportEditForm } from "@/components/reports/report-edit-form"
import { ReportImageUpload } from "@/components/reports/report-image-upload"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { getCurrentUser, getToken, hasPermission } from "@/lib/auth"
import { getReportDetailById } from "@/lib/reports/api"

type EditSearchParams = { id?: string }

function getSingleValue(value: string | string[] | undefined): string | undefined {
  if (typeof value === "string") return value
  if (Array.isArray(value)) return value[0]
  return undefined
}

export default async function EditReportPage({ searchParams }: { searchParams: EditSearchParams }) {
  const token = await getToken()
  if (!token) redirect("/auth/login")

  const user = await getCurrentUser()
  if (!user || !hasPermission(user, "Edit Report Documentation")) {
    return (
      <div className="mx-auto max-w-3xl px-4 py-10">
        <Card>
          <CardHeader>
            <CardTitle className="text-xl">Permission required</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <p className="text-sm text-muted-foreground">
              You need the Edit Report Documentation permission to update reports.
            </p>
            <Button asChild variant="outline">
              <Link href="/">Back to home</Link>
            </Button>
          </CardContent>
        </Card>
      </div>
    )
  }

  const idRaw = getSingleValue(searchParams.id)
  const id = idRaw ? Number(idRaw) : NaN
  if (!Number.isFinite(id) || id <= 0) {
    return (
      <div className="mx-auto max-w-3xl px-4 py-10">
        <h1 className="text-2xl font-bold">Missing report</h1>
        <p className="mt-2 text-sm text-muted-foreground">Provide a valid report id in the URL.</p>
        <Button asChild className="mt-6" variant="outline">
          <Link href="/">Back to home</Link>
        </Button>
      </div>
    )
  }

  const result = await getReportDetailById(id)
  const report = result.data
  if (!report) {
    return (
      <div className="mx-auto max-w-3xl px-4 py-10">
        <Card>
          <CardHeader>
            <CardTitle className="text-xl">Unable to load report</CardTitle>
          </CardHeader>
          <CardContent>
            <Button asChild variant="outline">
              <Link href="/">Back to home</Link>
            </Button>
          </CardContent>
        </Card>
      </div>
    )
  }

  if (!report.canEditDocumentation) {
    return (
      <div className="mx-auto max-w-3xl px-4 py-10">
        <Card>
          <CardHeader>
            <CardTitle className="text-xl">Editing not available</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <p className="text-sm text-muted-foreground">
              You do not have access to edit this report.
            </p>
            <Button asChild variant="outline">
              <Link href={`/reports?id=${report.id}`}>Back to report</Link>
            </Button>
          </CardContent>
        </Card>
      </div>
    )
  }

  const title = report.displayTitle || report.displayName || report.name

  return (
    <div className="mx-auto max-w-5xl space-y-6 px-4 py-8">
      <div className="text-sm text-muted-foreground">
        <Link href={`/reports?id=${report.id}`} className="hover:underline">
          {title}
        </Link>
        <span className="px-1">/</span>
        <span>Edit documentation</span>
      </div>
      <ReportEditForm
        reportId={report.id}
        initial={report}
        cancelHref={`/reports?id=${report.id}`}
      />
      <Card>
        <CardHeader>
          <CardTitle className="text-base">Images</CardTitle>
        </CardHeader>
        <CardContent>
          <ReportImageUpload reportId={report.id} />
        </CardContent>
      </Card>
    </div>
  )
}
