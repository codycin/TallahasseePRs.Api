import { BiHomeAlt2 } from "react-icons/bi";
import { BiUser } from "react-icons/bi";
import { BiPlusCircle } from "react-icons/bi";

import Link from "next/link";

export default function Footer() {
  return (
    <div className="fixed bottom-0 left-0 z-20 w-full p-4 border-t border-default shadow-sm mb-1 justify-center">
      <div className="flex items-center justify-center space-x-20">
        <Link href="/create">
          <BiPlusCircle className=" w-6 h-6" />
        </Link>
        <Link href="/home">
          <BiHomeAlt2 className=" w-6 h-6" />
        </Link>
        <BiUser className=" w-6 h-6" />
      </div>
    </div>
  );
}
