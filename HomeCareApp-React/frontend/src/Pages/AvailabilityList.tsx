import React, { useState, useEffect } from "react";
import { Button, Form } from "react-bootstrap";
import AvailabilityTable from "./AvailabilityTable";
import AvailabilityGrid from "./AvailabilityGrid";
import { Availability } from "../types/Availability";
import * as AvailabilityService from "./AvailabilityService";

const AvailabilityList: React.FC = () => {
  const [availabilities, setAvailabilities] = useState<Availability[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [showTable, setShowTable] = useState<boolean>(true);
  const [searchQuery, setSearchQuery] = useState<string>("");

  const toggleTableOrGrid = () =>
    setShowTable((prevShowTable) => !prevShowTable);

  const fetchAvailabilities = async () => {
    setLoading(true);
    setError(null);

    try {
      // Bruk kun service-laget
      const data = await AvailabilityService.fetchAvailabilities();
      setAvailabilities(data);
      console.log("Availabilities from API:", data);
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

  // Hent view-mode og data ved mount
  useEffect(() => {
    const savedViewMode = localStorage.getItem("availabilityViewMode");
    console.log("Saved view mode from localStorage:", savedViewMode);

    if (savedViewMode === "grid") {
      setShowTable(false);
    }

    fetchAvailabilities();
  }, []);

  // Lagre view-mode når det endres
  useEffect(() => {
    console.log(
      "[save view state] Saving the view mode",
      showTable ? "table" : "grid"
    );
    localStorage.setItem(
      "availabilityViewMode",
      showTable ? "table" : "grid"
    );
  }, [showTable]);

  const filteredAvailabilities = availabilities.filter((a) => {
    if (!searchQuery) return true;
    const q = searchQuery.toLowerCase();

    return (
      (a.personnelId && a.personnelId.toLowerCase().includes(q)) ||
      (a.notes && a.notes.toLowerCase().includes(q)) ||
      (a.date && a.date.toString().toLowerCase().includes(q))
    );
  });

  const handleAvailabilityDeleted = async (id: number) => {
    const confirmDelete = window.confirm(
      `Are you sure you want to delete this availability ${id}?`
    );
    if (!confirmDelete) return;

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
    <div style={{ padding: "1rem" }}>
      <h1>Available days (Availability)</h1>

      <div className="mb-3 d-flex gap-2">
        <Button
          onClick={fetchAvailabilities}
          className="mb-2 me-2"
          disabled={loading}
        >
          {loading ? "Laster..." : "Oppdater liste"}
        </Button>

        <Button
          variant="secondary"
          onClick={toggleTableOrGrid}
          className="mb-2"
        >
          {showTable ? "Display Grid" : "Display Table"}
        </Button>
      </div>

      <Form.Group className="mb-3">
        <Form.Control
          type="text"
          placeholder="Search by personnel, date or notes"
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
        />
      </Form.Group>

      {error && <p style={{ color: "red" }}>{error}</p>}

      {showTable ? (
        <AvailabilityTable
          availabilities={filteredAvailabilities}
          onAvailabilityDeleted={handleAvailabilityDeleted}
        />
      ) : (
        <AvailabilityGrid
          availabilities={filteredAvailabilities}
          onAvailabilityDeleted={handleAvailabilityDeleted}
        />
      )}

      <Button
        href="/availability/create"
        className="btn btn-secondary mt-3"
      >
        Add New Availability
      </Button>
    </div>
  );
};

export default AvailabilityList;
