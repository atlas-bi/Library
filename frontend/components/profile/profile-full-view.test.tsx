import { render, screen, waitFor, act } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { describe, expect, it, vi, beforeEach } from "vitest"
import { ProfileFullView } from "./profile-full-view"
import { loadProfileAnalyticsAction } from "@/app/profile/actions"
import type { ProfileAnalyticsData } from "@/app/profile/actions"

vi.mock("@/app/profile/actions", () => ({
  loadProfileAnalyticsAction: vi.fn(),
  loadProfileFiltersAction: vi.fn().mockResolvedValue(null),
}))

const mockData: ProfileAnalyticsData = {
  chart: {
    runs: 42,
    users: 5,
    runTime: 1.2,
    history: [{ date: "Jan 24", runs: 42, users: 5, runTime: 1.2 }],
  },
  users: [{ key: "Alice", count: 10, percent: 0.5 }],
  reports: [{ key: "Report A", count: 8, percent: 0.4 }],
  fails: [],
  runList: [{ name: "Report X", type: "SSRS", url: "/reports?id=1", runs: 3, lastRun: "1/1/2024" }],
  stars: [{ id: 1, fullName: "Bob Star", email: "bob@example.com" }],
  subscriptions: [
    {
      id: 1,
      userId: 2,
      userName: "Carol Sub",
      description: "Daily digest",
      lastRunTime: "2024-01-01",
      lastStatus: "Success",
      subscriptionTo: "Email",
    },
  ],
}

describe("ProfileFullView", () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it("renders loading state when no initialData and fetch is pending", async () => {
    vi.mocked(loadProfileAnalyticsAction).mockImplementation(
      () => new Promise(() => {}), // never resolves
    )

    render(<ProfileFullView id={1} type="user" />)
    // Loading state appears after useEffect fires startTransition, so wait for it
    expect(await screen.findByText(/loading profile/i)).toBeDefined()
  })

  it("renders error message when action returns service_unavailable", async () => {
    vi.mocked(loadProfileAnalyticsAction).mockResolvedValue({
      data: null,
      error: "service_unavailable",
    })

    render(<ProfileFullView id={1} type="user" />)

    await waitFor(() => {
      expect(screen.getByText(/unable to load profile analytics/i)).toBeDefined()
    })
  })

  it("renders error message when action returns not_found", async () => {
    vi.mocked(loadProfileAnalyticsAction).mockResolvedValue({
      data: null,
      error: "not_found",
    })

    render(<ProfileFullView id={999} type="report" />)

    await waitFor(() => {
      expect(screen.getByText(/unable to load profile analytics/i)).toBeDefined()
    })
  })

  it("renders error message when action returns forbidden", async () => {
    vi.mocked(loadProfileAnalyticsAction).mockResolvedValue({
      data: null,
      error: "forbidden",
    })

    render(<ProfileFullView id={1} type="report" />)

    await waitFor(() => {
      expect(screen.getByText(/unable to load profile analytics/i)).toBeDefined()
    })
  })

  it("renders analytics unavailable when no data and no error", async () => {
    vi.mocked(loadProfileAnalyticsAction).mockResolvedValue({
      data: null,
      error: null,
    })

    render(<ProfileFullView id={1} type="user" />)

    await waitFor(() => {
      expect(screen.getByText(/profile analytics unavailable/i)).toBeDefined()
    })
  })

  it("renders summary stats when initialData is provided", () => {
    render(<ProfileFullView id={1} type="user" initialData={mockData} />)

    expect(screen.getByText("42")).toBeDefined() // runs count
    expect(screen.getByText("5")).toBeDefined()  // users count
  })

  it("renders run list table when Report Runs tab is active", async () => {
    const user = userEvent.setup()
    render(<ProfileFullView id={1} type="report" initialData={mockData} />)

    const reportRunsTab = screen.getByRole("button", { name: /report runs/i })
    await user.click(reportRunsTab)

    expect(screen.getByRole("link", { name: "Report X" })).toBeDefined()
  })

  it("renders stars table when Stars tab is active", async () => {
    const user = userEvent.setup()
    render(<ProfileFullView id={1} type="report" initialData={mockData} />)

    const starsTab = screen.getByRole("button", { name: /stars/i })
    await user.click(starsTab)

    expect(screen.getByText("Bob Star")).toBeDefined()
  })

  it("renders subscriptions table when Subscriptions tab is active", async () => {
    const user = userEvent.setup()
    render(<ProfileFullView id={1} type="report" initialData={mockData} />)

    const subsTab = screen.getByRole("button", { name: /subscriptions/i })
    await user.click(subsTab)

    expect(screen.getByText("Carol Sub")).toBeDefined()
    expect(screen.getByText("Daily digest")).toBeDefined()
  })

  it("does not show Stars/Subscriptions tabs for user type", () => {
    render(<ProfileFullView id={1} type="user" initialData={mockData} />)

    // For type=user the nav only renders if tabs.length > 1, and stars/subs are excluded
    expect(screen.queryByRole("button", { name: /stars/i })).toBeNull()
    expect(screen.queryByRole("button", { name: /subscriptions/i })).toBeNull()
  })

  it("refetches data when date range changes", async () => {
    const user = userEvent.setup()
    vi.mocked(loadProfileAnalyticsAction).mockResolvedValue({
      data: { ...mockData, chart: { ...mockData.chart!, runs: 99, users: 1, runTime: 0.5 } },
      error: null,
    })

    render(<ProfileFullView id={1} type="user" initialData={mockData} />)

    // The date-range select now appears in both the sidebar and the main content area.
    // Use getAllByRole and interact with the first instance (sidebar).
    const [dateSelect] = screen.getAllByRole("combobox", { name: /profile date range/i })
    await user.selectOptions(dateSelect, "last-30-days")
    await waitFor(() => {
      expect(loadProfileAnalyticsAction).toHaveBeenCalled()
    })
  })

  it("renders empty run list message when runList is empty", async () => {
    const user = userEvent.setup()
    render(
      <ProfileFullView
        id={1}
        type="report"
        initialData={{ ...mockData, runList: [] }}
      />,
    )

    const reportRunsTab = screen.getByRole("button", { name: /report runs/i })
    await user.click(reportRunsTab)

    expect(screen.getByText(/no run data to show/i)).toBeDefined()
  })

  it("renders empty stars message when stars is empty", async () => {
    const user = userEvent.setup()
    render(
      <ProfileFullView
        id={1}
        type="report"
        initialData={{ ...mockData, stars: [] }}
      />,
    )

    const starsTab = screen.getByRole("button", { name: /stars/i })
    await user.click(starsTab)

    expect(screen.getByText(/there are no stars/i)).toBeDefined()
  })

  it("hides user profile links in stars when userProfilesEnabled is false", async () => {
    const user = userEvent.setup()
    render(
      <ProfileFullView
        id={1}
        type="report"
        initialData={mockData}
        userProfilesEnabled={false}
      />,
    )

    const starsTab = screen.getByRole("button", { name: /stars/i })
    await user.click(starsTab)

    // Should show the name but NOT as a link
    expect(screen.getByText("Bob Star")).toBeDefined()
    expect(screen.queryByRole("link", { name: "Bob Star" })).toBeNull()
  })
})
