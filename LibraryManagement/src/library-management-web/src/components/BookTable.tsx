import {
  type BookModel,
  BookStatus,
  getBookStatusLabel,
} from '../models/book';

interface Props {
  books: BookModel[];
  onBorrow: (code: string) => void;
  onReturn: (code: string) => void;
  onDelete: (code: string) => void;
}

export function BookTable({
  books,
  onBorrow,
  onReturn,
  onDelete,
}: Props) {
  return (
    <table className="table table-striped table-hover table-bordered align-middle">
      <thead>
        <tr>
          <th>Code</th>
          <th>Title</th>
          <th>Author</th>
          <th>Year</th>
          <th>Status</th>
          <th>Actions</th>
        </tr>
      </thead>

      <tbody>
        {books.map((book) => (
          <tr key={book.id}>
            <td>{book.code}</td>
            <td>{book.title}</td>
            <td>{book.author}</td>
            <td>{book.year}</td>
            <td>
                <span className="status-badge">
                    {getBookStatusLabel(book.status)}
                </span>
            </td>
            <td>
              {book.status === BookStatus.Available && (
                <button className="btn btn-sm btn-warning me-2" onClick={() => onBorrow(book.code)}>
                  Borrow
                </button>
              )}

              {book.status === BookStatus.Borrowed && (
                <button className="btn btn-sm btn-info me-2" onClick={() => onReturn(book.code)}>
                  Return
                </button>
              )}

              <button onClick={() => onDelete(book.code)}>
                Delete
              </button>
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}