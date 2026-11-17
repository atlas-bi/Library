export function AnalyticsLogPagination({
  currentPage,
  totalPages,
  onPageChange,
}: {
  currentPage: number
  totalPages: number
  onPageChange: (page: number) => void
}) {
  if (totalPages <= 1) return null

  const pages: number[] = []
  for (let i = 1; i <= totalPages; i += 1) {
    if (i === 1 || i === totalPages || Math.abs(i - currentPage) <= 1) {
      pages.push(i)
    }
  }

  const withEllipsis: (number | "ellipsis")[] = []
  pages.forEach((page, index) => {
    if (index > 0 && page - pages[index - 1] > 1) {
      withEllipsis.push("ellipsis")
    }
    withEllipsis.push(page)
  })

  return (
    <nav aria-label="Pagination" className="flex flex-wrap gap-1">
      {withEllipsis.map((entry, idx) =>
        entry === "ellipsis" ? (
          <span key={`e-${idx}`} className="px-2 py-1 text-muted-foreground">
            …
          </span>
        ) : (
          <button
            key={entry}
            type="button"
            className={`min-w-8 rounded border px-2 py-1 text-sm ${
              entry === currentPage
                ? "border-primary bg-primary/10 font-semibold"
                : "border-border hover:bg-muted/50"
            }`}
            onClick={() => onPageChange(entry - 1)}
            aria-current={entry === currentPage ? "page" : undefined}
          >
            {entry}
          </button>
        ),
      )}
    </nav>
  )
}
