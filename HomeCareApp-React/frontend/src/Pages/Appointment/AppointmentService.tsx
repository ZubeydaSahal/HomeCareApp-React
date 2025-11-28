// src/appointments/AppointmentService.ts
import { Appointment, AppointmentCreatePayload } from "../../types/Appointment";

const API_URL = import.meta.env.VITE_API_URL;
const headers = {
  "Content-Type": "application/json",
};

const handleResponse = async (response: Response) => {
  if (response.ok) {
    if (response.status === 204) return null;
    return response.json();
  } else {
    const errorText = await response.text();
    throw new Error(errorText || `Network response was not OK (${response.status})`);
  }
};

// GET: list
export const fetchAppointments = async (): Promise<Appointment[]> => {
  const response = await fetch(`${API_URL}/api/appointments/list`, {
    credentials: "include",
  });
  return handleResponse(response);
};

// GET: one
export const getAppointment = async (id: number): Promise<Appointment> => {
  const response = await fetch(`${API_URL}/api/appointments/${id}`, {
    credentials: "include",
  });
  return handleResponse(response);
};

// POST: create
export const createAppointment = async (
  payload: AppointmentCreatePayload
): Promise<Appointment> => {
  const response = await fetch(`${API_URL}/api/appointments/create`, {
    method: "POST",
    headers,
    credentials: "include",
    body: JSON.stringify(payload),
  });
  return handleResponse(response);
};

// PUT: update
export const updateAppointment = async (
  id: number,
  payload: AppointmentCreatePayload
): Promise<void> => {
  const response = await fetch(`${API_URL}/api/appointments/update/${id}`, {
    method: "PUT",
    headers,
    credentials: "include",
    body: JSON.stringify(payload),
  });
  await handleResponse(response);
};

// DELETE
export const deleteAppointment = async (id: number): Promise<void> => {
  const response = await fetch(`${API_URL}/api/appointments/delete/${id}`, {
    method: "DELETE",
    credentials: "include",
  });
  await handleResponse(response);
};
