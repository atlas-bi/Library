import Link from "next/link"

export function AnalyticsPageHeader() {
  return (
    <>
      <nav aria-label="Breadcrumb" className="mb-2 text-sm text-[#363636]">
        <ol className="flex flex-wrap items-center gap-1">
          <li>Analytics</li>
          <li aria-hidden className="text-[#b5b5b5]">
            /
          </li>
          <li>
            <Link href="/" className="text-[#3273dc] hover:underline">
              Home
            </Link>
          </li>
        </ol>
      </nav>
      <h1 className="atlas-home-heading mb-6">Analytics</h1>
    </>
  )
}
