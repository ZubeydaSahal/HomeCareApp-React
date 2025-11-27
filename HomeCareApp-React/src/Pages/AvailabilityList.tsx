import React, { useState, useEffect } from "react";
import { Button, Form } from "react-bootstrap";
import AvailabilityTable from "./AvailabilityTable";   // evt "../availability/AvailabilityTable"
import AvailabilityGrid from "./AvailabilityGrid";     // evt "../availability/AvailabilityGrid"
import { Availability } from "../types/Availability";
import * as AvailabiliyService from "./AvailabilityService";


const API_URL = import.meta.env.VITE_API_URL;

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
      const data=await AvailabiliyService.fetchAvailabilities();
      const response = await fetch(`${API_URL}/api/availability/list`, {
        credentials: "include",
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data: Availability[] = await response.json();
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
  // set the view mode to local storage when availablable time is fetched
  useEffect(() => {
    const savedViewMode = localStorage.getItem("availabilityViewMode");
    console.log("Saved view mode from localStorage:", savedViewMode);//debugging line
    if (savedViewMode) {
      if(savedViewMode === 'grid') 
        setShowTable(false)
      console.log('show table', showTable);
    }
    fetchAvailabilities();
  }, []);

  // save the view mode to local storage when  something changes
  useEffect(() => {
    console.log('[save view state] Saving the view mode' , showTable ? 'table' : 'grid');
    localStorage.setItem("availabilityViewMode", showTable ? "table" : "grid");
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

  const handleAvailabilityDeleted= async (id: number) => {
    const confirmDelete = window.confirm(`Are you sure you want to delete this availability ${id}?`);
    if (!confirmDelete){
      try{
        await AvailabiliyService.deleteAvailability(id);
        const response = await fetch(`${API_URL}/api/availability/delete/${id}`, {
          method: "DELETE",
        });
        setAvailabilities(prevAvailabilities => prevAvailabilities.filter(a => a.id !== id));
        console.log('Availability deleted successfully:', id);
      } catch (error) {
        console.error('Error deleting availability:', error);
        setError("Failed to delete availability.");
      }
    }
   

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

       {error && <p style={{ color: 'red' }}>{error}</p>}
      {showTable
        ? <AvailabilityTable availabilities={filteredAvailabilities} apiUrl={API_URL} onAvailabilityDeleted={handleAvailabilityDeleted} />
        : <AvailabilityGrid availabilities={filteredAvailabilities} apiUrl={API_URL} onAvailabilityDeleted={handleAvailabilityDeleted} />}
      <Button href='/availabilitycreate' className="btn btn-secondary mt-3">Add New Availability</Button> 
    </div>
  );
};



export default AvailabilityList;
