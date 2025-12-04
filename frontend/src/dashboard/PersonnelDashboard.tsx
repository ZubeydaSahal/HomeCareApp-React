import React, { useEffect, useState } from "react";
import { Spinner, Alert, Container, Row, Col } from "react-bootstrap";
import { Link } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { fetchAppointments } from "../Pages/appointments/AppointmentService";
import { Appointment } from "../types/Appointment";

const PersonnelDashboard: React.FC = () => {
  const { user } = useAuth();
  const [appointments, setAppointments] = useState<Appointment[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const displayName = user?.name ?? "Caregiver";

  useEffect(() => {
    const load = async () => {
      try {
        setError(null);
        setLoading(true);

        const all = await fetchAppointments();
        console.log("All appointments from API (personnel view):", all);

        const today = new Date();
        today.setHours(0, 0, 0, 0);

        // Gets logged-in user's ID from the nameidentifier claim in the JWT
        const userId = (user as any)?.[
          "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
        ];

        console.log("Current personnelId (from token):", userId);

        // Filter all appointments where this user is personnel
        const forThisPersonnel = all.filter(
          (a) => !userId || a.personnelId === userId
        );

        console.log("Appointments for this personnel:", forThisPersonnel);

        setAppointments(forThisPersonnel);
      } catch (err) {
        console.error(err);
        setError("Could not load your appointments.");
      } finally {
        setLoading(false);
      }
    };

    load();
  }, [user]);

  // === Statistics calculation ===

  const today = new Date();
  today.setHours(0, 0, 0, 0);

  const parseDate = (iso: string) => {
    const d = new Date(iso);
    d.setHours(0, 0, 0, 0);
    return d;
  };

  // Start/end of this week (Monday–Sunday)
  const startOfWeek = new Date(today);
  const day = today.getDay(); // 0 = Sunday, 1 = Monday, ...
  const diffToMonday = (day + 6) % 7; // make Monday the start
  startOfWeek.setDate(today.getDate() - diffToMonday);
  startOfWeek.setHours(0, 0, 0, 0);

  const endOfWeek = new Date(startOfWeek);
  endOfWeek.setDate(startOfWeek.getDate() + 6);
  endOfWeek.setHours(23, 59, 59, 999);

  // Number of different patients (clientId or clientName)
  const uniquePatients = new Set(
    appointments.map((a) => a.clientId || a.clientName || "")
  );
  if (uniquePatients.has("")) {
    uniquePatients.delete("");
  }
  const totalPatients = uniquePatients.size;

  // Appointments this week
  const appointmentsThisWeek = appointments.filter((a) => {
    const d = parseDate(a.date);
    return d >= startOfWeek && d <= endOfWeek;
  }).length;

  // "Pending" = future/booked appointments
  const pendingCount = appointments.filter((a) => {
    const d = parseDate(a.date);
    const isFutureOrToday = d >= today;
    return isFutureOrToday && a.status === "Booked";
  }).length;

  const cancelledCount = appointments.filter(
    (a) => a.status === "Cancelled"
  ).length;

  // Upcoming appointments list (for table)
  const upcomingAppointments = appointments
    .filter((a) => {
      const d = parseDate(a.date);
      const isFutureOrToday = d >= today;
      return isFutureOrToday && a.status !== "Cancelled";
    })
    .sort((a, b) => {
      const da = new Date(a.date).getTime();
      const db = new Date(b.date).getTime();
      if (da !== db) return da - db;

      const ta = (a.startTime ?? "").slice(0, 5);
      const tb = (b.startTime ?? "").slice(0, 5);
      return ta.localeCompare(tb);
    });

  // Recent completed appointments (uten notes)
  const recentCompleted = appointments
    .filter((a) => {
      const d = parseDate(a.date);
      return d <= today && a.status === "Completed";
    })
    .sort((a, b) => {
      const da = new Date(a.date).getTime();
      const db = new Date(b.date).getTime();
      return db - da; // nyeste først
    })
    .slice(0, 5);

  return (
    <Container className="py-4 personnel-page">
      {/* Welcome / topsection */}
      <div className="mb-4 d-flex flex-column flex-md-row justify-content-between align-items-md-center gap-3">
        <div>
          <h2 className="fw-bold mb-2">Welcome back, {displayName}!</h2>
          <p className="text-muted mb-0">
            Here's an overview of your schedule and patients
          </p>
        </div>

        {/* NY knapp for availability */}
        <div>
          <Link to="/availability/create" className="btn btn-primary">
            <i className="bi bi-calendar-plus me-2"></i>
            Add availability
          </Link>
        </div>
      </div>

      {/* Quick stats */}
      <Row className="g-3 mb-4" xs={1} sm={2} lg={4}>
        <Col>
          <div className="card border-1 border-dark bg-light bg-gradient h-100">
            <div className="card-body p-4 text-dark d-flex justify-content-between align-items-center">
              <div>
                <h2 className="fw-bold mb-1">{totalPatients}</h2>
                <p className="mb-0">Patients</p>
              </div>
              <i className="bi bi-people-fill fs-1 opacity-75"></i>
            </div>
          </div>
        </Col>

        <Col>
          <div className="card border-1 border-dark bg-light bg-gradient h-100">
            <div className="card-body p-4 text-dark d-flex justify-content-between align-items-center">
              <div>
                <h2 className="fw-bold mb-1">{appointmentsThisWeek}</h2>
                <p className="mb-0">This Week</p>
              </div>
              <i className="bi bi-calendar-check-fill fs-1 opacity-75"></i>
            </div>
          </div>
        </Col>

        <Col>
          <div className="card border-1 border-dark bg-light bg-gradient h-100">
            <div className="card-body p-4 text-dark d-flex justify-content-between align-items-center">
              <div>
                <h2 className="fw-bold mb-1">{pendingCount}</h2>
                <p className="mb-0">Pending</p>
              </div>
              <i className="bi bi-clock-fill fs-1 opacity-75"></i>
            </div>
          </div>
        </Col>

        <Col>
          <div className="card border-1 border-dark bg-light bg-gradient h-100">
            <div className="card-body p-4 text-dark d-flex justify-content-between align-items-center">
              <div>
                <h2 className="fw-bold mb-1">{cancelledCount}</h2>
                <p className="mb-0">Cancelled</p>
              </div>
              <i className="bi bi-x-circle-fill fs-1 opacity-75"></i>
            </div>
          </div>
        </Col>
      </Row>

      {/* Main content: Upcoming appointments + Recent activity */}
      <Row className="g-4 mb-4">
        <Col xs={12} lg={7}>
          <div className="card border bg-light h-100">
            <div className="card-body p-4">
              <h5 className="fw-bold mb-4 text-dark">Upcoming Appointments</h5>

              {error && (
                <Alert variant="danger" className="mb-3">
                  {error}
                </Alert>
              )}

              {loading ? (
                <div className="d-flex justify-content-center my-4">
                  <Spinner animation="border" role="status" />
                </div>
              ) : upcomingAppointments.length === 0 ? (
                <div className="text-center py-4">
                  <i className="bi bi-calendar-x display-4 text-muted mb-3 d-block"></i>
                  <p className="text-dark mb-0">
                    No upcoming appointments scheduled.
                  </p>
                </div>
              ) : (
                <div className="table-responsive rounded overflow-hidden border border-dark">
                  <table className="table table-hover bg-white mb-0">
                    <thead className="table-light">
                      <tr>
                        <th className="fw-bold text-dark py-3 px-4 border-bottom border-dark">
                          Patient Name
                        </th>
                        <th className="fw-bold text-dark py-3 px-4 border-bottom border-dark">
                          Task
                        </th>
                        <th className="fw-bold text-dark py-3 px-4 border-bottom border-dark">
                          Date
                        </th>
                        <th className="fw-bold text-dark py-3 px-4 border-bottom border-dark">
                          Time
                        </th>
                      </tr>
                    </thead>
                    <tbody>
                      {upcomingAppointments.map((a) => (
                        <tr key={a.id}>
                          <td className="py-3 px-4 text-dark border-0">
                            {a.clientName ?? "Unnamed patient"}
                          </td>
                          <td className="py-3 px-4 text-dark border-0">
                            {a.taskDescription}
                          </td>
                          <td className="py-3 px-4 text-dark border-0">
                            {new Date(a.date).toLocaleDateString("nb-NO", {
                              day: "2-digit",
                              month: "2-digit",
                              year: "2-digit",
                            })}
                          </td>
                          <td className="py-3 px-4 text-dark border-0">
                            {a.startTime?.slice(0, 5)}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          </div>
        </Col>

        <Col xs={12} lg={5}>
          <div className="card border bg-light h-100">
            <div className="card-body p-4">
              <h5 className="fw-bold mb-4 text-dark">Recent Activity</h5>

              {recentCompleted.length === 0 ? (
                <p className="text-muted mb-0">
                  No recently completed appointments.
                </p>
              ) : (
                <div className="vstack gap-3">
                  {recentCompleted.map((a) => (
                    <div key={a.id} className="bg-white border rounded p-3">
                      <div className="d-flex justify-content-between mb-1">
                        <span className="fw-semibold">
                          {a.clientName ?? "Unnamed patient"}
                        </span>
                        <span className="text-muted small">
                          {new Date(a.date).toLocaleDateString("nb-NO")}{" "}
                          {a.startTime?.slice(0, 5)}
                        </span>
                      </div>
                      <p className="mb-1 text-muted small">
                        {a.taskDescription ?? "Home care visit"}
                      </p>
                      <p className="mb-0 text-muted small">
                        Status: {a.status}
                      </p>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>
        </Col>
      </Row>
    </Container>
  );
};

export default PersonnelDashboard;
