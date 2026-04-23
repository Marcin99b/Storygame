import { api } from "./client";
import type { LibraryBook, MediaType } from "./types";

export interface AddBookRequest {
  catalogBookId?: string | null;
  imageId?: string | null;
  title: string;
  description: string;
  mediaType: MediaType;
  length: number;
}

export interface GetLibraryResult {
  books?: LibraryBook[];
}

export const libraryApi = {
  getBooks: () => api.get<GetLibraryResult>("/library/"),
  addBook: (body: AddBookRequest) => api.post<void>("/library/", body),
};
