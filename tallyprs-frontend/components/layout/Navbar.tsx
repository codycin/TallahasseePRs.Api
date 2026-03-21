import Link from "next/link";
import { BiDotsVerticalRounded } from "react-icons/bi";

export default function Navbar() {
  return (
    <nav className="border-b px-6 py-4 flex items-center justify-between">
      <Link href="/" className="text-xl font-bold">
        TallyPRs
      </Link>
      <BiDotsVerticalRounded />
    </nav>
  );
}
