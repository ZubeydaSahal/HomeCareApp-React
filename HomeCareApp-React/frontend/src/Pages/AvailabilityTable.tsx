// src/availability/AvailabilityTable.tsx (eller tilsvarende path)
import React, { useState } from "react";
import { Table, Button } from "react-bootstrap";
import { Availability } from "../types/Availability";
import {Link} from "react-router-dom";

interface AvailabilityTableProps {
  availabilities: Availability[];
  apiUrl?: string; // brukes ikke nå, men fint å ha hvis du vil senere
  onAvailabilityDeleted?: (id: number) => void;
}

const AvailabilityTable: React.FC<AvailabilityTableProps> = ({
  availabilities,
  apiUrl,
  onAvailabilityDeleted
}) => {
  const [showNotes, setShowNotes] = useState<boolean>(true);
  const [showPersonnel, setShowPersonnel] = useState<boolean>(true);

  const toggleNotes = () => setShowNotes((prev) => !prev);
  const togglePersonnel = () => setShowPersonnel((prev) => !prev);

  return (
    <div>
      <Button
        onClick={togglePersonnel}
        className="btn btn-secondary mb-3 me-2"
      >
        {showPersonnel ? "Hide Personnel" : "Show Personnel"}
      </Button>
      <Button onClick={toggleNotes} className="btn btn-secondary mb-3">
        {showNotes ? "Hide Notes" : "Show Notes"}
      </Button>

      <Table striped bordered hover>
        <thead>
          <tr>
            <th>Id</th>
            {showPersonnel && <th>Personnel</th>}
            <th>Date</th>
            <th>Start</th>
            <th>End</th>
            {showNotes && <th>Notes</th>}
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {availabilities.map((a) => (
            <tr key={a.id}>
              <td>{a.id}</td>
              {showPersonnel && <td>{a.personnelId}</td>}
              <td>{a.date}</td>
              <td>{a.startTime}</td>
              <td>{a.endTime}</td>
              {showNotes && <td>{a.notes}</td>}

              <td className="text-center">
                <Link to={`/availability/edit/${a.id}`} className="btn btn-primary btn-sm">
                  Edit
                </Link>
              </td>
              <td>
                <Link to={`/availability/delete/${a.id}`} className="btn btn-danger btn-sm">
                  Delete
                </Link>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>
    </div>
  );
};

export default AvailabilityTable;
