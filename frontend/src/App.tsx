import React from "react";
import {
  BrowserRouter as Router,
  Routes,
  Route,
  Navigate,
  useLocation,
} from "react-router-dom";

import HomePage from "./home/HomePage";
import NavMenu from "./shared/NavMenu";

import AvailabilityCreate from "./Pages/availability/AvailabilityCreate";
import AvailbilityUpdate from "./Pages/availability/AvailabilityUpdate";
import AvailabilityListPage from "./Pages/availability/AvailabilityList";

import AppointmentListPage from "./Pages/appointments/AppointmentList";
import AppointmentCreatePage from "./Pages/appointments/AppointmentCreatePage";
import AppointmentUpdatePage from "./Pages/appointments/AppointmentUpdate";

import LoginPage from "./auth/LoginPage";
import RegisterPage from "./auth/RegisterPage";
import ProtectedRoute from "./auth/ProtectedRoute";
import { AuthProvider } from "./auth/AuthContext";

import DashboardPage from "./dashboard/DashboardPage";

// Admin-sider
import AdminUsersPage from "./admin/AdminUserPage";
import PersonnelDashboard from "./dashboard/PersonnelDashboard";
import PatientDashboard from "./dashboard/PatientDashboard";
import AdminDashboard from "./dashboard/AdminDashboardPage";

import "./App.css";

const AppContent: React.FC = () => {
  const location = useLocation();
  const hideNavbar = ["/login", "/register"].includes(location.pathname);

  return (
    <>
      {!hideNavbar && <NavMenu />}
      <div className="app-content">
        <Routes>
          {/* Public pages */}
          <Route path="/" element={<HomePage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />

          {/* You must be logged in to view these pages*/}
          <Route element={<ProtectedRoute />}>
            {/* Dashboard */}
            <Route path="/dashboard" element={<DashboardPage />} />
            <Route path="/personnel/dashboard" element={<PersonnelDashboard />} />  
            <Route path="/patient/dashboard" element={<PatientDashboard />} />
            <Route path="/admin/dashboard" element={<AdminDashboard />} />


            {/* Availability */}
            <Route path="/availability" element={<AvailabilityListPage />} />
            <Route
              path="/availability/create"
              element={<AvailabilityCreate />}
            />
            <Route
              path="/availability/edit/:availabilityId"
              element={<AvailbilityUpdate />}
            />

            {/* Appointments – for user that is logged in */}
            <Route path="/appointments" element={<AppointmentListPage />} />
            <Route
              path="/appointments/create"
              element={<AppointmentCreatePage />}
            />
            <Route
              path="/appointments/edit/:appointmentId"
              element={<AppointmentUpdatePage />}
            />

            {/* Admin-side */}
            <Route path="/admin" element={<AdminUsersPage />} />

          </Route>

          {/* Fallback */}
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </div>
    </>
  );
};

const App: React.FC = () => {
  return (
    <AuthProvider>
      <Router>
        <AppContent />
      </Router>
    </AuthProvider>
  );
};

export default App;
