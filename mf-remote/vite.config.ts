import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import { federation } from "@module-federation/vite";

export default defineConfig({
  plugins: [
    react(),
    federation({
      name: "decco-demo",
      filename: "remoteEntry.js",
      exposes: {
        "./DashboardWidget": "./src/Widget.tsx",
      },
      shared: {
        react: {
          singleton: true,
          requiredVersion: "^19.2.0",
        },
        "react-dom": {
          singleton: true,
          requiredVersion: "^19.2.0",
        },
      },
    }),
  ],
  build: {
    target: "esnext",
  },
  // Fixed ports, one step above the Dashboard's (5173 / 4173): Vite's defaults are the
  // Dashboard's ports, and `predev` / `prepreview` free the configured port before starting —
  // with the defaults, starting this remote would stop the Dashboard.
  server: {
    port: 5174,
    strictPort: true,
  },
  preview: {
    port: 4174,
    strictPort: true,
  },
});
