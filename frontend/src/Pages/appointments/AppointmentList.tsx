import React, { useEffect, useState } from "react";
import { fetchAppointments, deleteAppointment } from "./AppointmentService";
import { Appointment } from "../../types/Appointment";
import AppointmentTable from "./AppointmentTable";
import {
  Modal,
  Button,
  Alert,
  Spinner,
  Container,
} from "react-bootstrap";

const AppointmentList: React.FC = () => {
  const [appointments, setAppointments] = useState<Appointment[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // state for deleting
  const [showConfirm, setShowConfirm] = useState(false);
  const [selectedAppt, setSelectedAppt] = useState<Appointment | null>(null);
  const [deleting, setDeleting] = useState(false);

  // Fetch all appointments
  useEffect(() => {
    const load = async () => {
      try {
        const data = await fetchAppointments();
        setAppointments(data);
      } catch (err: any) {
        console.error("Error fetching appointments:", err);
        setError(err.message ?? "Could not fetch appointments");
      } finally {
        setLoading(false);
      }
    };
    load();
  }, []);

  // now user clicks "Delete" in the table
  const handleRequestDelete = (id: number) => {
    const appt = appointments.find((a) => a.id === id) || null;
    setSelectedAppt(appt);
    setShowConfirm(true);
  };

  const handleConfirmClose = () => {
    setShowConfirm(false);
    setSelectedAppt(null);
  };

  const handleConfirmDelete = async () => {
    if (!selectedAppt) return;

    try {
      setDeleting(true);
      await deleteAppointment(selectedAppt.id);
      setAppointments((prev) => prev.filter((a) => a.id !== selectedAppt.id));
      handleConfirmClose();
    } catch (err: any) {
      console.error("Error deleting appointment:", err);
      setError(err.message ?? "Could not delete appointment");
    } finally {
      setDeleting(false);
    }
  };

  if (loading)
    return (
      <Container className="py-4">
        <div className="d-flex align-items-center gap-2">
          <Spinner animation="border" role="status" />
          <span>Loading appointments...</span>
        </div>
      </Container>
    );

  return (
    <Container className="py-4">
      <h2 className="fw-bold mb-3">Appointments</h2>

      {error && (
        <Alert variant="danger" onClose={() => setError(null)} dismissible>
          {error}
        </Alert>
      )}

      {appointments.length === 0 ? (
        <p>No appointments found.</p>
      ) : (
        // Gjør tabellen responsiv på små skjermer
        <div className="table-responsive">
          <AppointmentTable
            appointments={appointments}
            onDelete={handleRequestDelete}
          />
        </div>
      )}

      {/* Confirmation modal */}
      <Modal show={showConfirm} onHide={handleConfirmClose} centered>
        <Modal.Header closeButton>
          <Modal.Title>Delete appointment</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          {selectedAppt ? (
            <>
              <p>Are you sure you want to delete this appointment?</p>
              <ul>
                <li>
                  <strong>Task:</strong>{" "}
                  {selectedAppt.taskDescription ?? "Home care visit"}
                </li>
                <li>
                  <strong>Date:</strong>{" "}
                  {new Date(selectedAppt.date).toLocaleDateString("nb-NO")}
                </li>
                <li>
                  <strong>Time:</strong>{" "}
                  {selectedAppt.startTime.substring(0, 5)} –{" "}
                  {selectedAppt.endTime.substring(0, 5)}
                </li>
              </ul>
              <p className="text-danger mb-0">This action cannot be undone.</p>
            </>
          ) : (
            <p>No appointment selected.</p>
          )}
        </Modal.Body>
        <Modal.Footer>
          <Button
            variant="secondary"
            onClick={handleConfirmClose}
            disabled={deleting}
          >
            Cancel
          </Button>
          <Button
            variant="danger"
            onClick={handleConfirmDelete}
            disabled={deleting || !selectedAppt}
          >
            {deleting ? "Deleting..." : "Delete"}
          </Button>
        </Modal.Footer>
      </Modal>
    </Container>
  );
};

export default AppointmentList;
