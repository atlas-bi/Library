"use client"

import { Filter, X } from "lucide-react"
import { useCallback, useId, useState } from "react"
import { ProfileDateRangeSelect } from "@/components/profile/profile-date-range-select"
import type { ProfileDateRangeId } from "@/lib/profile/date-ranges"
import type { ProfileFilterItemDto, ProfileFiltersResponseDto } from "@/lib/profile/types"
import { cn } from "@/lib/utils"

export type ProfileSidebarFilters = {
  server: string[]
  database: string[]
  masterFile: string[]
  visible: string[]
  certification: string[]
  availability: string[]
  reportType: number[]
}

export const EMPTY_SIDEBAR_FILTERS: ProfileSidebarFilters = {
  server: [],
  database: [],
  masterFile: [],
  visible: [],
  certification: [],
  availability: [],
  reportType: [],
}

// ----------------------------------------------------------------------------
// Tag-input for free-form multi-value string fields
// ----------------------------------------------------------------------------
function TagInput({
  id,
  label,
  values,
  onAdd,
  onRemove,
  placeholder,
}: {
  id: string
  label: string
  values: string[]
  onAdd: (value: string) => void
  onRemove: (value: string) => void
  placeholder?: string
}) {
  const [draft, setDraft] = useState("")

  const commit = useCallback(() => {
    const trimmed = draft.trim()
    if (trimmed && !values.includes(trimmed)) {
      onAdd(trimmed)
    }
    setDraft("")
  }, [draft, values, onAdd])

  return (
    <div className="space-y-1.5">
      <label htmlFor={id} className="text-xs font-medium text-muted-foreground">
        {label}
      </label>

      {values.length > 0 && (
        <ul className="flex flex-wrap gap-1" aria-label={`Active ${label} filters`}>
          {values.map((v) => (
            <li key={v}>
              <span className="inline-flex items-center gap-0.5 rounded-full border border-border/60 bg-muted px-2 py-0.5 text-xs">
                {v}
                <button
                  type="button"
                  onClick={() => onRemove(v)}
                  className="ml-0.5 text-muted-foreground hover:text-foreground"
                  aria-label={`Remove ${label} filter: ${v}`}
                >
                  <X className="size-3" />
                </button>
              </span>
            </li>
          ))}
        </ul>
      )}

      <div className="flex gap-1">
        <input
          id={id}
          type="text"
          value={draft}
          onChange={(e) => setDraft(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === "Enter" || e.key === ",") {
              e.preventDefault()
              commit()
            }
          }}
          placeholder={placeholder ?? "Type and press Enter"}
          className="w-full rounded-md border border-input bg-background px-2.5 py-1 text-xs shadow-xs outline-none focus-visible:border-ring focus-visible:ring-[3px] focus-visible:ring-ring/50"
        />
        <button
          type="button"
          onClick={commit}
          disabled={!draft.trim()}
          className="rounded-md border border-input bg-muted px-2 py-1 text-xs font-medium hover:bg-muted/80 disabled:opacity-40"
        >
          Add
        </button>
      </div>
    </div>
  )
}

// ----------------------------------------------------------------------------
// Collapsible filter section wrapper
// ----------------------------------------------------------------------------
function FilterSection({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <details className="group/section" open>
      <summary className="flex cursor-pointer select-none list-none items-center justify-between py-1 text-xs font-semibold uppercase tracking-wide text-muted-foreground [&::-webkit-details-marker]:hidden">
        {title}
        <span className="text-muted-foreground/60 transition-transform group-open/section:rotate-180">
          ▾
        </span>
      </summary>
      <div className="mt-2 space-y-2">{children}</div>
    </details>
  )
}

// ----------------------------------------------------------------------------
// Checkbox list for dynamic filters (from backend)
// ----------------------------------------------------------------------------
function CheckboxFilterSection({
  title,
  options,
  selectedValues,
  onChange,
}: {
  title: string
  options: ProfileFilterItemDto[]
  selectedValues: string[]
  onChange: (value: string) => void
}) {
  if (options.length === 0) return null

  return (
    <FilterSection title={title}>
      <fieldset>
        <legend className="sr-only">{title} filter</legend>
        <div className="space-y-1.5 max-h-48 overflow-y-auto pr-2">
          {options.map((opt) => (
            <label key={opt.value} className="flex cursor-pointer items-start gap-2 text-xs group">
              <input
                type="checkbox"
                checked={selectedValues.includes(opt.value)}
                onChange={() => onChange(opt.value)}
                className="mt-0.5 size-3.5 shrink-0 rounded border-input accent-primary"
              />
              <span className="flex-1 text-muted-foreground group-hover:text-foreground transition-colors break-words">
                {opt.label || "(Blank)"}
              </span>
              <span className="text-muted-foreground/50 shrink-0 tabular-nums">
                {opt.count}
              </span>
            </label>
          ))}
        </div>
      </fieldset>
    </FilterSection>
  )
}

// ----------------------------------------------------------------------------
// Main sidebar
// ----------------------------------------------------------------------------
export function ProfileFilterSidebar({
  dateRangeId,
  dateRangeOptions,
  onDateRangeChange,
  filters,
  onFiltersChange,
  filterOptions,
  className,
}: {
  dateRangeId: ProfileDateRangeId
  dateRangeOptions: Array<{ id: ProfileDateRangeId; label: string }>
  onDateRangeChange: (id: ProfileDateRangeId) => void
  filters: ProfileSidebarFilters
  onFiltersChange: (patch: Partial<ProfileSidebarFilters>) => void
  filterOptions?: ProfileFiltersResponseDto | null
  className?: string
}) {
  const uid = useId()

  // Helpers
  const addString = (key: keyof Omit<ProfileSidebarFilters, "reportType">) =>
    (value: string) => onFiltersChange({ [key]: [...filters[key], value] })

  const removeString = (key: keyof Omit<ProfileSidebarFilters, "reportType">) =>
    (value: string) => onFiltersChange({ [key]: filters[key].filter((v) => v !== value) })

  const toggleVisible = (value: string) => {
    const next = filters.visible.includes(value)
      ? filters.visible.filter((v) => v !== value)
      : [...filters.visible, value]
    onFiltersChange({ visible: next })
  }

  const addReportType = (value: string) => {
    const num = Number(value)
    if (!Number.isNaN(num) && !filters.reportType.includes(num)) {
      onFiltersChange({ reportType: [...filters.reportType, num] })
    }
  }
  const removeReportType = (value: string) =>
    onFiltersChange({ reportType: filters.reportType.filter((n) => String(n) !== value) })

  const toggleString = (key: keyof Omit<ProfileSidebarFilters, "reportType">) => (value: string) => {
    const current = filters[key]
    const next = current.includes(value) ? current.filter((v) => v !== value) : [...current, value]
    onFiltersChange({ [key]: next })
  }

  const toggleReportType = (value: string) => {
    const num = Number(value)
    if (Number.isNaN(num)) return
    const current = filters.reportType
    const next = current.includes(num) ? current.filter((n) => n !== num) : [...current, num]
    onFiltersChange({ reportType: next })
  }

  const hasActiveFilters =
    filters.server.length > 0 ||
    filters.database.length > 0 ||
    filters.masterFile.length > 0 ||
    filters.visible.length > 0 ||
    filters.certification.length > 0 ||
    filters.availability.length > 0 ||
    filters.reportType.length > 0

  return (
    <aside
      className={cn(
        "sticky top-0 max-h-[calc(100vh-8rem)] space-y-4 overflow-y-auto pb-4",
        className,
      )}
      aria-label="Profile filters"
    >
      {/* Header */}
      <div className="flex items-center justify-between gap-2">
        <div className="flex items-center gap-2 text-sm font-semibold">
          <Filter className="size-4 text-muted-foreground" aria-hidden="true" />
          <span>Filter Profile</span>
        </div>
        {hasActiveFilters && (
          <button
            type="button"
            onClick={() =>
              onFiltersChange({
                server: [],
                database: [],
                masterFile: [],
                visible: [],
                certification: [],
                availability: [],
                reportType: [],
              })
            }
            className="text-xs text-muted-foreground underline-offset-2 hover:text-foreground hover:underline"
          >
            Clear all
          </button>
        )}
      </div>

      <div className="h-px bg-border/60" />

      {/* Date range */}
      <FilterSection title="Date Range">
        <ProfileDateRangeSelect
          value={dateRangeId}
          options={dateRangeOptions}
          onChange={onDateRangeChange}
          className="w-full"
        />
      </FilterSection>

      <div className="h-px bg-border/60" />

      {/* Server */}
      {filterOptions ? (
        <CheckboxFilterSection
          title="Server"
          options={filterOptions.server}
          selectedValues={filters.server}
          onChange={toggleString("server")}
        />
      ) : (
        <FilterSection title="Server">
          <TagInput
            id={`${uid}-server`}
            label="Server name"
            values={filters.server}
            onAdd={addString("server")}
            onRemove={removeString("server")}
            placeholder="e.g. SQLPROD01"
          />
        </FilterSection>
      )}

      {/* Database */}
      {filterOptions ? (
        <CheckboxFilterSection
          title="Database"
          options={filterOptions.database}
          selectedValues={filters.database}
          onChange={toggleString("database")}
        />
      ) : (
        <FilterSection title="Database">
          <TagInput
            id={`${uid}-database`}
            label="Database name"
            values={filters.database}
            onAdd={addString("database")}
            onRemove={removeString("database")}
            placeholder="e.g. ReportingDB"
          />
        </FilterSection>
      )}

      {/* Master file */}
      {filterOptions ? (
        <CheckboxFilterSection
          title="Master File"
          options={filterOptions.masterFile}
          selectedValues={filters.masterFile}
          onChange={toggleString("masterFile")}
        />
      ) : (
        <FilterSection title="Master File">
          <TagInput
            id={`${uid}-masterFile`}
            label="Master file"
            values={filters.masterFile}
            onAdd={addString("masterFile")}
            onRemove={removeString("masterFile")}
            placeholder="e.g. shared.rdl"
          />
        </FilterSection>
      )}

      {/* Visibility */}
      {filterOptions ? (
        <CheckboxFilterSection
          title="Visibility"
          options={filterOptions.visible}
          selectedValues={filters.visible}
          onChange={toggleString("visible")}
        />
      ) : (
        <FilterSection title="Visibility">
          <fieldset>
            <legend className="sr-only">Visibility filter</legend>
            <div className="space-y-1">
              {["true", "false"].map((v) => (
                <label key={v} className="flex cursor-pointer items-center gap-2 text-xs">
                  <input
                    type="checkbox"
                    checked={filters.visible.includes(v)}
                    onChange={() => toggleVisible(v)}
                    className="size-3.5 rounded border-input accent-primary"
                  />
                  {v === "true" ? "Visible" : "Hidden"}
                </label>
              ))}
            </div>
          </fieldset>
        </FilterSection>
      )}

      {/* Certification */}
      {filterOptions ? (
        <CheckboxFilterSection
          title="Certification"
          options={filterOptions.certification}
          selectedValues={filters.certification}
          onChange={toggleString("certification")}
        />
      ) : (
        <FilterSection title="Certification">
          <TagInput
            id={`${uid}-certification`}
            label="Certification"
            values={filters.certification}
            onAdd={addString("certification")}
            onRemove={removeString("certification")}
            placeholder="e.g. Approved"
          />
        </FilterSection>
      )}

      {/* Availability */}
      {filterOptions ? (
        <CheckboxFilterSection
          title="Availability"
          options={filterOptions.availability}
          selectedValues={filters.availability}
          onChange={toggleString("availability")}
        />
      ) : (
        <FilterSection title="Availability">
          <TagInput
            id={`${uid}-availability`}
            label="Availability"
            values={filters.availability}
            onAdd={addString("availability")}
            onRemove={removeString("availability")}
            placeholder="e.g. Internal"
          />
        </FilterSection>
      )}

      {/* Report type */}
      {filterOptions ? (
        <CheckboxFilterSection
          title="Report Type"
          options={filterOptions.reportType}
          selectedValues={filters.reportType.map(String)}
          onChange={toggleReportType}
        />
      ) : (
        <FilterSection title="Report Type">
          <TagInput
            id={`${uid}-reportType`}
            label="Report type ID"
            values={filters.reportType.map(String)}
            onAdd={addReportType}
            onRemove={removeReportType}
            placeholder="e.g. 1"
          />
        </FilterSection>
      )}
    </aside>
  )
}

