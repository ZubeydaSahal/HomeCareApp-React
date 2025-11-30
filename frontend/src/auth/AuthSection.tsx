import React from "react";
import { Link } from "react-router-dom";
import { useAuth } from "./AuthContext";
import { Nav, Dropdown, Button } from "react-bootstrap";
import "../css/navbar.css";

const AuthSection: React.FC = () => {
  const { user, logout } = useAuth();

  const displayName = user?.name ?? user?.email ?? user?.sub ?? "User";

  return (
    <Nav className="auth-nav">
      {user ? (
        <Dropdown align="end">
          <Dropdown.Toggle as={Nav.Link} id="dropdown-user" className="user-dropdown-toggle">
            {displayName}
          </Dropdown.Toggle>
          <Dropdown.Menu>
            <Dropdown.Item onClick={logout}>Logout</Dropdown.Item>
          </Dropdown.Menu>
        </Dropdown>
      ) : (
        <>
          <Link to="/login">
            <Button 
              variant="outline-secondary" 
              size="lg"
              className="auth-btn-login"
            >
              Login
            </Button>
          </Link>
          <Link to="/register">
            <Button 
              variant="secondary"
              size="lg"
              className="auth-btn-register"
            >
              Register
            </Button>
          </Link>
        </>
      )}
    </Nav>
  );
};

export default AuthSection;
