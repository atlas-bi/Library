import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest"
import { HomeTabsClient } from "@/components/home/home-tabs-client"

const fetchMock = vi.fn<(input: RequestInfo | URL) => Promise<Response>>()

describe("HomeTabsClient", () => {
  beforeEach(() => {
    vi.stubGlobal("fetch", fetchMock)
    process.env.NEXT_PUBLIC_API_URL = "http://localhost:5000"
    // biome-ignore lint/suspicious/noDocumentCookie: test setup requires a readable cookie token
    document.cookie = "atlas_token=test-token"
  })

  afterEach(() => {
    vi.unstubAllGlobals()
    delete process.env.NEXT_PUBLIC_API_URL
    fetchMock.mockReset()
  })

  it("loads the default stars tab and caches loaded tabs", async () => {
    fetchMock.mockImplementation((input) => {
      const url = String(input)
      if (url.endsWith("/api/users/7/stars")) {
        return Promise.resolve(
          new Response(
            JSON.stringify({
              summary: { totalCount: 2, unsortedCount: 0 },
              filters: {
                hasReports: true,
                hasCollections: false,
                hasInitiatives: false,
                hasTerms: false,
                hasUsers: false,
                hasGroups: false,
                hasSearches: false,
              },
              folders: [],
              items: [
                {
                  starId: 1,
                  itemId: 1,
                  url: "/reports?id=1",
                  name: "Executive Dashboard",
                  typeLabel: "Report",
                  description: "Used for leadership review.",
                  starCount: 3,
                },
              ],
              suggestedReports: [],
            }),
            {
              status: 200,
              headers: { "Content-Type": "application/json" },
            },
          ),
        )
      }

      if (url.endsWith("/api/users/7/groups")) {
        return Promise.resolve(
          new Response(JSON.stringify([{ id: 12, name: "Finance", type: "AD", source: "LDAP" }]), {
            status: 200,
            headers: { "Content-Type": "application/json" },
          }),
        )
      }

      return Promise.resolve(
        new Response(JSON.stringify({ error: "unknown" }), {
          status: 500,
          headers: { "Content-Type": "application/json" },
        }),
      )
    })

    render(<HomeTabsClient requestContext={{ userId: 7, defaultReportTypeIds: [1, 2] }} />)

    expect(screen.getByRole("tab", { name: "Stars" })).toHaveAttribute("aria-selected", "true")
    expect(screen.getByText("Loading Stars...")).toBeInTheDocument()

    await screen.findByText("Executive Dashboard")
    expect(fetchMock).toHaveBeenCalledTimes(1)

    await userEvent.click(screen.getByRole("tab", { name: "Groups" }))
    await screen.findByText("Finance")
    expect(fetchMock).toHaveBeenCalledTimes(2)

    await userEvent.click(screen.getByRole("tab", { name: "Stars" }))
    await waitFor(() => {
      expect(screen.getByText("Executive Dashboard")).toBeInTheDocument()
    })
    expect(fetchMock).toHaveBeenCalledTimes(2)
  })
})
