import React from "react";
import { Table, Button } from "react-bootstrap";
import { Link } from "react-router-dom";
import { Appointment } from "../../types/Appointment";

interface AppointmentTableProps {
  appointments: Appointment[];
  onDelete?: (id: number) => void;
}

const formatDate = (dateStr: string) =>
  new Date(dateStr).toLocaleDateString("no-NO");

const formatTime = (timeStr: string) => timeStr.substring(0, 5);

const AppointmentTable: React.FC<AppointmentTableProps> = ({
  appointments,
  onDelete,
}) => {
  const sorted = [...appointments].sort((a, b) => {
    const da = new Date(a.date);
    const db = new Date(b.date);
    if (da.getTime() !== db.getTime()) return da.getTime() - db.getTime();
    return a.startTime.localeCompare(b.startTime);
  });

  return (
    <div className="card border-1 border-dark bg-white overflow-hidden">
      <div className="card-body p-0">
        <Table hover className="mb-0 bg-white rounded">
          <thead className="table-light">
            <tr>
              <th className="fw-bold text-dark py-3 px-4 border-bottom border-dark">
                Date
              </th>
              <th className="fw-bold text-dark py-3 px-4 border-bottom border-dark">
                Start
              </th>
              <th className="fw-bold text-dark py-3 px-4 border-bottom border-dark">
                End
              </th>
              <th className="fw-bold text-dark py-3 px-4 border-bottom border-dark">
                Status
              </th>
              <th className="fw-bold text-dark py-3 px-4 border-bottom border-dark">
                Patient
              </th>
              <th className="fw-bold text-dark py-3 px-4 border-bottom border-dark">
                Personnel
              </th>
              <th className="fw-bold text-dark py-3 px-4 border-bottom border-dark text-end">
                Actions
              </th>
            </tr>
          </thead>
          <tbody>
            {sorted.map((a) => (
              <tr key={a.id}>
                <td className="py-3 px-4 text-dark">{formatDate(a.date)}</td>
                <td className="py-3 px-4 text-dark">
                  {formatTime(a.startTime)}
                </td>
                <td className="py-3 px-4 text-dark">{formatTime(a.endTime)}</td>
                <td className="py-3 px-4 text-dark">
                  {a.status === "Booked" && (
                    <span className="badge bg-secondary">Booked</span>
                  )}
                  {a.status === "Completed" && (
                    <span className="badge bg-success">Completed</span>
                  )}
                  {a.status === "Cancelled" && (
                    <span className="badge bg-danger">Cancelled</span>
                  )}
                </td>

                {/* Patient */}
                <td className="py-3 px-4 text-dark">
                  {a.clientName ?? "Unknown"}
                </td>

                {/* Personnel */}
                <td className="py-3 px-4 text-dark">
                  {a.personnelName ?? "Unknown"}
                </td>

                {/* Actions */}
                <td className="py-3 px-4 text-dark text-end">
                  <Link
                    to={`/appointments/edit/${a.id}`}
                    className="btn btn-outline-secondary btn-sm me-2"
                  >
                    Edit
                  </Link>

                  {onDelete ? (
                    <Button
                      size="sm"
                      variant="outline-danger"
                      onClick={() => onDelete(a.id)}
                    >
                      Delete
                    </Button>
                  ) : (
                    <Button size="sm" variant="outline-secondary" disabled>
                      Delete
                    </Button>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </Table>
      </div>
    </div>
  );
};

export default AppointmentTable;
