const API_URL = import.meta.env.VITE_API_URL;

const getAuthHeaders = () => {
  const token = localStorage.getItem("token");
  return {
    "Content-Type": "application/json",
    Authorization: `Bearer ${token}`,
  };
};

export const fetchPatients = async () => {
  const response = await fetch(`${API_URL}/api/admin/patients`, {
    method: "GET",
    headers: getAuthHeaders(),
  });
  if (!response.ok) throw new Error("Failed to load patient list");
  return response.json();
};

export const fetchPersonnel = async () => {
  const response = await fetch(`${API_URL}/api/admin/personnel`, {
    method: "GET",
    headers: getAuthHeaders(),
  });
  if (!response.ok) throw new Error("Failed to load personnel list");
  return response.json();
};
