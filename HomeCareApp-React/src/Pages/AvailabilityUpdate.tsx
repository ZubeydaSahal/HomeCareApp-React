// src/availability/AvailabilityUpdatePage.tsx
import React, { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import AvailabilityForm from "./AvailabilityForm";
import { Availability } from "../types/Availability";
import * as AvailabiliyService from "./AvailabilityService";


const API_URL = import.meta.env.VITE_API_URL;

const AvailabilityUpdate: React.FC = () => {
  const { availabilityId } = useParams<{ availabilityId: string }>(); // gets id from URL
  const navigate = useNavigate(); // create navigate function

  const [availability, setAvailability] = useState<Availability | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchAvailability = async () => {
      try {
        const response = await fetch(
          `${API_URL}/api/availability/${availabilityId}`, // GET én availability
          { credentials: "include" }
        );

        if (!response.ok) {
          throw new Error("Network response was not ok");
        }

        const data: Availability = await response.json();
        setAvailability(data);
      } catch (err) {
        setError("Failed to fetch availability");
        console.error("There was a problem with the fetch operation:", err);
      } finally {
        setLoading(false);
      }
    };

    if (availabilityId) {
      fetchAvailability();
    }
  }, [availabilityId]);

  const handleAvailabilityUpdated = async (updated: Availability) => {
    try {
      const data=await AvailabiliyService.updateAvailability(updated.id, updated);
      const response = await fetch(
        `${API_URL}/api/availability/update/${updated.id}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
          },
          credentials: "include",
          body: JSON.stringify(updated),
        }
      );

      if (!response.ok) {
        throw new Error("Network response was not ok");
      }

      console.log("Availability updated successfully");
      navigate("/availability"); // tilbake til lista
    } catch (error) {
      console.error("There was a problem with the fetch operation:", error);
    }
  };

  if (loading) return <p>Loading...</p>;
  if (error) return <p>{error}</p>;
  if (!availability) return <p>No availability found</p>;

  return (
    <div>
      <h2>Update Availability</h2>
      <AvailabilityForm
        onAvailabilityChanged={handleAvailabilityUpdated}
        availabilityId={availability.id}
        initialData={availability}
        isUpdate={true}
      />
    </div>
  );
};

export default AvailabilityUpdate;
