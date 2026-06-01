import { useState } from 'react';
import { booksApi } from '../api/booksApi';
import { getApiErrorMessage } from '../helpers/errorHelper';

interface Props {
  onBookCreated: () => void;
}

export function BookForm({ onBookCreated }: Props) {
  const [title, setTitle] = useState('');
  const [author, setAuthor] = useState('');
  const [year, setYear] = useState<number>(2024);
  const [code, setCode] = useState('');
  const [, setError] = useState<string | null>(null);

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();

    try {
      setError(null);

      await booksApi.create({
        title,
        author,
        year,
        code,
      });

      setTitle('');
      setAuthor('');
      setYear(2024);
      setCode('');

      onBookCreated();
    } catch (error) {
      setError(getApiErrorMessage(error));
    }
  }

  return (
    <form onSubmit={handleSubmit} className="row g-3 align-items-end">
        <div className="col-md-3">
            <label className="form-label fw-semibold">Code</label>
            <input className="form-control" placeholder="e.g. AI-91" value={code}
            onChange={(e) => setCode(e.target.value)} />
        </div>

        <div className="col-md-3">
            <label className="form-label fw-semibold">Title</label>
            <input className="form-control" placeholder="e.g. Clean Code" value={title}
            onChange={(e) => setTitle(e.target.value)} />
        </div>

        <div className="col-md-3">
            <label className="form-label fw-semibold">Author</label>
            <input className="form-control" placeholder="e.g. Robert Martin" value={author}
            onChange={(e) => setAuthor(e.target.value)} />
        </div>

        <div className="col-md-2">
            <label className="form-label fw-semibold">Year</label>
            <input className="form-control" type="number" value={year}
            onChange={(e) => setYear(Number(e.target.value))} />
        </div>

        <div className="col-md-1 d-grid">
            <button className="btn btn-primary" type="submit">Add</button>
        </div>
    </form>
  );
}