import React, { useEffect, useState } from "react";
import { fetchAppointments, deleteAppointment } from "./AppointmentService";
import { Appointment } from "../../types/Appointment";
import AppointmentTable from "./AppointmentTable";

const AppointmentList: React.FC = () => {
  const [appointments, setAppointments] = useState<Appointment[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Hent alle avtaler ved mount
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

  const handleDelete = async (id: number) => {
    if (!window.confirm("Are you sure you want to delete this appointment?")) {
      return;
    }

    try {
      await deleteAppointment(id);
      setAppointments((prev) => prev.filter((a) => a.id !== id));
    } catch (err: any) {
      console.error("Error deleting appointment:", err);
      alert(err.message ?? "Could not delete appointment");
    }
  };

  if (loading) return <p>Loading appointments...</p>;
  if (error) return <p className="text-danger">{error}</p>;

  return (
    <div>
      <h2 className="fw-bold mb-3">Appointments</h2>
      {appointments.length === 0 ? (
        <p>No appointments found.</p>
      ) : (
        <AppointmentTable appointments={appointments} onDelete={handleDelete} />
      )}
    </div>
  );
};

export default AppointmentList;
