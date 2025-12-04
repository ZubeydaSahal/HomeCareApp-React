import React, { useState, useEffect } from "react";
import { Button, Form, Alert, Spinner } from "react-bootstrap";
import { Link } from "react-router-dom";
import { Availability } from "../../types/Availability";
import * as AvailabilityService from "./AvailabilityService";
import AvailabilityTable from "./AvailabilityTable";

const AvailabilityList: React.FC = () => {
  const [availabilities, setAvailabilities] = useState<Availability[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [searchQuery, setSearchQuery] = useState<string>("");

  const fetchAvailabilities = async () => {
    setLoading(true);
    setError(null);

    try {
      const data = await AvailabilityService.fetchAvailabilities() as Availability[];
      setAvailabilities(data);
      console.log("Fetched availabilities:", data);
    } catch (error: unknown) {
      if (error instanceof Error) {
        console.error("Error fetching availabilities:", error.message);
      } else {
        console.error("Unknown error", error);
      }
      setError("Kunne ikke hente ledige tider.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchAvailabilities();
  }, []);

  const filteredAvailabilities = availabilities.filter((a) => {
    if (!searchQuery) return true;

    const q = searchQuery.toLowerCase();
    return (
      (a.personnelName && a.personnelName.toLowerCase().includes(q)) ||
      (a.notes && a.notes.toLowerCase().includes(q)) ||
      (a.date && a.date.toString().toLowerCase().includes(q))
    );
  });

  const handleAvailabilityDeleted = async (id: number) => {
    try {
      await AvailabilityService.deleteAvailability(id);
      setAvailabilities((prev) => prev.filter((a) => a.id !== id));
      console.log("Availability deleted successfully:", id);
    } catch (error) {
      console.error("Error deleting availability:", error);
      setError("Failed to delete availability.");
    }
  };

  return (
    <div className="personnel-page" style={{ padding: "1rem" }}>
      <div className="mb-4">
        <p className="text-dark mb-0">
          Manage your availability slots for patient appointments.
        </p>
      </div>

      <div className="mb-4 d-flex justify-content-between align-items-center">
        <h1 className="h4 mb-0">My Calendar</h1>
        <Link to="/availability/create" className="btn btn-primary">
          + Add Availability Slot
        </Link>
      </div>

      <Form.Group className="mb-3">
        <Form.Control
          type="text"
          placeholder="Search by personnel, date or notes"
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
        />
      </Form.Group>

      {error && (
        <Alert variant="danger" className="mb-3">
          {error}
        </Alert>
      )}

      {loading ? (
        <div className="d-flex justify-content-center my-4">
          <Spinner animation="border" role="status" />
        </div>
      ) : (
        <AvailabilityTable
          availabilities={filteredAvailabilities}
          onAvailabilityDeleted={handleAvailabilityDeleted}
        />
      )}
    </div>
  );
};

export default AvailabilityList;
