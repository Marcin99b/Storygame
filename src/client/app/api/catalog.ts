import { api } from "./client";
import type { CatalogBook } from "./types";

export interface CatalogSearchParams {
  titleContains?: string;
  hasTextEdition?: boolean;
  hasAudiobook?: boolean;
}

export interface SearchCatalogResult {
  books: CatalogBook[];
}

export const catalogApi = {
  search: (params: CatalogSearchParams = {}) => {
    const query = new URLSearchParams();
    if (params.titleContains) query.set("titleContains", params.titleContains);
    if (params.hasTextEdition !== undefined)
      query.set("hasTextEdition", String(params.hasTextEdition));
    if (params.hasAudiobook !== undefined)
      query.set("hasAudiobook", String(params.hasAudiobook));
    const qs = query.toString();
    return api.get<SearchCatalogResult>(`/catalog/${qs ? `?${qs}` : ""}`);
  },
};
