import React from "react";
import { Table, Button } from "react-bootstrap";
import { Link } from "react-router-dom";
import { Availability } from "../../types/Availability";

interface Props {
  availabilities: Availability[];
  onAvailabilityDeleted?: (id: number) => void;
}

const AvailabilityTable: React.FC<Props> = ({
  availabilities,
  onAvailabilityDeleted,
}) => {
  const sorted = [...availabilities].sort((a, b) => {
    const da = new Date(a.date).getTime();
    const db = new Date(b.date).getTime();
    if (da !== db) return da - db;

    const ta = a.startTime ?? "";
    const tb = b.startTime ?? "";
    return ta.localeCompare(tb);
  });

  const formatDate = (iso: string) =>
    new Date(iso).toLocaleDateString("nb-NO", {
      day: "2-digit",
      month: "2-digit",
      year: "2-digit",
    });

  const formatTime = (t?: string) => (t ? t.slice(0, 5) : "");

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
              <th className="fw-bold text-dark py-3 px-4 border-bottom border-dark text-end">
                Actions
              </th>
            </tr>
          </thead>
          <tbody>
            {sorted.map((a) => {
              const isBooked = a.isBooked ?? a.appointmentId != null;

              return (
                <tr key={a.id}>
                  <td className="py-3 px-4 text-dark">{formatDate(a.date)}</td>
                  <td className="py-3 px-4 text-dark">
                    {formatTime(a.startTime)}
                  </td>
                  <td className="py-3 px-4 text-dark">
                    {formatTime(a.endTime)}
                  </td>
                  <td className="py-3 px-4 text-dark">
                    {isBooked ? (
                      <span className="badge bg-success">Booked</span>
                    ) : (
                      <span className="badge bg-secondary">Available</span>
                    )}
                  </td>
                  <td className="py-3 px-4 text-dark text-end">
                    {!isBooked ? (
                      <>
                        <Link
                          to={`/availability/edit/${a.id}`}
                          className="btn btn-outline-secondary btn-sm me-2"
                        >
                          Edit
                        </Link>
                        <Button
                          variant="outline-danger"
                          size="sm"
                          onClick={() => a.id && onAvailabilityDeleted?.(a.id)}
                        >
                          Delete
                        </Button>
                      </>
                    ) : (
                      <Button variant="outline-secondary" size="sm" disabled>
                        Booked
                      </Button>
                    )}
                  </td>
                </tr>
              );
            })}

            {sorted.length === 0 && (
              <tr>
                <td colSpan={5} className="py-4 px-4 text-center text-muted">
                  No availability slots found.
                </td>
              </tr>
            )}
          </tbody>
        </Table>
      </div>
    </div>
  );
};

export default AvailabilityTable;
