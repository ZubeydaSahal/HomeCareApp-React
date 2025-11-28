// src/types/Appointment.ts
export interface Appointment {
    id: number;
    availabilityId: number;
    clientId: string;
    clientName?: string;
    personnelId?: string;
    personnelName?: string;
    date: string;     
    startTime: string;  
    endTime: string;     
    taskDescription: string;
    status: string;
  }
  
  export interface AppointmentCreatePayload {
    availabilityId: number;
    clientId?: string;     
    taskDescription: string;
    startTime: string;       
    endTime: string;         
    status: string;
  }
  