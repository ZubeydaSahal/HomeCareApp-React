import React from "react";
import { Navigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import PatientDashboard from "./PatientDashboard";
import PersonnelDashboard from "./PersonnelDashboard";
import AdminPage from "./AdminDashboardPage";

const DashboardPage: React.FC = () => {
  const { user } = useAuth();

  if (!user) {
    return <Navigate to="/login" replace />;
  }

  const role = user.role; // always "Admin" | "Personnel" | "Patient"

  if (role === "Patient") {
    return <PatientDashboard />;
  }

  if (role === "Personnel") {
    return <PersonnelDashboard />;
  }

  if (role === "Admin") {
    return <AdminPage />;
  }

  return <div className="mt-5">No dashboard available for this role.</div>;
};

export default DashboardPage;
