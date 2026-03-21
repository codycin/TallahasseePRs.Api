import Link from "next/link";

export default function NavbarGuest() {
  return (
    <nav className="border-b px-6 py-4 flex items-center justify-between">
      <Link href="/" className="text-xl font-bold">
        TallyPRs
      </Link>

      <div className="flex gap-4">
        <Link href="/login">Login</Link>
        <Link href="/register">Register</Link>
      </div>
    </nav>
  );
}
