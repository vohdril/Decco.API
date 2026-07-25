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
});
