"use client"

import { BarChart2, Edit, Plus, Share, Star, Trash2 } from "lucide-react"
import Link from "next/link"
import { useRouter } from "next/navigation"
import { useState, useTransition } from "react"
import {
  deleteInitiativeAction,
  toggleStarAction,
} from "@/lib/initiatives/actions"
import type { InitiativeDetailDto } from "@/lib/initiatives/types"
import { ShareModal } from "./share-modal"

interface InitiativeDetailProps {
  data: InitiativeDetailDto
  canViewOtherUser?: boolean
  canCreateInitiative?: boolean
}

export function InitiativeDetail({
  data,
  canViewOtherUser = true,
  canCreateInitiative = false,
}: InitiativeDetailProps) {
  const router = useRouter()

  const {
    id,
    name,
    description,
    operationOwner,
    executiveOwner,
    financialImpact,
    strategicImportance,
    lastUpdatedBy,
    lastModifiedDisplay,
    collections,
    canEditInitiative,
    canDeleteInitiative,
    features,
  } = data

  const [isStarred, setIsStarred] = useState(data.isStarred ?? false)
  const [starCount, setStarCount] = useState(data.starCount ?? 0)
  const [isDeleting, setIsDeleting] = useState(false)
  const [shareModalOpen, setShareModalOpen] = useState(false)

  const [isPending, startTransition] = useTransition()

  const toggleStar = () => {
    const previousIsStarred = isStarred
    const previousStarCount = starCount

    setIsStarred(!previousIsStarred)
    setStarCount((prev) => (previousIsStarred ? Math.max(0, prev - 1) : prev + 1))

    startTransition(() => {
      void (async () => {
        const result = await toggleStarAction(id)
        if (result.error) {
          setIsStarred(previousIsStarred)
          setStarCount(previousStarCount)
          alert(`Error updating star: ${result.error}`)
        } else if (result.data) {
          setIsStarred(result.data.isStarred)
          setStarCount(result.data.count)
        }
      })()
    })
  }

  const handleDelete = async () => {
    if (!window.confirm("Are you sure you want to delete this initiative?")) {
      return
    }
    setIsDeleting(true)
    const result = await deleteInitiativeAction(id)
    setIsDeleting(false)
    if (result.error) {
      alert(`Error deleting initiative: ${result.error}`)
    } else {
      router.push("/initiatives")
      router.refresh()
    }
  }

  const handleEdit = () => {
    router.push(`/initiatives/edit?id=${id}`)
  }

  const handleShare = () => {
    setShareModalOpen(true)
  }

  return (
    <div className="mx-auto flex flex-col md:flex-row gap-8 py-8 items-start">
      {/* Left Action Rail */}
      <div className="flex flex-col items-center gap-4 sticky top-24 pt-2">
        {/* Star Button */}
        <button
          type="button"
          disabled={isPending}
          onClick={toggleStar}
          className="relative group"
          title={isStarred ? "Unstar this initiative" : "Star this initiative"}
        >
          <div className="relative inline-block">
            <Star
              className={`h-5 w-5 ${isStarred ? "fill-current text-[#dba71a]" : "text-gray-500"}`}
              strokeWidth={isStarred ? 0 : 2}
            />
            <span className="absolute -top-2 -right-3 flex h-4 min-w-[16px] items-center justify-center rounded-full bg-[#59cbb7] px-1 text-[9px] font-bold text-white">
              {starCount}
            </span>
          </div>
        </button>

        {/* Share Button */}
        {features?.sharingEnabled !== false && (
          <button
            type="button"
            onClick={handleShare}
            className="text-gray-500 hover:text-gray-700 transition-colors"
            title="Share"
          >
            <Share className="h-5 w-5" strokeWidth={2.5} />
          </button>
        )}

        {/* Create Button */}
        {canCreateInitiative && (
          <Link
            href="/initiatives/new"
            className="text-gray-500 hover:text-gray-700 transition-colors"
            title="Create New Initiative"
          >
            <Plus className="h-5 w-5" strokeWidth={2} />
          </Link>
        )}

        {/* Edit Button */}
        {canEditInitiative && (
          <button
            type="button"
            onClick={handleEdit}
            className="text-gray-500 hover:text-gray-700 transition-colors"
            title="Edit Initiative"
          >
            <Edit className="h-5 w-5" strokeWidth={2} />
          </button>
        )}

        {/* Delete Button */}
        {canDeleteInitiative && (
          <button
            type="button"
            onClick={handleDelete}
            disabled={isDeleting}
            className="text-gray-500 hover:text-gray-700 transition-colors disabled:opacity-50"
            title="Delete Initiative"
          >
            <Trash2 className="h-5 w-5" strokeWidth={2} />
          </button>
        )}
      </div>

      {/* Main Content Area */}
      <div className="flex-1 max-w-5xl">
        <h1 className="mb-6 font-serif text-[3rem] font-bold leading-tight text-[#363636]">
          {name}
        </h1>

        <nav className="mb-8 text-[1rem] text-gray-500">
          <ul className="flex items-center gap-2">
            <li>
              <a href="#details" className="text-[#3273dc] hover:underline">
                Details
              </a>
            </li>
            {collections && collections.length > 0 && (
              <>
                <li className="mx-2 text-[#b5b5b5]">/</li>
                <li>
                  <a href="#collections" className="text-[#3273dc] hover:underline">
                    Linked Collections
                  </a>
                </li>
              </>
            )}
          </ul>
        </nav>

        {description && (
          <div className="mb-8">
            <h2 className="mb-4 font-serif text-[2.5rem] font-semibold text-[#363636]">
              Description
            </h2>
            <div className="text-[1rem] leading-relaxed text-[#4a4a4a] whitespace-pre-wrap">
              {description}
            </div>
          </div>
        )}

        <div className="mb-8">
          <h2 id="details" className="mb-4 font-serif text-[2.5rem] font-semibold text-[#363636]">
            Details
          </h2>
          <div className="overflow-x-auto">
            <table className="w-full text-left text-[1rem] text-[#363636]">
              <tbody className="divide-y divide-[#dbdbdb]">
                {operationOwner && (
                  <tr className="hover:bg-[#fafafa]">
                    <td className="py-2 pr-8 font-medium">Operational Owner</td>
                    <td className="py-2">
                      {canViewOtherUser ? (
                        <a
                          href={`/users?id=${operationOwner.id}`}
                          className="text-[#3273dc] hover:underline"
                        >
                          {operationOwner.fullName || operationOwner.username}
                        </a>
                      ) : (
                        <>{operationOwner.fullName || operationOwner.username}</>
                      )}
                    </td>
                  </tr>
                )}
                {executiveOwner && (
                  <tr className="hover:bg-[#fafafa]">
                    <td className="py-2 pr-8 font-medium">Executive Owner</td>
                    <td className="py-2">
                      {canViewOtherUser ? (
                        <a
                          href={`/users?id=${executiveOwner.id}`}
                          className="text-[#3273dc] hover:underline"
                        >
                          {executiveOwner.fullName || executiveOwner.username}
                        </a>
                      ) : (
                        <>{executiveOwner.fullName || executiveOwner.username}</>
                      )}
                    </td>
                  </tr>
                )}
                {financialImpact && (
                  <tr className="hover:bg-[#fafafa]">
                    <td className="py-2 pr-8 font-medium">Financial Impact</td>
                    <td className="py-2">{financialImpact.name}</td>
                  </tr>
                )}
                {strategicImportance && (
                  <tr className="hover:bg-[#fafafa]">
                    <td className="py-2 pr-8 font-medium">Strategic Importance</td>
                    <td className="py-2">{strategicImportance.name}</td>
                  </tr>
                )}
                {lastUpdatedBy && (
                  <tr className="hover:bg-[#fafafa]">
                    <td className="py-2 pr-8 font-medium">Last Updated By</td>
                    <td className="py-2">
                      {canViewOtherUser ? (
                        <a
                          href={`/users?id=${lastUpdatedBy.id}`}
                          className="text-[#3273dc] hover:underline"
                        >
                          {lastUpdatedBy.fullName || lastUpdatedBy.username}
                        </a>
                      ) : (
                        <>{lastUpdatedBy.fullName || lastUpdatedBy.username}</>
                      )}
                    </td>
                  </tr>
                )}
                {lastModifiedDisplay && (
                  <tr className="hover:bg-[#fafafa]">
                    <td className="py-2 pr-8 font-medium">Last Updated</td>
                    <td className="py-2">{lastModifiedDisplay}</td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>

        {collections && collections.length > 0 && (
          <div>
            <h2
              id="collections"
              className="mb-4 font-serif text-[2.5rem] font-semibold text-[#363636]"
            >
              Linked Collections
            </h2>
            <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
              {collections.map((c) => (
                <div
                  key={c.id}
                  className="atlas-home-card flex flex-col overflow-hidden rounded-[8px] border border-[#dbdbdb] bg-white shadow-sm"
                >
                  {/* Card Header */}
                  <div className="flex items-center justify-between px-4 py-3 border-b border-[#dbdbdb]">
                    <Link
                      href={`/collections?id=${c.id}`}
                      className="text-[1rem] font-bold text-[#363636] hover:text-[#3273dc] truncate pr-2"
                    >
                      {c.name}
                    </Link>
                    <div className="flex items-center">
                      <span className="bg-[#f5f5f5] text-[#4a4a4a] px-3 py-[0.15rem] text-[0.7rem] font-semibold tracking-wide uppercase leading-tight rounded">
                        Collection
                      </span>
                    </div>
                  </div>

                  {/* Card Body */}
                  <div className="p-4 flex-1">
                    <div className="flex flex-row items-stretch gap-4">
                      <div className="shrink-0 flex items-start">
                        <picture>
                          <source srcSet="/img/report_placeholder_128x128.webp" type="image/webp" />
                          <img
                            src="/img/report_placeholder_128x128.png"
                            alt={`${c.name} thumbnail`}
                            className="h-28 w-28 rounded-lg border border-[#dbdbdb] object-cover"
                            loading="lazy"
                          />
                        </picture>
                      </div>
                      <div className="flex flex-col flex-1 min-w-0">
                        <Link
                          href={`/collections?id=${c.id}`}
                          className="text-sm font-medium leading-6 text-[#4a4a4a] hover:text-[#4a4a4a]"
                        >
                          <p>
                            {c.description ? (
                              <>
                                <span className="line-clamp-3 inline">{c.description}</span>{" "}
                                <span className="text-[#3273dc] hover:underline whitespace-nowrap">
                                  read more
                                </span>
                              </>
                            ) : (
                              <span className="text-[#3273dc] hover:underline">
                                Open to view details.
                              </span>
                            )}
                          </p>
                        </Link>
                      </div>
                    </div>
                  </div>

                  {/* Card Footer */}
                  <footer className="flex items-center border-t border-[#dbdbdb] bg-[#fafafa]">
                    <div className="flex items-center justify-center border-r border-[#dbdbdb] px-4 py-2 hover:bg-[#f5f5f5] cursor-pointer text-[#4a4a4a] transition-colors">
                      <button className="flex items-center gap-1.5 text-sm font-medium">
                        <Star
                          className={`h-[1.1rem] w-[1.1rem] ${c.isStarred ? "fill-[#dba71a] text-[#dba71a]" : "text-gray-400"}`}
                        />
                        <span>Star {c.starCount}</span>
                      </button>
                    </div>
                    <div className="flex items-center justify-center gap-4 px-4 py-2 text-[#b5b5b5]">
                      <button className="hover:text-[#4a4a4a] transition-colors">
                        <BarChart2 className="h-4 w-4" />
                      </button>
                      {features?.sharingEnabled !== false && (
                        <button className="hover:text-[#4a4a4a] transition-colors">
                          <Share className="h-4 w-4" />
                        </button>
                      )}
                    </div>
                  </footer>
                </div>
              ))}
            </div>
          </div>
        )}
      </div>

      <ShareModal
        isOpen={shareModalOpen}
        onClose={() => setShareModalOpen(false)}
        initiativeName={name}
        initiativeId={id}
      />
    </div>
  )
}
