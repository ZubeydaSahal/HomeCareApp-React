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
  console.log("NavMenu - isAuthenticated:", isAuthenticated, "isPersonnel:", isPersonnel);
  console.log("User:", user);
  return (
    <Navbar expand="lg" bg="dark" variant="dark" className="mb-4">
      <Container>
        <Navbar.Brand as={Link} to="/">
          HomeCareApp
        </Navbar.Brand>

        <Navbar.Toggle aria-controls="main-navbar" />

        <Navbar.Collapse id="main-navbar">
          <Nav className="me-auto">
            {/* Offentlig landingsside */}
            <Nav.Link as={Link} to="/">
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

            {/* Appointments: for alle som er logget inn (både patient og personnel) */}
            {isAuthenticated && (
              <NavDropdown title="Appointments" id="appointments-dropdown">
                <NavDropdown.Item as={Link} to="/appointments">
                  All appointments
                </NavDropdown.Item>
                <NavDropdown.Item as={Link} to="/appointments/create">
                  New appointment
                </NavDropdown.Item>
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
