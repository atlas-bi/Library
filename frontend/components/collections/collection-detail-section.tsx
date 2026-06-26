import type { ReactNode } from "react"
import { cn } from "@/lib/utils"

export function CollectionDetailSection({
  id,
  title,
  children,
  className,
}: {
  id: string
  title: string
  children: ReactNode
  className?: string
}) {
  return (
    <section id={id} className={cn("scroll-mt-24 space-y-4", className)}>
      <h2 className="border-b border-border pb-2 text-2xl font-semibold tracking-tight">{title}</h2>
      {children}
    </section>
  )
}

export function CollectionSubsection({ title, children }: { title: string; children: ReactNode }) {
  return (
    <div className="space-y-2">
      <h3 className="text-xl font-semibold tracking-tight">{title}</h3>
      {children}
    </div>
  )
}
