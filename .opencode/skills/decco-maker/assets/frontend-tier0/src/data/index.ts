import type { DeccoApi } from "./gateway";
import { httpApi } from "./httpApi";

export const api: DeccoApi = httpApi;

export type { DeccoApi } from "./gateway";
