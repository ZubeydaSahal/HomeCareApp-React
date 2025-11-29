import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import Container from "react-bootstrap/Container";
import HomePage from "./home/HomePage";
import NavMenu from "./shared/NavMenua";
import AvailabilityCreate from "./Pages/AvailabilityCreate";
import AvailbilityUpdate from "./Pages/AvailabilityUpdate";
import "./App.css";
import AvailabilityListPage from "./Pages/AvailabilityList";
import AppointmentListPage  from './Pages/appointments/AppointmentList';
import AppointmentCreatePage from './Pages/appointments/AppointmentCreatePage'
import AppointmentUpdatePage from './Pages/appointments/AppointmentUpdate'
import React from "react";
import LoginPage from './auth/LoginPage'
import RegisterPage from './auth/RegisterPage'
import ProtectedRoute from './auth/ProtectedRoute'
import { AuthProvider } from './auth/AuthContext'


const App = () => {
  return (
    <AuthProvider>
    <Router>
      <NavMenu />
      <Container>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/availability" element={<AvailabilityListPage />} />
          
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route path="*" element={<Navigate to="/" replace />} />
          <Route element={<ProtectedRoute />}>
          <Route path="/availability/create" element={<AvailabilityCreate />} />
          <Route
            path="/availability/edit/:availabilityId"
            element={<AvailbilityUpdate />}
          />
           <Route path="/appointments" element={<AppointmentListPage />} />
          <Route path="/appointments/create" element={<AppointmentCreatePage />} /> 
          <Route
    path="/appointments/edit/:appointmentId"
    element={<AppointmentUpdatePage />}
  />
            </Route>
        </Routes>
      </Container>
    </Router>
    </AuthProvider>
  );
};

export default App;
