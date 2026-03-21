"use client";

import { useEffect, useState } from "react";
import { testBackendConnection } from "@/services/testService";

export default function HomePage() {
  const [data, setData] = useState<unknown>(null);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function load() {
      try {
        const result = await testBackendConnection();
        setData(result);
      } catch (err) {
        console.error(err);
        setError("Could not connect to backend.");
      } finally {
        setLoading(false);
      }
    }

    load();
  }, []);

  return (
    <main className="min-h-screen p-8">
      <h1 className="text-3xl font-bold mb-4">TallahasseePRs Frontend Test</h1>

      {loading && <p>Loading...</p>}
      {error && <p className="text-red-600">{error}</p>}

      {!loading && !error && (
        <pre className="bg-gray-100 p-4 text-black rounded text-sm overflow-x-auto">
          {JSON.stringify(data, null, 2)}
        </pre>
      )}
    </main>
  );
}
