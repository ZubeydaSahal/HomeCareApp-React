import React from "react";
import { Link } from "react-router-dom";
import { useAuth } from "./AuthContext";
import { Nav, Dropdown } from "react-bootstrap";
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
            <button className="auth-btn-login">
              Login
            </button>
          </Link>
          <Link to="/register">
            <button className="auth-btn-register">
              Register
            </button>
          </Link>
        </>
      )}
    </Nav>
  );
};

export default AuthSection;
