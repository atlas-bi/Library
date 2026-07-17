"use server"

import {
  createInitiative,
  deleteInitiative,
  type ShareInitiativeBody,
  searchInitiativeCollections,
  shareInitiative,
  starInitiative,
  unstarInitiative,
  updateInitiative,
} from "./api"
import type { InitiativeWriteBody } from "./types"

export async function deleteInitiativeAction(id: number) {
  return deleteInitiative(id)
}

export async function searchInitiativeCollectionsAction(query: string) {
  return searchInitiativeCollections(query)
}

export async function createInitiativeAction(body: InitiativeWriteBody) {
  return createInitiative(body)
}

export async function updateInitiativeAction(id: number, body: InitiativeWriteBody) {
  return updateInitiative(id, body)
}

export async function starInitiativeAction(id: number) {
  return starInitiative(id)
}

export async function unstarInitiativeAction(id: number) {
  return unstarInitiative(id)
}

export async function shareInitiativeAction(id: number, data: ShareInitiativeBody) {
  return shareInitiative(id, data)
}
