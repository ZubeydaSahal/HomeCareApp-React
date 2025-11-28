import React from "react";
import { Navbar, Nav, NavDropdown, Container } from "react-bootstrap";
import { Link } from "react-router-dom";
import "../App.css";

const NavMenu: React.FC = () => {
  return (
    <Navbar expand="lg" bg="dark" variant="dark" className="mb-4">
      <Container>
        {/* Logo / tittel */}
        <Navbar.Brand as={Link} to="/">
          HomeCareApp
        </Navbar.Brand>

        {/* Hamburger-meny på mobil */}
        <Navbar.Toggle aria-controls="main-navbar" />

        <Navbar.Collapse id="main-navbar">
          <Nav className="me-auto">
            <Nav.Link as={Link} to="/">
              Home
            </Nav.Link>

            <Nav.Link as={Link} to="/availability">
              Availability
            </Nav.Link>

            <Nav.Link as={Link} to="/availability/create">
              New Availability
            </Nav.Link>

            <NavDropdown title="Appointments" id="appointments-dropdown">
              <NavDropdown.Item as={Link} to="/appointments">
                All appointments
              </NavDropdown.Item>
              <NavDropdown.Item as={Link} to="/appointments/create">
                New appointment
              </NavDropdown.Item>
            </NavDropdown>
          </Nav>

          {/* Høyre side – for eksempel login/info senere */}
          <Nav>
            <Nav.Link as={Link} to="/login">
              Login
            </Nav.Link>
            <Nav.Link as={Link} to="/register">
              Register
            </Nav.Link>
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
};

export default NavMenu;
