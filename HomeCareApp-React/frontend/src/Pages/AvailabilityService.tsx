const API_URL = import.meta.env.VITE_API_URL;

const getAuthHeaders = () => {
  const token = localStorage.getItem('token');
  const headers: HeadersInit = {
    'Content-Type': 'application/json',
  };
  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }
  return headers;
};
const handleResponse = async (response: Response) => {
  if (response.ok) {
    if (response.status === 204) {
      return null;
    }
    return response.json();
  } else {
    const errorText = await response.text();
    throw new Error(errorText || `Network response was not OK (${response.status})`);
  }
};

// Get list of availabilities
export const fetchAvailabilities = async (availabilityId?: string) => {
    const url = availabilityId
      ? `${API_URL}/api/availability/${availabilityId}`
      : `${API_URL}/api/availability/list`;
  
    const response = await fetch(url, { credentials: "include" });
    return handleResponse(response);
  };
  

// Create new availability
export const createAvailability = async (availability: any) => {
  const response = await fetch(`${API_URL}/api/availability/create`, {
    method: "POST",
    headers:getAuthHeaders(),
    body: JSON.stringify(availability),
  });
  return handleResponse(response);
};



// Update existing availability
export const updateAvailability = async (availabilityId: number, availability: any) => {
  const response = await fetch(`${API_URL}/api/availability/update/${availabilityId}`, {
    method: "PUT",
    headers:getAuthHeaders(),
    body: JSON.stringify(availability),
  });
  return handleResponse(response);
};

// Delete availability
export const deleteAvailability = async (availabilityId: number) => {
  const response = await fetch(`${API_URL}/api/availability/delete/${availabilityId}`, {
    method: "DELETE",
    headers: getAuthHeaders(),
    credentials: "include",
  });
  return handleResponse(response);
};
