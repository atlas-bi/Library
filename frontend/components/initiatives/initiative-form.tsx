"use client"

import {
  ArrowLeft,
  ArrowRight,
  Bold,
  Code,
  Eye,
  Heading,
  Italic,
  Link as LinkIcon,
  List,
  Quote,
  X,
} from "lucide-react"
import Link from "next/link"
import { useRouter } from "next/navigation"
import { useState, useTransition } from "react"
import { createInitiativeAction, updateInitiativeAction } from "@/lib/initiatives/actions"
import type { InitiativeDetailDto } from "@/lib/initiatives/types"

export function InitiativeForm({
  mode,
  initial,
  cancelHref,
}: {
  mode: "create" | "edit"
  initial?: InitiativeDetailDto | null
  cancelHref: string
}) {
  const router = useRouter()
  const [name, setName] = useState(initial?.name ?? "")
  const [description, setDescription] = useState(initial?.description ?? "")

  // Mock states for the new fields based on screenshot
  const [operationOwner, setOperationOwner] = useState(
    initial?.operationOwner
      ? {
          id: initial.operationOwner.id,
          name: initial.operationOwner.fullName || initial.operationOwner.username,
        }
      : null,
  )
  const [executiveOwner, setExecutiveOwner] = useState(
    initial?.executiveOwner
      ? {
          id: initial.executiveOwner.id,
          name: initial.executiveOwner.fullName || initial.executiveOwner.username,
        }
      : null,
  )
  const [financialImpact, setFinancialImpact] = useState(initial?.financialImpact ?? null)
  const [strategicImportance, setStrategicImportance] = useState(
    initial?.strategicImportance ?? null,
  )
  const [collectionIds, setCollectionIds] = useState<number[]>(
    initial?.collections?.map((c) => c.id) || [],
  )
  const [collectionQuery, setCollectionQuery] = useState("")

  const [formError, setFormError] = useState<string | null>(null)
  const [isPending, startTransition] = useTransition()

  const submit = () => {
    setFormError(null)
    const trimmedName = name.trim()
    if (!trimmedName) {
      setFormError("Name is required.")
      return
    }

    const body: any = {
      name: trimmedName,
      description: description.trim() ? description.trim() : null,
      purpose: null, // Legacy Razor form doesn't seem to have a purpose field in the screenshot
      collectionIds: collectionIds,
      operationOwnerId: operationOwner?.id ?? null,
      executiveOwnerId: executiveOwner?.id ?? null,
      financialImpactId: financialImpact?.id ?? null,
      strategicImportanceId: strategicImportance?.id ?? null,
      hidden: initial?.hidden ?? null,
    }

    startTransition(() => {
      void (async () => {
        if (mode === "create") {
          const result = await createInitiativeAction(body)
          if (result?.error) {
            setFormError(result.error)
          } else if (result?.data?.id) {
            router.push(`/initiatives?id=${result.data.id}`)
          }
          return
        }

        if (initial?.id) {
          const result = await updateInitiativeAction(initial.id, body)
          if (result?.error) {
            setFormError(result.error)
          } else if (result?.data?.id) {
            router.push(`/initiatives?id=${result.data.id}`)
          }
        }
      })()
    })
  }

  return (
    <div className="mx-auto max-w-7xl py-8">
      <h1 className="mb-6 font-serif text-[3rem] font-bold leading-tight text-[#363636]">
        {mode === "create" ? "New Initiative" : "Edit Initiative"}
      </h1>

      <div className="mb-8 flex items-center justify-between">
        <Link
          href={cancelHref}
          className="inline-flex flex-col items-center justify-center rounded-[4px] border border-[#dbdbdb] bg-white px-5 py-2 text-center text-[#363636] hover:border-[#b5b5b5]"
        >
          <div className="flex items-center gap-2">
            <ArrowLeft className="h-5 w-5" />
            <span className="text-[1rem]">
              <strong>Cancel</strong>
              <br />
              Go Back
            </span>
          </div>
        </Link>
        <button
          type="button"
          onClick={submit}
          disabled={isPending}
          className="inline-flex flex-col items-center justify-center rounded-[4px] border border-[#dbdbdb] bg-white px-5 py-2 text-center text-[#363636] hover:border-[#b5b5b5]"
        >
          <div className="flex items-center gap-2">
            <span className="text-[1rem] text-right">
              <strong>Save</strong>
              <br />
              and Continue
            </span>
            <ArrowRight className="h-5 w-5" />
          </div>
        </button>
      </div>

      {formError && (
        <div className="mb-6 rounded-[4px] bg-red-100 p-4 text-red-700">{formError}</div>
      )}

      <div className="mb-4">
        <label className="mb-2 block text-[1rem] font-bold text-[#363636]">Name</label>
        <div className="control">
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            disabled={isPending}
            className="w-full rounded-[4px] border border-[#dbdbdb] px-3 py-2 text-[1rem] text-[#363636] shadow-[inset_0_0.0625em_0.125em_rgba(10,10,10,0.05)] focus:border-[#3273dc] focus:outline-none"
            placeholder="e.g Data Sorting"
          />
        </div>
      </div>

      <div className="mb-4">
        <label className="mb-2 block text-[1rem] font-bold text-[#363636]">Description</label>
        <div className="overflow-hidden rounded-[4px] border border-[#dbdbdb] bg-white shadow-[inset_0_0.0625em_0.125em_rgba(10,10,10,0.05)] focus-within:border-[#3273dc]">
          {/* WYSIWYG Toolbar matching the Share Modal / screenshot */}
          <div className="flex items-center gap-1 border-b border-[#dbdbdb] bg-[#fafafa] p-1 text-[#7a7a7a]">
            <button
              type="button"
              className="h-7 w-7 rounded-[4px] hover:bg-[#dbdbdb] hover:text-[#363636] flex items-center justify-center"
            >
              <Bold className="h-[14px] w-[14px]" />
            </button>
            <button
              type="button"
              className="h-7 w-7 rounded-[4px] hover:bg-[#dbdbdb] hover:text-[#363636] flex items-center justify-center"
            >
              <Italic className="h-[14px] w-[14px]" />
            </button>
            <button
              type="button"
              className="h-7 w-7 rounded-[4px] hover:bg-[#dbdbdb] hover:text-[#363636] flex items-center justify-center"
            >
              <Heading className="h-[14px] w-[14px]" />
            </button>
            <button
              type="button"
              className="h-7 w-7 rounded-[4px] hover:bg-[#dbdbdb] hover:text-[#363636] flex items-center justify-center"
            >
              <Quote className="h-[14px] w-[14px]" />
            </button>
            <button
              type="button"
              className="h-7 w-7 rounded-[4px] hover:bg-[#dbdbdb] hover:text-[#363636] flex items-center justify-center"
            >
              <Code className="h-[14px] w-[14px]" />
            </button>
            <button
              type="button"
              className="h-7 w-7 rounded-[4px] hover:bg-[#dbdbdb] hover:text-[#363636] flex items-center justify-center"
            >
              <List className="h-[14px] w-[14px]" />
            </button>
            <button
              type="button"
              className="h-7 w-7 rounded-[4px] hover:bg-[#dbdbdb] hover:text-[#363636] flex items-center justify-center"
            >
              <LinkIcon className="h-[14px] w-[14px]" />
            </button>
            <div className="flex-1"></div>
            <button
              type="button"
              className="h-7 w-7 rounded-[4px] hover:bg-[#dbdbdb] hover:text-[#363636] flex items-center justify-center"
            >
              <Eye className="h-[14px] w-[14px]" />
            </button>
          </div>
          <textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            disabled={isPending}
            className="w-full min-h-[120px] resize-none border-0 p-3 text-[1rem] text-[#363636] focus:outline-none"
          />
        </div>
      </div>

      <div className="mb-4">
        <label className="mb-2 block text-[1rem] font-bold text-[#363636]">Operations Owner</label>
        <div className="flex">
          <input
            type="text"
            value={operationOwner?.name ?? ""}
            onChange={(e) =>
              setOperationOwner((prev) =>
                prev ? { ...prev, name: e.target.value } : { id: 0, name: e.target.value },
              )
            }
            disabled={isPending}
            className="w-full rounded-l-[4px] border border-[#dbdbdb] px-3 py-2 text-[1rem] text-[#363636] shadow-[inset_0_0.0625em_0.125em_rgba(10,10,10,0.05)] focus:border-[#3273dc] focus:outline-none focus:relative focus:z-10"
            placeholder="type to search.."
          />
          <button
            type="button"
            onClick={() => setOperationOwner(null)}
            className="flex items-center justify-center rounded-r-[4px] border border-l-0 border-[#dbdbdb] bg-white px-4 text-[#363636] hover:bg-[#f5f5f5]"
          >
            <X className="h-4 w-4" />
          </button>
        </div>
      </div>

      <div className="mb-4">
        <label className="mb-2 block text-[1rem] font-bold text-[#363636]">Executive Owner</label>
        <div className="flex">
          <input
            type="text"
            value={executiveOwner?.name ?? ""}
            onChange={(e) =>
              setExecutiveOwner((prev) =>
                prev ? { ...prev, name: e.target.value } : { id: 0, name: e.target.value },
              )
            }
            disabled={isPending}
            className="w-full rounded-l-[4px] border border-[#dbdbdb] px-3 py-2 text-[1rem] text-[#363636] shadow-[inset_0_0.0625em_0.125em_rgba(10,10,10,0.05)] focus:border-[#3273dc] focus:outline-none focus:relative focus:z-10"
            placeholder="type to search.."
          />
          <button
            type="button"
            onClick={() => setExecutiveOwner(null)}
            className="flex items-center justify-center rounded-r-[4px] border border-l-0 border-[#dbdbdb] bg-white px-4 text-[#363636] hover:bg-[#f5f5f5]"
          >
            <X className="h-4 w-4" />
          </button>
        </div>
      </div>

      <div className="mb-4">
        <label className="mb-2 block text-[1rem] font-bold text-[#363636]">Financial Impact</label>
        <div className="flex">
          <input
            type="text"
            value={financialImpact?.name ?? ""}
            readOnly
            disabled={isPending}
            className="w-full cursor-pointer rounded-l-[4px] border border-[#dbdbdb] bg-white px-3 py-2 text-[1rem] text-[#363636] shadow-[inset_0_0.0625em_0.125em_rgba(10,10,10,0.05)] focus:border-[#3273dc] focus:outline-none focus:relative focus:z-10"
            placeholder="click to select.."
          />
          <button
            type="button"
            onClick={() => setFinancialImpact(null)}
            className="flex items-center justify-center rounded-r-[4px] border border-l-0 border-[#dbdbdb] bg-white px-4 text-[#363636] hover:bg-[#f5f5f5]"
          >
            <X className="h-4 w-4" />
          </button>
        </div>
      </div>

      <div className="mb-4">
        <label className="mb-2 block text-[1rem] font-bold text-[#363636]">
          Strategic Importance
        </label>
        <div className="flex">
          <input
            type="text"
            value={strategicImportance?.name ?? ""}
            readOnly
            disabled={isPending}
            className="w-full cursor-pointer rounded-l-[4px] border border-[#dbdbdb] bg-white px-3 py-2 text-[1rem] text-[#363636] shadow-[inset_0_0.0625em_0.125em_rgba(10,10,10,0.05)] focus:border-[#3273dc] focus:outline-none focus:relative focus:z-10"
            placeholder="click to select.."
          />
          <button
            type="button"
            onClick={() => setStrategicImportance(null)}
            className="flex items-center justify-center rounded-r-[4px] border border-l-0 border-[#dbdbdb] bg-white px-4 text-[#363636] hover:bg-[#f5f5f5]"
          >
            <X className="h-4 w-4" />
          </button>
        </div>
      </div>

      <div className="mb-4">
        <label className="mb-2 block text-[1rem] font-bold text-[#363636]">
          Linked Collections
        </label>
        <div className="control">
          <input
            type="text"
            value={collectionQuery}
            onChange={(e) => setCollectionQuery(e.target.value)}
            disabled={isPending}
            className="w-full rounded-[4px] border border-[#dbdbdb] px-3 py-2 text-[1rem] text-[#363636] shadow-[inset_0_0.0625em_0.125em_rgba(10,10,10,0.05)] focus:border-[#3273dc] focus:outline-none"
            placeholder="search for collections.."
          />
        </div>
      </div>
    </div>
  )
}
