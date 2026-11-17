"use server"

import { revalidatePath } from "next/cache"
import { redirect } from "next/navigation"
import { createTerm, deleteTerm, updateTerm } from "@/lib/terms/api"
import type { TermWriteBody } from "@/lib/terms/types"

export async function createTermAction(
  body: TermWriteBody,
): Promise<{ error: string } | undefined> {
  const result = await createTerm(body)
  if (!result.ok) {
    return { error: result.message }
  }
  revalidatePath("/terms")
  redirect(`/terms?id=${result.data.id}`)
}

export async function updateTermAction(
  id: number,
  body: TermWriteBody,
): Promise<{ error: string } | undefined> {
  const result = await updateTerm(id, body)
  if (!result.ok) {
    return { error: result.message }
  }
  revalidatePath("/terms")
  redirect(`/terms?id=${id}`)
}

export async function deleteTermAction(id: number): Promise<{ error: string } | undefined> {
  const result = await deleteTerm(id)
  if (!result.ok) {
    return { error: result.message }
  }
  revalidatePath("/terms")
  redirect("/terms")
}