import { QueryClient } from "@tanstack/react-query";

// Cliente único do TanStack Query — server-state, cache e estados de
// loading/error de forma declarativa (o padrão corporativo p/ dados remotos).
export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      refetchOnWindowFocus: false,
      staleTime: 30_000,
    },
  },
});
