import { Appointment, AppointmentCreatePayload } from "../../types/Appointment";

const API_URL = import.meta.env.VITE_API_URL;

const getAuthHeaders = () => {
  const token = localStorage.getItem("token");
  const headers: HeadersInit = {
    "Content-Type": "application/json",
  };
  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }
  return headers;
};

const handleResponse = async (response: Response) => {
  if (response.ok) {
    if (response.status === 204) return null;
    return response.json();
  } else {
    const errorText = await response.text();
    throw new Error(
      errorText || `Network response was not OK (${response.status})`
    );
  }
};

// GET: list
export const fetchAppointments = async (): Promise<Appointment[]> => {
  const response = await fetch(`${API_URL}/api/appointments/list`, {
    method: "GET",
    headers: getAuthHeaders(),
  });
  return handleResponse(response);
};

// GET: one
export const getAppointment = async (id: number): Promise<Appointment> => {
  const response = await fetch(`${API_URL}/api/appointments/${id}`, {
    method: "GET",
    headers: getAuthHeaders(),
  });
  return handleResponse(response);
};

// POST: create
export const createAppointment = async (
  payload: AppointmentCreatePayload
): Promise<Appointment> => {
  const response = await fetch(`${API_URL}/api/appointments/create`, {
    method: "POST",
    headers: getAuthHeaders(),
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
    headers: getAuthHeaders(),
    body: JSON.stringify(payload),
  });
  await handleResponse(response);
};

// DELETE
export const deleteAppointment = async (id: number): Promise<void> => {
  const response = await fetch(`${API_URL}/api/appointments/delete/${id}`, {
    method: "DELETE",
    headers: getAuthHeaders(),
  });
  await handleResponse(response);
};
