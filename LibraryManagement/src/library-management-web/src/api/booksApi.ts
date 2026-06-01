import axios from 'axios';
import type {
    BookModel,
    CreateBookRequest,
    UpdateBookRequest,
} from '../models/book';

const API_BASE_URL = 'http://localhost:5291/api/books';

export const booksApi = {
  getAll: async (status?: string, search?: string): Promise<BookModel[]> => {
  const response = await axios.get<BookModel[]>(API_BASE_URL, {
    params: {
      status,
      search,
    },
  });

  return response.data;
},

  create: async (request: CreateBookRequest): Promise<BookModel> => {
    const response = await axios.post<BookModel>(API_BASE_URL, request);
    return response.data;
  },

  update: async (
    code: string,
    request: UpdateBookRequest
  ): Promise<BookModel> => {
    const response = await axios.put<BookModel>(
      `${API_BASE_URL}/${code}`,
      request
    );

    return response.data;
  },

  delete: async (code: string): Promise<void> => {
    await axios.delete(`${API_BASE_URL}/${code}`);
  },

  borrow: async (code: string): Promise<void> => {
    await axios.post(`${API_BASE_URL}/${code}/borrow`);
  },

  returnBook: async (code: string): Promise<void> => {
    await axios.post(`${API_BASE_URL}/${code}/return`);
  },
};