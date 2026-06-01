export const BookStatus = {
  Available: 1 as const,
  Borrowed: 2 as const,
};

export type BookStatus = typeof BookStatus[keyof typeof BookStatus];

export interface BookModel {
  id: string;
  title: string;
  author: string;
  year: number;
  code: string;
  status: BookStatus;
}

export interface CreateBookRequest {
  title: string;
  author: string;
  year: number;
  code: string;
}

export interface UpdateBookRequest {
  title: string;
  author: string;
  year: number;
}

export function getBookStatusLabel(status: BookStatus): string {
  if (status === BookStatus.Available) {
    return 'Available';
  }

  if (status === BookStatus.Borrowed) {
    return 'Borrowed';
  }

  return 'Unknown';
}