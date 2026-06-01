import { useEffect, useState } from 'react';
import { booksApi } from '../api/booksApi';
import type { BookModel } from '../models/book';
import { getApiErrorMessage } from '../helpers/errorHelper';
import{ BookTable } from '../components/BookTable';
import { BookForm } from '../components/BookForm';

export function BooksPage() {
  const [books, setBooks] = useState<BookModel[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [, setError] = useState<string | null>(null);
  const [, setSuccess] = useState<string | null>(null);

  async function loadBooks(status?: string, search?: string) {
    try {
      setError(null);
      const data = await booksApi.getAll(status, search);
      setBooks(data);
    } catch (error) {
      setError(getApiErrorMessage(error));
    }
  }

  async function searchBooks() {
    await loadBooks(undefined, searchTerm);
  }

  async function showAvailableBooks() {
    await loadBooks('available', searchTerm);
  }

  async function handleBorrow(code: string) {
    try {
      await booksApi.borrow(code);
      setSuccess('Book borrowed successfully.');
      await loadBooks();
    } catch (error) {
      setError(getApiErrorMessage(error));
    }
  }

  async function handleReturn(code: string) {
    try {
      await booksApi.returnBook(code);
      setSuccess('Book returned successfully.');
      await loadBooks();
    } catch (error) {
      setError(getApiErrorMessage(error));
    }
  }

  async function handleDelete(code: string) {
    try {
      await booksApi.delete(code);
      setSuccess('Book deleted successfully.');
      await loadBooks();
    } catch (error) {
      setError(getApiErrorMessage(error));
    }
  }

  useEffect(() => {
    const loadInitialBooks = async () => {
      await loadBooks();
    };

    void loadInitialBooks();
  }, []);

  return (
        <>
            <header className="app-header">
            <h1>Library Manager</h1>
            <p className="lead mb-0">Manage your library collection with ease</p>
            </header>

            <main className="container pb-5">
            <div className="card card-soft mb-4">
                <div className="card-body p-4">
                <h2 className="h4 fw-bold text-primary mb-1">Add New Book</h2>
                <p className="text-muted mb-4">
                    Fill in the details below to add a new book to the library
                </p>
                <hr className="border-secondary-subtle my-4" />

                <BookForm onBookCreated={loadBooks} />
                </div>
            </div>

            <div className="card card-soft mb-4">
                <div className="card-body p-4">
                <div className="d-flex gap-2">
                    <input
                    className="form-control"
                    placeholder="Search by title, author or code"
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    />

                    <button className="btn btn-primary" onClick={searchBooks}>
                    Search
                    </button>

                    <button className="btn btn-secondary" onClick={() => loadBooks()}>
                    Show All
                    </button>

                    <button className="btn btn-success" onClick={showAvailableBooks}>
                    Available
                    </button>
                </div>
                </div>
            </div>

            <BookTable
                books={books}
                onBorrow={handleBorrow}
                onReturn={handleReturn}
                onDelete={handleDelete}
            />
            </main>
        </>
        );
}