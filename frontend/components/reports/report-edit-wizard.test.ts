import { describe, expect, it } from "vitest"
import {
  getDefaultReportEditWizardStep,
  parseReportEditWizardStep,
} from "@/components/reports/report-edit-wizard"

describe("report edit wizard steps", () => {
  it("defaults to description", () => {
    expect(getDefaultReportEditWizardStep()).toBe("description")
  })

  it("parses hash fragments for each wizard step", () => {
    expect(parseReportEditWizardStep("#meta")).toBe("meta")
    expect(parseReportEditWizardStep("#complete")).toBe("complete")
    expect(parseReportEditWizardStep("#unknown")).toBeNull()
  })
})
