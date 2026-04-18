export interface CatalogBook {
  id: string;
  imageId?: string;
  title: string;
  description: string;
  textEditionFields: { exist: boolean; totalPages: number };
  audiobookFields: { exist: boolean; totalMinutes: number };
}

export type MediaType = "Ebook" | "Paperback" | "Audiobook";

export interface LibraryBook {
  id: string;
  userId: string;
  catalogBookId?: string;
  title: string;
  description: string;
  mediaType: MediaType;
  length: number;
  addedToLibraryAt: string;
  lengthUnit: "Pages" | "Minutes";
}

export interface UserProfile {
  name: string;
  isVerified: boolean;
}
