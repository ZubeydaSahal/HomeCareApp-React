import React from "react";
import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import Container from "react-bootstrap/Container";

import HomePage from "./home/HomePage";
import NavMenu from "./shared/NavMenu";
import AvailabilityCreate from "./Pages/AvailabilityCreate";
import AvailbilityUpdate from "./Pages/AvailabilityUpdate";
import AvailabilityListPage from "./Pages/AvailabilityList";

import AppointmentListPage from "./Pages/appointments/AppointmentList";
import AppointmentCreatePage from "./Pages/appointments/AppointmentCreatePage";
import AppointmentUpdatePage from "./Pages/appointments/AppointmentUpdate";

import LoginPage from "./auth/LoginPage";
import RegisterPage from "./auth/RegisterPage";
import ProtectedRoute from "./auth/ProtectedRoute";
import { AuthProvider } from "./auth/AuthContext";

import "./App.css";

const App = () => {
  return (
    <AuthProvider>
      <Router>
        <NavMenu />

        <Container>
          <Routes>
            {/* Offentlige sider */}
            <Route path="/" element={<HomePage />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />

            {/* Alt inni ProtectedRoute krever innlogging */}
            <Route element={<ProtectedRoute />}>
              {/* Availability */}
              <Route path="/availability" element={<AvailabilityListPage />} />
              <Route path="/availability/create" element={<AvailabilityCreate />} />
              <Route
                path="/availability/edit/:availabilityId"
                element={<AvailbilityUpdate />}
              />

              {/* Appointments */}
              <Route path="/appointments" element={<AppointmentListPage />} />
              <Route
                path="/appointments/create"
                element={<AppointmentCreatePage />}
              />
              <Route
                path="/appointments/edit/:appointmentId"
                element={<AppointmentUpdatePage />}
              />
            </Route>

            {/* Fallback */}
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </Container>
      </Router>
    </AuthProvider>
  );
};

export default App;
