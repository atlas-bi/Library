import type { Metadata } from "next"
import { LibraryShell } from "@/components/layout/library-shell"
import { type AuthUser, getCurrentUser } from "@/lib/auth"

export const metadata: Metadata = {
  title: "Analytics Cheat Sheet",
  description:
    "An overview of analytics data sources, reporting tools, and validation tags used by the Analytics team.",
}

function resolveDisplayName(user: AuthUser | null): string {
  if (!user) return "Guest"
  return user.fullname && user.fullname !== "Guest" ? user.fullname : user.username || "Guest"
}

function getShellProps(user: AuthUser | null) {
  return {
    displayName: resolveDisplayName(user),
    isSignedIn: !!user,
    isAdministrator: !!user && user.roles.includes("Administrator"),
    adminEnabled: user?.adminEnabled ?? false,
  }
}

export default async function AboutAnalyticsPage() {
  const user = await getCurrentUser()
  const shellProps = getShellProps(user)

  return (
    <LibraryShell {...shellProps}>
      <article className="about-analytics-content mx-auto max-w-3xl py-6 text-sm leading-relaxed text-[var(--atlas-home-text)]">
        <h1 className="mb-6 text-3xl font-bold text-[var(--atlas-home-title)]">
          Analytics Cheat Sheet
        </h1>

        {/* ── Data Sources ─────────────────────────────────────────── */}
        <section aria-labelledby="data-sources-heading" className="mb-8">
          <h2
            id="data-sources-heading"
            className="mb-4 text-2xl font-semibold text-[var(--atlas-home-title)]"
          >
            Data Sources
          </h2>

          <div className="mb-4">
            <h3 className="mb-1 text-lg font-semibold text-[var(--atlas-home-link)]">Chronicles</h3>
            <p className="mb-1 font-medium italic">Real-time production database</p>
            <p>
              Chronicles is the Epic production system, the database management system that
              underlies all of Epic&rsquo;s applications. Each application builds upon the
              Chronicles unified data model to create clinical, financial, and administrative data
              sets. Because user-entered data is immediately available in Chronicles for real-time
              reporting, Hyperspace users requiring up to-the-minute results will most benefit from
              Chronicles-based reporting.
            </p>
          </div>

          <div className="mb-4">
            <h3 className="mb-1 text-lg font-semibold text-[var(--atlas-home-link)]">Clarity</h3>
            <p className="mb-1 font-medium italic">Operational data store</p>
            <p>
              Clarity extracts data from Chronicles and loads it to a dedicated reporting server.
              You can use this data to create analytical reports on large amounts of long-term data
              without slowing your production system. Use the Clarity database for reports that
              don&rsquo;t need access to real-time data and require significant analysis.
            </p>
          </div>

          <div className="mb-4">
            <h3 className="mb-1 text-lg font-semibold text-[var(--atlas-home-link)]">Caboodle</h3>
            <p className="mb-1 font-medium italic">Enterprise data warehouse</p>
            <p>
              Caboodle, an enterprise data warehouse platform, brings together information from
              varied sources in a curated healthcare data model for improved data management and
              analytics. The Caboodle data warehouse can help your organization get more value from
              the information in your system and broaden the scope of your analytical reporting.
              Caboodle provides an analytics platform that is intuitive, flexible, and customizable.
            </p>
          </div>

          <div className="mb-4">
            <h3 className="mb-1 text-lg font-semibold text-[var(--atlas-home-link)]">
              Other Third-Party Sources
            </h3>
            <p>
              In addition to the data available from the Epic platform, Analytics utilizes external
              sources of information to expand and enhance our reporting capabilities. These sources
              include market data from COMPData, claims data from Rush Health and Health Alliance,
              legacy data from Affinity, and extracts from our own non-Epic internal systems such as
              employee rosters and budget information. This data is mostly stored alongside Epic
              data on the Clarity and Caboodle servers but is generally referenced separately and
              not considered to be part of those datasets.
            </p>
          </div>
        </section>

        {/* ── Reporting Tools ───────────────────────────────────────── */}
        <section aria-labelledby="reporting-tools-heading" className="mb-8">
          <h2
            id="reporting-tools-heading"
            className="mb-4 text-2xl font-semibold text-[var(--atlas-home-title)]"
          >
            Reporting Tools
          </h2>
          <p className="mb-4">
            Data sitting in a database is not much use to anyone. Reporting tools allow us to track,
            visualize, and understand our data. Here&rsquo;s a breakdown of what we use at
            Riverside:
          </p>

          <div className="mb-4">
            <h3 className="mb-1 text-lg font-semibold text-[var(--atlas-home-link)]">
              Reporting Workbench
            </h3>
            <p className="mb-1 font-medium italic">Real Time Epic-Native Reports</p>
            <p>
              Reporting workbench is Epic&rsquo;s internal reporting tool used for tracking
              near-term, real-time data. It pulls directly from the Chronicles database and returns
              detailed, record-level information. Most reporting workbench reports can only be run
              for the last thirty days, so it is not the best tool for long-term, retrospective
              reporting, but it is a great way to see what&rsquo;s happening in the hospital right
              now. Examples: Hours in Observation Report, COVID Patients In-House Report
            </p>
          </div>

          <div className="mb-4">
            <h3 className="mb-1 text-lg font-semibold text-[var(--atlas-home-link)]">
              Radar Dashboards
            </h3>
            <p className="mb-1 font-medium italic">Real-Time Monitoring and Long Term Trending</p>
            <p>
              Radar is Epic&rsquo;s native dashboarding tool; it allows users to view a collection
              of small tables and graphs in a single report for quick reference and monitoring. Data
              in Radar Dashboards can come from Chronicles, Clarity, or Caboodle and can summarize
              data over longer periods of time than what is typically available in Reporting
              Workbench Reports. While a great resource, customization is limited and other tools
              are required to access the detailed data behind Radar Dashboard summaries. Examples:
              Daily Census Dashboard, COVID Pulse Dashboard
            </p>
          </div>

          <div className="mb-4">
            <h3 className="mb-1 text-lg font-semibold text-[var(--atlas-home-link)]">
              SQL Server Reporting Service (SSRS)
            </h3>
            <p className="mb-1 font-medium italic">Custom Long Term Reports</p>
            <p>
              SSRS is the primary way we report out of the Clarity and Caboodle databases. It gives
              developers the ability to create highly customized reports with record detail and
              summaries and can utilize sophisticated calculations and logic to meet the specific
              needs of the report consumer. SSRS reports can be run both inside and outside of Epic,
              making them easier to access than other reporting tools. SSRS also has better tools
              for setting up and monitoring email subscriptions.
            </p>
            <p className="mt-1">
              Examples: IP to OP Converted Accounts Detail, Medicaid Inpatient Percentage Monthly,
              Part B Only
            </p>
          </div>

          <div className="mb-4">
            <h3 className="mb-1 text-lg font-semibold text-[var(--atlas-home-link)]">Tableau</h3>
            <p className="mb-1 font-medium italic">Powerful Data Visualization</p>
            <p>
              Tableau was new to Riverside in 2020. It is a visualization platform that can
              incorporate and transform data from Clarity, Caboodle, and a variety of other non-Epic
              data sources into powerful interactive visualizations that make it easy for end users
              to gain insight from large datasets.
            </p>
            <p className="mt-1">
              Examples: Cost and Utilization Dashboard, Leakage Dashboard, Pneumonia Readmissions
              Dashboard
            </p>
          </div>
        </section>

        {/* ── Other Tools ───────────────────────────────────────────── */}
        <section aria-labelledby="other-tools-heading" className="mb-8">
          <h2
            id="other-tools-heading"
            className="mb-4 text-2xl font-semibold text-[var(--atlas-home-title)]"
          >
            Other Tools
          </h2>

          <ul className="space-y-3">
            <li>
              <span className="font-semibold text-[var(--atlas-home-link)]">Cobalt</span>
              {" \u2013 "}
              Internally developed dashboard platform that leverages the speed and power of
              Microsoft Analysis Services. Go-to tool for monitoring market conditions using
              COMPData.
            </li>
            <li>
              <span className="font-semibold text-[var(--atlas-home-link)]">Cubes</span>
              {" \u2013 "}
              Interactive data sources that can be used for self-service reporting in Excel. We have
              over 20 cubes in production covering topics such as Denials, Perioperative Services,
              ED Visits, Revenue and Usage, Supply and Drug Costs, Referrals, Blood Utilization,
              Narcotic Orders, Accounts, AR Aging, and Payments.
            </li>
            <li>
              <span className="font-semibold text-[var(--atlas-home-link)]">SlicerDicer</span>
              {" \u2013 "}
              This is Epic&rsquo;s self-service reporting tool that is built into Hyperspace.
              Similar to cubes, SlicerDicer is designed to make it easier for end users to pull data
              for themselves. Epic has released over 50 SlicerDicer data models, though each can
              require significant build and validation work on the customer&rsquo;s end to deploy.
              Currently, the following models are deployed or in the validation pipeline: Patients,
              HB Accounts, Births, HB Denials, PB Denials, HB Transactions, PB Transactions,
              Surgical Services.
            </li>
            <li>
              <span className="font-semibold text-[var(--atlas-home-link)]">Crystal Reports</span>
              {" \u2013 "}
              Crystal Reports is a tool similar to SSRS that can be used for long-term, SQL based
              reporting. Due to platform limitations and licensing costs, SSRS is the preferred
              platform at Riverside for these style reports. However, due to the large number of
              Epic-released Crystal reports that are still useful to our users, we continue to
              support the tool for legacy content.
            </li>
          </ul>
        </section>

        {/* ── Validation Tags ───────────────────────────────────────── */}
        <section aria-labelledby="validation-tags-heading" className="mb-8">
          <h2
            id="validation-tags-heading"
            className="mb-2 text-2xl font-semibold text-[var(--atlas-home-title)]"
          >
            Validation Tags
          </h2>
          <p className="mb-6">
            In an effort to both make more content available and help end users better understand
            the reliability of different reports, we have added Validation Tags to every report in
            Atlas. These tags indicate how reliable a report should be considered based on its
            origin and amount of testing. There are five tags:
          </p>

          <div className="space-y-4">
            <div>
              <h3 className="mb-1 font-semibold text-[var(--atlas-analytics-certified,#1a7a4a)]">
                Analytics Certified
              </h3>
              <p className="ml-4">
                Certified reports have received the highest level of scrutiny from the Analytics
                Team and the owning end user. These reports are the most reliable and accurate in
                the system.
              </p>
            </div>

            <div>
              <h3 className="mb-1 font-semibold text-[var(--atlas-analytics-reviewed,#2563eb)]">
                Analytics Reviewed
              </h3>
              <p className="ml-4">
                Reviewed reports have gone through the standard Analytics code review and validation
                process. Reviewed reports can be trusted for monitoring most operational processes.
              </p>
            </div>

            <div>
              <h3 className="mb-1 font-semibold text-[var(--atlas-epic-released,#7c3aed)]">
                Epic Released
              </h3>
              <p className="ml-4">
                Epic Released Reports are limited to the standard Epic configuration. They cannot be
                customized to match Riverside&rsquo;s unique system build and may not always be
                accurate. Use with caution.
              </p>
            </div>

            <div>
              <h3 className="mb-1 font-semibold text-[var(--atlas-legacy,#92400e)]">Legacy</h3>
              <p className="ml-4">
                Legacy reports have not been validated by the Analytics team for accuracy and
                reliability. Use with caution.
              </p>
            </div>

            <div>
              <h3 className="mb-1 font-semibold text-[var(--atlas-high-risk,#dc2626)]">
                High Risk
              </h3>
              <p className="ml-4">
                High Risk reports have not been validated by the Analytics Team and do not have a
                usage track record. It may display inaccurate or misleading information. Use at your
                own risk.
              </p>
            </div>

            <div>
              <h3 className="mb-1 font-semibold text-[var(--atlas-self-service,#0369a1)]">
                Self-Service
              </h3>
              <p className="ml-4">
                Data has been validated but content produced via self-service is the responsibility
                of the end user.
              </p>
            </div>
          </div>
        </section>
      </article>
    </LibraryShell>
  )
}
