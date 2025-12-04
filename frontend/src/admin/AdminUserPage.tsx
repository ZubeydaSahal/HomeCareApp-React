import React, { useEffect, useState } from "react";
import { Container, Spinner, Alert, Table } from "react-bootstrap";
import { useAuth } from "../auth/AuthContext";
import { fetchPatients, fetchPersonnel } from "./AdminService";

interface AdminUserRow {
  id: string;
  fullName: string;
  email: string;
  role: "Patient" | "Personnel";
}

const AdminUsersPage: React.FC = () => {
  const { user } = useAuth();

  const [users, setUsers] = useState<AdminUserRow[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Only admin can access
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
        setLoading(true);
        setError(null);

        const [patientsData, personnelData] = await Promise.all([
          fetchPatients(),
          fetchPersonnel(),
        ]);

        const patients: AdminUserRow[] = patientsData.map((p: any) => ({
          id: p.id,
          fullName: p.fullName,
          email: p.email,
          role: "Patient",
        }));

        const personnel: AdminUserRow[] = personnelData.map((p: any) => ({
          id: p.id,
          fullName: p.fullName,
          email: p.email,
          role: "Personnel",
        }));

        // slå sammen og sorter litt ryddig
        const combined = [...patients, ...personnel].sort((a, b) => {
          if (a.role !== b.role) {
            return a.role.localeCompare(b.role); // Patient før Personnel
          }
          return a.fullName.localeCompare(b.fullName);
        });

        setUsers(combined);
      } catch (err) {
        console.error(err);
        setError("Could not load users.");
      } finally {
        setLoading(false);
      }
    };

    load();
  }, []);

  return (
    <Container className="py-4">
      <h2 className="fw-bold mb-4">All Users</h2>

      {error && (
        <Alert variant="danger" onClose={() => setError(null)} dismissible>
          {error}
        </Alert>
      )}

      {loading ? (
        <div className="d-flex align-items-center gap-2">
          <Spinner animation="border" role="status" />
          <span>Loading users...</span>
        </div>
      ) : users.length === 0 ? (
        <p className="text-muted mb-0">No users found.</p>
      ) : (
        <div className="table-responsive">
          <Table bordered hover size="sm" className="mb-0">
            <thead className="table-light">
              <tr>
                <th>Full name</th>
                <th>Email</th>
                <th>Role</th>
              </tr>
            </thead>
            <tbody>
              {users.map((u) => (
                <tr key={u.id}>
                  <td>{u.fullName}</td>
                  <td>{u.email}</td>
                  <td>{u.role}</td>
                </tr>
              ))}
            </tbody>
          </Table>
        </div>
      )}
    </Container>
  );
};

export default AdminUsersPage;
