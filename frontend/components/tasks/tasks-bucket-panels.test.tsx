import { render, screen } from "@testing-library/react"
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest"
import {
  AuditPanel,
  isOlderThanSixMonths,
  MissingSchedulePanel,
  NewUndocumentedPanel,
} from "./tasks-bucket-panels"
import type { TaskMaintenanceReportDto } from "@/lib/tasks/types"

const maintenanceRow = (
  overrides: Partial<TaskMaintenanceReportDto> = {},
): TaskMaintenanceReportDto => ({
  reportId: 10,
  name: "Audit Report",
  user: "Alex Maintainer",
  date: "01/15/2025",
  ...overrides,
})

describe("AuditPanel", () => {
  beforeEach(() => {
    vi.useFakeTimers()
    vi.setSystemTime(new Date("2026-09-30T12:00:00Z"))
  })

  afterEach(() => {
    vi.useRealTimers()
  })

  it("uses the six-month review hint and last-maintained-on sentence", () => {
    render(
      <AuditPanel
        items={[maintenanceRow({ date: "06/01/2026", name: "Recent Audit Report" })]}
      />,
    )

    expect(
      screen.getByText("Reports with red outline have not been reviewed in the last 6 months."),
    ).toBeInTheDocument()
    expect(
      screen.getByText(/Last updated\/maintained by Alex Maintainer on 06\/01\/2026/),
    ).toBeInTheDocument()
    expect(screen.queryByText(/Due on/i)).not.toBeInTheDocument()
  })

  it("shows Past Due when the maintenance date is older than six months", () => {
    render(<AuditPanel items={[maintenanceRow()]} />)

    expect(screen.getByText("Past Due")).toBeInTheDocument()
    expect(isOlderThanSixMonths(new Date(2025, 0, 15))).toBe(true)
  })

  it("does not show Past Due for dates within six months", () => {
    render(<AuditPanel items={[maintenanceRow({ date: "06/01/2026" })]} />)

    expect(screen.queryByText("Past Due")).not.toBeInTheDocument()
  })
})

describe("MissingSchedulePanel", () => {
  it("uses the six-month hint and last-maintained-on sentence without past-due styling", () => {
    render(
      <MissingSchedulePanel
        items={[maintenanceRow({ reportId: 11, name: "No Schedule Report", date: "01/01/2020" })]}
      />,
    )

    expect(
      screen.getByText("Reports with red outline have not been reviewed in the last 6 months."),
    ).toBeInTheDocument()
    expect(
      screen.getByText(/Last updated\/maintained by Alex Maintainer on 01\/01\/2020/),
    ).toBeInTheDocument()
    expect(screen.queryByText("Past Due")).not.toBeInTheDocument()
    expect(screen.queryByText(/Due on/i)).not.toBeInTheDocument()
  })
})

describe("NewUndocumentedPanel", () => {
  it("matches the Razor new-undocumented heading copy", () => {
    render(<NewUndocumentedPanel items={[]} />)

    expect(
      screen.getByText(
        "New (edited) Undocumented Reports (Workbench, SSRS, Dashboard, and Crystal)",
      ),
    ).toBeInTheDocument()
  })
})
