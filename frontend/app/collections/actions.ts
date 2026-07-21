"use server"

import { revalidatePath } from "next/cache"
import { redirect } from "next/navigation"
import {
  createCollection,
  deleteCollection,
  searchCollectionReports,
  searchCollectionTerms,
  updateCollection,
} from "@/lib/collections/api"
import type { CollectionWriteBody } from "@/lib/collections/types"

export async function searchCollectionTermsAction(query: string) {
  return searchCollectionTerms(query)
}

export async function searchCollectionReportsAction(query: string) {
  return searchCollectionReports(query)
}

export async function createCollectionAction(
  body: CollectionWriteBody,
): Promise<{ error: string } | undefined> {
  const result = await createCollection(body)
  if (!result.ok) {
    return { error: result.message }
  }
  revalidatePath("/collections")
  redirect(`/collections?id=${result.data.id}`)
}

export async function updateCollectionAction(
  id: number,
  body: CollectionWriteBody,
): Promise<{ error: string } | undefined> {
  const result = await updateCollection(id, body)
  if (!result.ok) {
    return { error: result.message }
  }
  revalidatePath("/collections")
  redirect(`/collections?id=${id}`)
}

export async function deleteCollectionAction(id: number): Promise<{ error: string } | undefined> {
  const result = await deleteCollection(id)
  if (!result.ok) {
    return { error: result.message }
  }
  revalidatePath("/collections")
  redirect("/collections")
}
