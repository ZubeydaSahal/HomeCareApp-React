import React from "react";
import { Navbar, Nav, Container } from "react-bootstrap";
import { Link } from "react-router-dom";
import AuthSection from "../auth/AuthSection";
import { useAuth } from "../auth/AuthContext";

import "../App.css";
import "../css/navbar.css";

const NavMenu: React.FC = () => {
  const { user } = useAuth();

  const isAuthenticated = !!user;
  const isPersonnel = user?.role === "Personnel";
  const isAdmin = user?.role === "Admin";
  const isPatient = user?.role === "Patient";

  return (
    <Navbar expand="lg" bg="white" className="mb-4 navbar-container">
      <Container>
        <Navbar.Brand as={Link} to="/">
          <img
            src="/HomeCareApp-Logo.png"
            alt="Carely Logo"
            className="navbar-logo d-inline-block align-top"
          />
        </Navbar.Brand>

        <Navbar.Toggle aria-controls="main-navbar" />

        <Navbar.Collapse id="main-navbar">
          {/* Venstre side av menyen – sider/aksjoner */}
          <Nav className="me-auto">
            {isAuthenticated && (
              <>
                {/* Personnel + Admin: Create availability */}
                {(isPersonnel || isAdmin) && (
                  <Nav.Link as={Link} to="/availability/create">
                    Create availability
                  </Nav.Link>
                )}

                {/* Admin + Patient: Create appointment */}
                {(isAdmin || isPatient) && (
                  <Nav.Link as={Link} to="/appointments/create">
                    Create appointment
                  </Nav.Link>
                )}
              </>
            )}
          </Nav>

          {/* Høyre side – login / user meny */}
          <Nav className="ms-auto">
            <AuthSection />
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
};

export default NavMenu;
