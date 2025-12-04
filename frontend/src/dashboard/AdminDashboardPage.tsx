import React from "react";
import { Link } from "react-router-dom";
import { Container, Row, Col } from "react-bootstrap";
import { useAuth } from "../auth/AuthContext";

const AdminDashboard: React.FC = () => {
  const { user } = useAuth();
  const name = user?.name ?? user?.sub ?? "Admin";

  return (
    <Container className="py-4">
      <div className="bg-light border border-dark rounded-3 p-4 p-md-5 shadow-sm">
        {/* Title */}
        <div className="mb-4">
          <h2 className="fw-bold mb-2">Welcome back, {name}!</h2>
          <p className="text-muted mb-0">
            Here you can manage personnel, patients and appointments.
          </p>
        </div>

        {/* Kort / actions */}
        <Row className="g-4" xs={1} md={2}>
          <Col>
            <Link to="/admin" className="text-decoration-none">
              <div className="card border-1 border-dark bg-white h-100 rounded-3 shadow-sm">
                <div className="card-body p-4 d-flex align-items-center justify-content-between">
                  <div>
                    <h5 className="fw-bold mb-1 text-dark">View all users</h5>
                    <p className="text-muted mb-0">
                      See all registered patients and personnel.
                    </p>
                  </div>
                  <i className="bi bi-people-fill fs-1 text-secondary opacity-75 ms-3"></i>
                </div>
              </div>
            </Link>
          </Col>

          <Col>
            <Link to="/appointments/create" className="text-decoration-none">
              <div className="card border-1 border-dark bg-white h-100 rounded-3 shadow-sm">
                <div className="card-body p-4 d-flex align-items-center justify-content-between">
                  <div>
                    <h5 className="fw-bold mb-1 text-dark">
                      Create appointment
                    </h5>
                    <p className="text-muted mb-0">
                      Book a new appointment for a patient.
                    </p>
                  </div>
                  <i className="bi bi-calendar-plus-fill fs-1 text-secondary opacity-75 ms-3"></i>
                </div>
              </div>
            </Link>
          </Col>
        </Row>
      </div>
    </Container>
  );
};

export default AdminDashboard;
