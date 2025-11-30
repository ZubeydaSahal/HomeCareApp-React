import React, { useState } from "react";
import { useAuth } from "./AuthContext";
import { useNavigate, Link } from "react-router-dom";
import "../css/forms.css";

const LoginPage: React.FC = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    try {
      await login({ email, password });
      navigate("/dashboard");
    } catch (err) {
      setError("Invalid email or password.");
      console.error(err);
    }
  };

  return (
    <div className="auth-container">
      {/* Carely Logo */}
      <div className="login-logo">
        <Link to="/">
          <img src="/HomeCareApp-Logo.png" alt="Carely Logo" />
        </Link>
      </div>

      <div className="auth-card">
        {/* Header */}
        <div className="auth-header">
          <h2 className="auth-title">Welcome Back</h2>
          <p className="auth-subtitle">Sign in to continue to Carely</p>
        </div>

        {/* Login Form */}
        <form onSubmit={handleSubmit} className="auth-form">
          {error && (
            <div className="alert alert-danger" role="alert">
              {error}
            </div>
          )}

          <div className="mb-3">
            <label htmlFor="email" className="form-label">
              Email Address
            </label>
            <input
              id="email"
              type="email"
              className="form-control"
              placeholder="you@example.com"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </div>

          <div className="mb-3">
            <label htmlFor="password" className="form-label">
              Password
            </label>
            <input
              id="password"
              type="password"
              className="form-control"
              placeholder="Enter your password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </div>

          <button type="submit" className="btn btn-primary w-100">
            Log In
          </button>
        </form>

        {/* Footer */}
        <div className="auth-footer">
          <p className="mb-0">
            Don't have an account? <Link to="/register">Register here</Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default LoginPage;
