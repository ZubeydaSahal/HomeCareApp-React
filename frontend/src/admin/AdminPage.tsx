import React, { useEffect, useState } from "react";
import { fetchPatients, fetchPersonnel } from "./AdminService";
import {
  Spinner,
  Alert,
  Table,
  Container,
  Row,
  Col,
} from "react-bootstrap";
import { useAuth } from "../auth/AuthContext";

const AdminPage: React.FC = () => {
  const { user } = useAuth();

  const [patients, setPatients] = useState<any[]>([]);
  const [personnel, setPersonnel] = useState<any[]>([]);

  const [loadingPatients, setLoadingPatients] = useState(true);
  const [loadingPersonnel, setLoadingPersonnel] = useState(true);

  const [errorPatients, setErrorPatients] = useState<string | null>(null);
  const [errorPersonnel, setErrorPersonnel] = useState<string | null>(null);

  // Role-check: only Admin
  if (!user || user.role !== "Admin") {
    return (
      <Container className="py-4">
        <Alert variant="danger">Access denied. Admin only.</Alert>
      </Container>
    );
  }

  useEffect(() => {
    const load = async () => {
      try {
        setLoadingPatients(true);
        setLoadingPersonnel(true);

        const [patientsData, personnelData] = await Promise.all([
          fetchPatients(),
          fetchPersonnel(),
        ]);

        setPatients(patientsData);
        setPersonnel(personnelData);
      } catch (err) {
        // Du kan logge err hvis du vil
        setErrorPatients("Could not load patient list.");
        setErrorPersonnel("Could not load personnel list.");
      } finally {
        setLoadingPatients(false);
        setLoadingPersonnel(false);
      }
    };

    load();
  }, []);

  return (
    <Container className="py-4">
      <h2 className="fw-bold mb-4">Users overview</h2>

      <Row className="g-4">
        {/* Patients */}
        <Col xs={12} lg={6}>
          <div className="card border-1 border-dark bg-light h-100">
            <div className="card-body">
              <h4 className="fw-bold mb-3 text-dark">Patients</h4>

              {errorPatients && (
                <Alert
                  variant="danger"
                  onClose={() => setErrorPatients(null)}
                  dismissible
                >
                  {errorPatients}
                </Alert>
              )}

              {loadingPatients ? (
                <div className="d-flex align-items-center gap-2">
                  <Spinner animation="border" role="status" />
                  <span>Loading patients...</span>
                </div>
              ) : patients.length === 0 ? (
                <p className="mb-0 text-muted">No patients found.</p>
              ) : (
                <div className="table-responsive mt-2">
                  <Table bordered hover size="sm" className="mb-0">
                    <thead className="table-light">
                      <tr>
                        <th>Full Name</th>
                        <th>Email</th>
                        <th>ID</th>
                      </tr>
                    </thead>
                    <tbody>
                      {patients.map((p) => (
                        <tr key={p.id}>
                          <td>{p.fullName}</td>
                          <td>{p.email}</td>
                          <td>{p.id}</td>
                        </tr>
                      ))}
                    </tbody>
                  </Table>
                </div>
              )}
            </div>
          </div>
        </Col>

        {/* Personnel */}
        <Col xs={12} lg={6}>
          <div className="card border-1 border-dark bg-light h-100">
            <div className="card-body">
              <h4 className="fw-bold mb-3 text-dark">Personnel</h4>

              {errorPersonnel && (
                <Alert
                  variant="danger"
                  onClose={() => setErrorPersonnel(null)}
                  dismissible
                >
                  {errorPersonnel}
                </Alert>
              )}

              {loadingPersonnel ? (
                <div className="d-flex align-items-center gap-2">
                  <Spinner animation="border" role="status" />
                  <span>Loading personnel...</span>
                </div>
              ) : personnel.length === 0 ? (
                <p className="mb-0 text-muted">No personnel found.</p>
              ) : (
                <div className="table-responsive mt-2">
                  <Table bordered hover size="sm" className="mb-0">
                    <thead className="table-light">
                      <tr>
                        <th>Full Name</th>
                        <th>Email</th>
                        <th>ID</th>
                      </tr>
                    </thead>
                    <tbody>
                      {personnel.map((p) => (
                        <tr key={p.id}>
                          <td>{p.fullName}</td>
                          <td>{p.email}</td>
                          <td>{p.id}</td>
                        </tr>
                      ))}
                    </tbody>
                  </Table>
                </div>
              )}
            </div>
          </div>
        </Col>
      </Row>
    </Container>
  );
};

export default AdminPage;
