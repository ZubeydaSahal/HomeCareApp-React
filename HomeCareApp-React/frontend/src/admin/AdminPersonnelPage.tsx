import React, { useEffect, useState } from "react";
import { fetchPersonnel } from "./AdminService";
import { Spinner, Alert, Table } from "react-bootstrap";
import { useAuth } from "../auth/AuthContext";

const AdminPersonnelPage: React.FC = () => {
  const { user } = useAuth();
  const [personnel, setPersonnel] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

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
        const data = await fetchPersonnel();
        setPersonnel(data);
      } catch (err) {
        setError("Could not load personnel list.");
      } finally {
        setLoading(false);
      }
    };

    load();
  }, []);

  return (
    <div className="container mt-4">
      <h2 className="fw-bold mb-4">All Personnel</h2>

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
            {personnel.map((p) => (
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

export default AdminPersonnelPage;
