export interface CatalogBook {
  id: string;
  imageId?: string | null;
  title: string;
  description: string;
  textEditionFields: { exist: boolean; totalPages: number };
  audiobookFields: { exist: boolean; totalMinutes: number };
}

export type MediaType = "Ebook" | "Paperback" | "Audiobook";

export interface LibraryBook {
  id: string;
  userId: string;
  catalogBookId?: string | null;
  imageId?: string | null;
  title: string;
  description: string;
  mediaType: MediaType;
  length: number;
  addedToLibraryAt: string;
  lengthUnit: "Pages" | "Minutes";
}

export interface UserProfile {
  name: string;
}

export interface Tracking {
  id: string;
  libraryBookId: string;
  userId: string;
  totalLength: number;
  currentIndex: number;
  isStarted: boolean;
  isFinished: boolean;
}

export interface MailMessage {
  receiver: string;
  subject: string;
  message: string;
  sentAt: string;
}
