// src/types/Availability.ts
export interface Availability {
  id?: number;
  personnelId: string;
  date: string;        // eller Date hvis API-et ditt bruker det
  startTime: string;   // eller TimeSpan-konvertert til string
  endTime: string;
  notes?: string | null;
}
