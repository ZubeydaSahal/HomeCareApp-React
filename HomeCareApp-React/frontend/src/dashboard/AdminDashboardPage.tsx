
import React from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

const AdminDashboard: React.FC = () => {
  const { user } = useAuth();
  const name = user?.name ?? user?.sub ?? "Admin";

  return (
    <div className="mt-4">
      <h2 className="fw-bold mb-3">Welcome back, {name}!</h2>
      <p className="text-muted mb-4">
        Here you can manage users, personnel and patients.
      </p>

      <div className="row g-3">
        <div className="col-md-6">
          <Link to="/admin" className="text-decoration-none">
            <div className="card border-dark bg-light h-100">
              <div className="card-body">
                <h5 className="fw-bold mb-1 text-dark">View all patients</h5>
                <p className="text-muted mb-0">
                  See a list of all registered patients.
                </p>
              </div>
            </div>
          </Link>
        </div>
        <div className="col-md-6">
          <Link to="/admin/personnel" className="text-decoration-none">
            <div className="card border-dark bg-light h-100">
              <div className="card-body">
                <h5 className="fw-bold mb-1 text-dark">Manage personnel</h5>
                <p className="text-muted mb-0">
                  Add, edit or remove personnel accounts.
                </p>
              </div>
            </div>
          </Link>
          </div>
      </div>
    </div>
  );
};

export default AdminDashboard;
