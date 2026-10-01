import { render, screen } from "@testing-library/react"
import { describe, expect, it } from "vitest"
import { ReportMaintenanceSection } from "@/components/reports/report-maintenance-section"
import type { ReportDetail } from "@/lib/reports/types"

describe("ReportMaintenanceSection", () => {
  it("renders maintenance log entries with maintainer links when allowed", () => {
    const report: ReportDetail = {
      id: 16,
      name: "census",
      document: {
        maintenanceLogs: [
          {
            id: 1,
            comment: "Reviewed query logic",
            maintenanceDate: "2026-01-10",
            status: { id: 1, name: "Complete" },
            maintainer: {
              id: 5,
              username: "jdoe",
              fullName: "Jane Doe",
              email: "jdoe@example.com",
            },
          },
        ],
      },
    }

    render(<ReportMaintenanceSection report={report} canViewUserProfiles />)

    expect(screen.getByRole("heading", { name: "Maintenance" })).toBeInTheDocument()
    expect(screen.getByRole("link", { name: "Jane Doe" })).toHaveAttribute("href", "/users?id=5")
    expect(screen.getByText(/Complete/)).toBeInTheDocument()
    expect(screen.getByText("Reviewed query logic")).toBeInTheDocument()
  })
})
