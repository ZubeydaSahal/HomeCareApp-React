export interface Availability {
  id: number;
  personnelId: string;
  personnelName?: string;
  date: string;
  startTime: string;
  endTime: string;
  notes?: string | null;
  appointmentId?: number | null;
  isBooked?: boolean;
}
