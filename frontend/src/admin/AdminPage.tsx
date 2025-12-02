import React, { useEffect, useState } from "react";
import { fetchPatients } from "./AdminService";
import { Spinner, Alert, Table } from "react-bootstrap";
import { useAuth } from "../auth/AuthContext";

const AdminPage: React.FC = () => {
  const { user } = useAuth();
  const [patients, setPatients] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Role-check: only Admin
  if (!user || user.role !== "Admin") {
    return (
      <div className="container mt-4">
        <Alert variant="danger">Access denied. Admin only.</Alert>
      </div>
    );
  }

  useEffect(() => {
    const load = async () => {
      try {
        setLoading(true);
        const data = await fetchPatients();
        setPatients(data);
      } catch (err) {
        setError("Could not load patient list.");
      } finally {
        setLoading(false);
      }
    };

    load();
  }, []);

  return (
    <div className="container mt-4">
      <h2 className="fw-bold mb-4">All Patients</h2>

      {error && <Alert variant="danger">{error}</Alert>}

      {loading ? (
        <Spinner animation="border" />
      ) : (
        <Table bordered hover>
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
      )}
    </div>
  );
};

export default AdminPage;
