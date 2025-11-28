import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import Container from "react-bootstrap/Container";
import HomePage from "./home/HomePage";
import NavMenu from "./shared/NavMenua";
import AvailabilityCreate from "./Pages/AvailabilityCreat";
import AvailbilityUpdate from "./Pages/AvailabilityUpdate";
import "./App.css";
import AvailabilityListPage from "./Pages/AvailabilityList";
import React from "react";

const App = () => {
  return (
    <Router>
      <NavMenu />
      <Container>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/availability" element={<AvailabilityListPage />} />
          <Route path="/availability/create" element={<AvailabilityCreate />} />
          <Route
            path="/availability/edit/:availabilityId"
            element={<AvailbilityUpdate />}
          />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </Container>
    </Router>
  );
};

export default App;
