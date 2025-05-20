import { Link } from 'react-router-dom';

export function Navbar() {
  return (
    <nav className="fixed top-0 w-full bg-gray-800 text-white p-4 shadow z-50">
      <ul className="flex gap-4">
        <li>
          <Link to="/" className="hover:underline">Main</Link>
        </li>
        <li>
          <Link to="/test" className="hover:underline">Test</Link>
        </li>
      </ul>
    </nav>
  );
}
