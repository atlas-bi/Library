import { render, screen } from "@testing-library/react"
import { describe, expect, it } from "vitest"
import { TooltipProvider } from "@/components/ui/tooltip"
import { ShareModal } from "./share-modal"

describe("ShareModal", () => {
  it("renders recipient search instead of accepting an unparsed address", () => {
    render(
      <TooltipProvider>
        <ShareModal
          isOpen={true}
          onClose={() => {}}
          initiativeName="New Initiative"
          initiativeId={12}
        />
      </TooltipProvider>,
    )

    expect(screen.getByLabelText("To:")).toBeDefined()
  })
})
