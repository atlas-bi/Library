import { fireEvent, render, screen } from "@testing-library/react"
import { describe, expect, it } from "vitest"
import { HomeNavbarClient } from "@/components/home/home-navbar-client"

describe("HomeNavbarClient", () => {
  it("opens and closes dropdown menus", () => {
    render(<HomeNavbarClient displayName="Chris" isSignedIn isAdministrator adminEnabled />)

    const libraryButton = screen.getByRole("button", { name: /library menu/i })
    expect(libraryButton).toHaveClass("cursor-pointer")

    fireEvent.click(libraryButton)
    expect(screen.getByRole("link", { name: "Collections" })).toBeInTheDocument()
    expect(screen.getByRole("link", { name: "About Analytics" })).toBeInTheDocument()

    const userButton = screen.getByRole("button", { name: /user menu/i })
    fireEvent.click(userButton)
    expect(screen.getByRole("link", { name: "Sign out" })).toBeInTheDocument()
    expect(screen.getByText("Admin")).toBeInTheDocument()

    fireEvent.click(userButton)
    expect(screen.queryByRole("link", { name: "Sign out" })).not.toBeInTheDocument()
  })
})
