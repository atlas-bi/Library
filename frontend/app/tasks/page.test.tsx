import { render, screen } from "@testing-library/react"
import { beforeEach, describe, expect, it, vi } from "vitest"
import type { AuthUser } from "@/lib/auth"
import type { TasksResponseDto } from "@/lib/tasks/types"

const { getTokenMock, getCurrentUserMock, getTasksMock, redirectMock } = vi.hoisted(() => ({
  getTokenMock: vi.fn<() => Promise<string | null>>(),
  getCurrentUserMock: vi.fn<() => Promise<AuthUser | null>>(),
  getTasksMock: vi.fn(),
  redirectMock: vi.fn((path: string) => {
    throw new Error(`NEXT_REDIRECT:${path}`)
  }),
}))

vi.mock("@/lib/auth", () => ({
  getToken: getTokenMock,
  getCurrentUser: getCurrentUserMock,
  hasPermission: (user: AuthUser | null, permission: string) =>
    Boolean(user?.permissions.includes(permission)),
}))

vi.mock("@/lib/tasks/api", () => ({
  getTasks: getTasksMock,
}))

vi.mock("next/navigation", () => ({
  redirect: redirectMock,
}))

vi.mock("@/components/layout/library-shell", () => ({
  LibraryShell: ({ children }: { children: React.ReactNode }) => (
    <div data-testid="library-shell">{children}</div>
  ),
}))

vi.mock("@/components/tasks/tasks-page-tabs", () => ({
  TasksPageTabs: ({ data }: { data: TasksResponseDto }) => (
    <div data-testid="tasks-page-tabs">
      buckets={data.recommendRetire.length},{data.unused.length}
    </div>
  ),
}))

import TasksPage from "./page"

const stubUser: AuthUser = {
  username: "jdoe",
  fullname: "Jane Doe",
  userId: "42",
  roles: ["Report Writer"],
  permissions: ["View Groups"],
  adminEnabled: false,
}

const emptyTasks: TasksResponseDto = {
  canMakeReports: [],
  recommendRetire: [],
  unused: [],
  maintenanceRequired: [],
  audit: [],
  missingSchedule: [],
  notInAnalytics: [],
  topUndocumented: [],
  newUndocumented: [],
}

describe("TasksPage", () => {
  beforeEach(() => {
    vi.clearAllMocks()
    getTokenMock.mockResolvedValue("token")
    getCurrentUserMock.mockResolvedValue(stubUser)
  })

  it("redirects unauthenticated users to login", async () => {
    getTokenMock.mockResolvedValue(null)

    await expect(TasksPage()).rejects.toThrow("NEXT_REDIRECT:/auth/login")
    expect(redirectMock).toHaveBeenCalledWith("/auth/login")
  })

  it("renders the tasks dashboard when data loads", async () => {
    getTasksMock.mockResolvedValueOnce({ data: emptyTasks, error: null })

    render(await TasksPage())

    expect(screen.getByTestId("library-shell")).toBeInTheDocument()
    expect(screen.getByRole("heading", { level: 1, name: "Maintenance Tasks" })).toBeInTheDocument()
    expect(screen.getByTestId("tasks-page-tabs")).toBeInTheDocument()
  })

  it("shows an error when the tasks API fails", async () => {
    getTasksMock.mockResolvedValueOnce({ data: null, error: "service_unavailable" })

    render(await TasksPage())

    expect(screen.getByText(/temporarily unavailable/i)).toBeInTheDocument()
  })
})
