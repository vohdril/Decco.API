/// <reference types="vite/client" />

interface ImportMetaEnv {
  /** Base URL da Decco.API */
  readonly VITE_API_BASE_URL?: string;
}
interface ImportMeta {
  readonly env: ImportMetaEnv;
}
