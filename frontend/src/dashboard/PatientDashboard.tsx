import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { fetchAppointments } from "../Pages/appointments/AppointmentService";
import { Appointment } from "../types/Appointment";
import { Spinner, Alert, Container, Row, Col } from "react-bootstrap";

const PatientDashboard: React.FC = () => {
  const { user } = useAuth();
  const [appointments, setAppointments] = useState<Appointment[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const displayName = user?.name ?? "Patient";

  useEffect(() => {
    const load = async () => {
      try {
        setError(null);
        setLoading(true);

        const all = await fetchAppointments();
        console.log("All appointments from API:", all);

        const today = new Date();
        today.setHours(0, 0, 0, 0);

        // Gets user's ID from JWT (nameidentifier claim)
        const userId = (user as any)?.[
          "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
        ];

        console.log("Current userId (from token):", userId);

        const upcoming = all
          .filter((a) => {
            const d = new Date(a.date);
            d.setHours(0, 0, 0, 0);
            const isFutureOrToday = d >= today;

            const isForThisPatient = !userId || a.clientId === userId;

            return isFutureOrToday && isForThisPatient;
          })
          .sort((a, b) => {
            const da = new Date(a.date).getTime();
            const db = new Date(b.date).getTime();
            if (da !== db) return da - db;

            const ta = (a.startTime ?? "").slice(0, 5);
            const tb = (b.startTime ?? "").slice(0, 5);
            return ta.localeCompare(tb);
          });

        console.log("Filtered upcoming appointments:", upcoming);

        setAppointments(upcoming);
      } catch (err) {
        console.error(err);
        setError("Could not load your appointments.");
      } finally {
        setLoading(false);
      }
    };

    load();
  }, [user]);

  const today = new Date();
  today.setHours(0, 0, 0, 0);

  const todayCount = appointments.filter((a) => {
    const d = new Date(a.date);
    d.setHours(0, 0, 0, 0);
    return d.getTime() === today.getTime();
  }).length;

  // Unike caregivers basert på personnelName
  const caregiverNames = Array.from(
    new Set(
      appointments
        .map((a) => a.personnelName)
        .filter((name): name is string => !!name && name.trim() !== "")
    )
  );

  return (
    <Container className="py-4">
      {/* Welcome / top text */}
      <div className="mb-5">
        <h2 className="fw-bold mb-3">Welcome back, {displayName}!</h2>
        {todayCount > 0 ? (
          <p className="text-muted mb-0 fs-5 lh-base">
            You have {todayCount} appointment{todayCount > 1 ? "s" : ""} today.
          </p>
        ) : (
          <p className="text-muted mb-0 fs-5 lh-base">
            You have no appointments scheduled today.
          </p>
        )}
      </div>

      <Row className="g-4">
        {/* Left column – actions + upcoming appointments */}
        <Col xs={12} lg={8}>
          {/* Action cards */}
          <Row xs={1} md={2} className="g-3 mb-4">
            <Col>
              <Link to="/appointments/create" className="text-decoration-none">
                <div className="card border-1 border-dark bg-light h-100 text-center py-4">
                  <div className="card-body">
                    <i className="bi bi-calendar-check display-1 text-secondary mb-3"></i>
                    <h5 className="card-title text-dark mb-0">
                      Book appointment
                    </h5>
                  </div>
                </div>
              </Link>
            </Col>
          </Row>

          {/* Upcoming appointments */}
          <div className="card border-1 border-dark bg-light">
            <div className="card-body p-4">
              <div className="d-flex justify-content-between align-items-center mb-4">
                <h5 className="fw-bold mb-0 text-dark">
                  Upcoming Appointments
                </h5>
                <Link className="btn btn-secondary btn-sm" to="/appointments">
                  view all
                </Link>
              </div>

              {error && (
                <Alert variant="danger" className="mb-3">
                  {error}
                </Alert>
              )}

              {loading ? (
                <div className="text-center py-5">
                  <Spinner animation="border" role="status" />
                </div>
              ) : appointments.length === 0 ? (
                <div className="text-center py-5">
                  <i className="bi bi-calendar-x display-4 text-muted mb-3 d-block"></i>
                  <p className="text-muted mb-3">
                    No upcoming appointments scheduled.
                  </p>
                  <Link className="btn btn-primary" to="/appointments/create">
                    <i className="bi bi-plus-circle me-2"></i>
                    Book appointment
                  </Link>
                </div>
              ) : (
                <div className="vstack gap-3">
                  {appointments.slice(0, 5).map((a) => (
                    <div
                      key={a.id}
                      className="bg-white border rounded p-3 d-flex justify-content-between align-items-start"
                    >
                      <div>
                        <h5 className="mb-2 fw-semibold text-dark lh-base">
                          {a.taskDescription ?? "Home care visit"}
                        </h5>
                        <p className="mb-0 text-muted lh-lg">
                          {a.personnelName ?? "Your caregiver"}
                        </p>
                      </div>
                      <div className="text-end">
                        <div className="text-dark fw-semibold lh-lg">
                          {new Date(a.date).toLocaleDateString("nb-NO")}
                        </div>
                        <div className="text-dark lh-lg">
                          {(a.startTime ?? "").slice(0, 5)}
                        </div>
                      </div>
                    </div>
                  ))}

                  {appointments.length > 5 && (
                    <div className="text-center mt-2">
                      <small className="text-muted">
                        Showing first 5 upcoming appointments.{" "}
                        <Link to="/appointments">View all</Link>
                      </small>
                    </div>
                  )}
                </div>
              )}
            </div>
          </div>
        </Col>

        {/* Right column – CareTeam */}
        <Col xs={12} lg={4}>
          <div className="card border-1 border-dark bg-light h-100" id="careTeam">
            <div className="card-body p-4">
              <h5 className="fw-bold mb-4 text-dark">My Care Team</h5>

              {caregiverNames.length === 0 ? (
                <div className="text-center py-4">
                  <i className="bi bi-people display-4 text-muted mb-3 d-block"></i>
                  <p className="text-muted mb-0 small">
                    No caregivers currently available.
                  </p>
                </div>
              ) : (
                <div>
                  <p className="text-muted small mb-3">
                    You are currently assigned to:
                  </p>
                  <ul className="list-unstyled mb-0">
                    {caregiverNames.map((name) => (
                      <li
                        key={name}
                        className="mb-2 d-flex align-items-center"
                      >
                        <i className="bi bi-person-circle me-2 text-secondary"></i>
                        <span>{name}</span>
                      </li>
                    ))}
                  </ul>
                </div>
              )}
            </div>
          </div>
        </Col>
      </Row>
    </Container>
  );
};

export default PatientDashboard;
