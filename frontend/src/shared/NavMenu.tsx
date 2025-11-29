import React from "react";
import { Navbar, Nav, NavDropdown, Container } from "react-bootstrap";
import { Link } from "react-router-dom";
import AuthSection from "../auth/AuthSection";
import { useAuth } from "../auth/AuthContext";

import "../App.css";

const NavMenu: React.FC = () => {
  const { user } = useAuth();

  const isAuthenticated = !!user;
  const isPersonnel = user?.role === "Personnel";
  const isAdmin = user?.role === "Admin";
  const isPatient = user?.role === "Patient";

  return (
    <Navbar expand="lg" bg="dark" variant="dark" className="mb-4">
      <Container>
        <Navbar.Brand as={Link} to={isAuthenticated ? "/dashboard" : "/"}>
          HomeCareApp
        </Navbar.Brand>

        <Navbar.Toggle aria-controls="main-navbar" />

        <Navbar.Collapse id="main-navbar">
          <Nav className="me-auto">
            {/* Home-lenke: til /dashboard hvis innlogget, ellers / */}
            <Nav.Link as={Link} to={isAuthenticated ? "/dashboard" : "/"}>
              Home
            </Nav.Link>

            {/* Availability kun for personnel */}
            {isAuthenticated && isPersonnel && (
              <>
                <Nav.Link as={Link} to="/availability">
                  Availability
                </Nav.Link>

                <Nav.Link as={Link} to="/availability/create">
                  New Availability
                </Nav.Link>
              </>
            )}

            {/* Admin-meny kun for adminbrukere */}
            {isAuthenticated && isAdmin && (
              <NavDropdown title="Admin" id="admin-dropdown">
                <NavDropdown.Item as={Link} to="/admin/patients">
                  Patients
                </NavDropdown.Item>
                <NavDropdown.Item as={Link} to="/admin/personnel">
                  Personnel
                </NavDropdown.Item>
              </NavDropdown>
            )}

            {/* Appointments: alle kan se liste, 
               men bare Patient (og ev. Admin) kan lage new */}
            {isAuthenticated && (
              <NavDropdown title="Appointments" id="appointments-dropdown">
                <NavDropdown.Item as={Link} to="/appointments">
                  All appointments
                </NavDropdown.Item>

                {(isPatient || isAdmin) && (
                  <NavDropdown.Item as={Link} to="/appointments/create">
                    New appointment
                  </NavDropdown.Item>
                )}
              </NavDropdown>
            )}
          </Nav>

          <AuthSection />
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
};

export default NavMenu;
