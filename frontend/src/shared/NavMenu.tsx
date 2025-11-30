import React from "react";
import { Navbar, Nav, NavDropdown, Container } from "react-bootstrap";
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
    <Navbar expand="lg" bg="white" variant="light" className="mb-4 navbar-custom">
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
          <Nav className="ms-auto">
            <AuthSection />
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
};

export default NavMenu;
