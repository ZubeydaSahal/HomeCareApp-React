const API_URL = import.meta.env.VITE_API_URL;

// uses JWT token to creat authorization header
const getAuthHeaders = () => {
  const token = localStorage.getItem("token");
  return {
    "Content-Type": "application/json",
    Authorization: `Bearer ${token}`,
  };
};
//fets all patients only for admin
export const fetchPatients = async () => {
  const response = await fetch(`${API_URL}/api/admin/patients`, {
    method: "GET",
    headers: getAuthHeaders(),
  });
  if (!response.ok) throw new Error("Failed to load patient list");
  return response.json();
};
//gets all personnel also only for admin
export const fetchPersonnel = async () => {
  const response = await fetch(`${API_URL}/api/admin/personnel`, {
    method: "GET",
    headers: getAuthHeaders(),
  });
  if (!response.ok) throw new Error("Failed to load personnel list");
  return response.json();
};
