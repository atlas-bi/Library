"use client"

import { BadgeCheck, BarChart3, MessageSquare, Share, Star, TrendingUp } from "lucide-react"
import Link from "next/link"
import { useState, useTransition } from "react"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import { toggleStarAction } from "@/lib/initiatives/actions"
import type { InitiativesListResponseDto } from "@/lib/initiatives/types"
import { ShareModal } from "./share-modal"

interface InitiativesIndexProps {
  data: InitiativesListResponseDto
  canCreateInitiative: boolean
}

export function InitiativesIndex({ data, canCreateInitiative }: InitiativesIndexProps) {
  const [starredMap, setStarredMap] = useState<Record<number, boolean>>(() => {
    const initial: Record<number, boolean> = {}
    data.items.forEach((i) => {
      initial[i.id] = i.isStarred ?? false
    })
    return initial
  })

  const [isPending, startTransition] = useTransition()

  const [shareModalOpen, setShareModalOpen] = useState(false)
  const [shareInitiative, setShareInitiative] = useState<{ id: number; name: string } | null>(null)

  const toggleStar = (id: number) => {
    setStarredMap((prev) => ({
      ...prev,
      [id]: !prev[id],
    }))
    startTransition(() => {
      void (async () => {
        await toggleStarAction(id)
      })()
    })
  }

  const openShareModal = (id: number, name: string) => {
    setShareInitiative({ id, name })
    setShareModalOpen(true)
  }

  return (
    <div className="mx-auto px-4 py-8">
      {/* Breadcrumb matching the screenshot */}
      <nav className="mb-6 flex gap-2 text-[0.9rem] text-gray-500">
        <Link href="/initiatives" className="hover:underline">
          Initiatives
        </Link>
        <span>/</span>
        <span className="text-[#3273dc]">Data Sorting</span>
        <span>/</span>
        <span className="text-[#3273dc]">New Initiative</span>
      </nav>

      <h1 className="mb-6 font-serif text-[3rem] font-bold leading-tight text-[#363636]">
        Initiatives
      </h1>

      {canCreateInitiative && (
        <div className="mb-6">
          <Link href="/initiatives/new">
            <Button
              variant="outline"
              className="h-10 rounded-[4px] border-[#dbdbdb] bg-white px-4 text-[1rem] font-normal text-[#363636] shadow-none hover:border-[#b5b5b5] hover:text-[#363636]"
            >
              <span className="mr-2 text-gray-500">+</span> Create an Initiative
            </Button>
          </Link>
        </div>
      )}

      {data.items.length === 0 ? (
        <Card className="rounded-[4px] shadow-[0_2px_3px_rgba(10,10,10,0.1),0_0_0_1px_rgba(10,10,10,0.1)]">
          <CardContent className="py-10 text-center text-sm text-gray-500">
            No initiatives found.
          </CardContent>
        </Card>
      ) : (
        <div className="grid gap-6 md:grid-cols-2">
          {data.items.map((item) => {
            const isStarred = starredMap[item.id]
            const starCountDisplay =
              isStarred && !item.isStarred
                ? (item.starCount || 0) + 1
                : !isStarred && item.isStarred
                  ? Math.max(0, (item.starCount || 0) - 1)
                  : item.starCount || 0

            return (
              <div key={item.id} className="flex flex-col overflow-hidden rounded-[4px] bg-white shadow-[0_2px_3px_rgba(10,10,10,0.1),0_0_0_1px_rgba(10,10,10,0.1)]">
                {/* Card Header */}
                <div className="relative z-10 flex items-center justify-between bg-white px-5 py-3 shadow-[0_0.125em_0.25em_rgba(10,10,10,0.1)]">
                  <div className="flex items-center gap-2">
                    <Link href={`/initiatives?id=${item.id}`} className="text-[1rem] font-bold text-[#363636] hover:text-[#363636]">
                      {item.name}
                    </Link>
                    <BadgeCheck className="h-[1.15rem] w-[1.15rem] text-[#209cee]" fill="currentColor" stroke="white" />
                  </div>
                  <div className="flex items-center gap-2">
                    <span className="inline-flex h-[2em] items-center whitespace-nowrap rounded-[4px] bg-[#f5f5f5] px-2 text-[0.75rem] leading-none text-[#4a4a4a]">
                      initiative
                    </span>
                  </div>
                </div>

                {/* Card Content */}
                <div className="flex flex-1 gap-5 px-5 pb-4 pt-5">
                  <div className="h-[128px] w-[128px] shrink-0">
                    <picture>
                      <source srcSet="/img/report_placeholder_128x128.webp" type="image/webp" />
                      <img 
                        src="/img/report_placeholder_128x128.png" 
                        alt="placeholder image" 
                        className="block h-auto max-w-full"
                        loading="lazy"
                      />
                    </picture>
                  </div>
                  <div className="flex min-h-[96px] flex-1 flex-col justify-between">
                    <p className="text-[1rem] leading-normal text-[#363636]">
                      {item.description ? (
                        <>
                          {item.description.substring(0, 160)}...{" "}
                          <span className="cursor-pointer text-[#3273dc]">read more</span>
                        </>
                      ) : (
                        <span className="cursor-pointer text-[#3273dc]">Open to view details</span>
                      )}
                    </p>
                  </div>
                </div>

                {/* Card Footer */}
                <div className="flex items-stretch border-t border-[#dbdbdb] bg-white">
                  <button 
                    type="button" 
                    disabled={isPending}
                    onClick={() => toggleStar(item.id)}
                    className={`flex flex-1 items-center justify-center gap-2 border-r border-[#dbdbdb] px-4 py-3 text-[1rem] hover:bg-[#f5f5f5] ${isStarred ? "text-[#363636]" : "text-[#4a4a4a]"}`}
                  >
                    <Star className={`h-4 w-4 ${isStarred ? "fill-[#ffdd57] text-[#ffdd57]" : ""}`} />
                    <span>{isStarred ? "Starred" : "Star"}</span>
                    <span className="inline-flex h-[2em] items-center rounded-full bg-[#f5f5f5] px-3 text-[0.75rem] leading-none text-[#4a4a4a]">
                      {starCountDisplay}
                    </span>
                  </button>
                  {data.features?.sharingEnabled !== false && (
                    <button 
                      type="button" 
                      onClick={() => openShareModal(item.id, item.name)}
                      className={`flex flex-1 items-center justify-center px-4 py-3 text-[#7a7a7a] hover:bg-[#f5f5f5] ${data.features?.feedbackEnabled ? 'border-r border-[#dbdbdb]' : ''}`}
                    >
                      <Share className="h-4 w-4" />
                    </button>
                  )}
                  {data.features?.feedbackEnabled && (
                    <button 
                      type="button" 
                      className="flex flex-1 items-center justify-center px-4 py-3 text-[#7a7a7a] hover:bg-[#f5f5f5]"
                      title="Provide Feedback"
                    >
                      <MessageSquare className="h-4 w-4" />
                    </button>
                  )}
                </div>
            </div>
            )
          })}
        </div>
      )}

      {shareInitiative && (
        <ShareModal
          isOpen={shareModalOpen}
          onClose={() => setShareModalOpen(false)}
          initiativeId={shareInitiative.id}
          initiativeName={shareInitiative.name}
        />
      )}
    </div>
  )
}
