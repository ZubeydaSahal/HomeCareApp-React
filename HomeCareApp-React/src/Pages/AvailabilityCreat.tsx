import React from "react";
import { useNavigate } from "react-router-dom";
import AvailabilityForm from "./AvailabilityForm";
import { Availability } from "../types/Availability";
import * as AvailabiliyService from "./AvailabilityService";



const API_URL = import.meta.env.VITE_API_URL;

const AvailabilityCreate: React.FC = () => {
  const navigate = useNavigate();

  const handleAvailabilityCreated = async (availability: Availability) => {
    try {
      const data=await AvailabiliyService.createAvailability(availability);
      const response = await fetch(`${API_URL}/api/availability/create`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(availability),
        credentials: "include", // hvis du bruker cookie-basert auth
      });

      if (!response.ok) {
        throw new Error("Network response was not ok");
      }

      const data = await response.json();
      console.log("Availability created successfully:", data);

      // Etter vellykket opprettelse → tilbake til liste
      navigate("/availability");
    } catch (error) {
      console.error(
        "There was a problem with the fetch operation:",
        error
      );
    }
  };

  return (
    <div>
      <h2>Create New Availability</h2>
      <AvailabilityForm onAvailabilityChanged={handleAvailabilityCreated} />
    </div>
  );
};

export default AvailabilityCreate;
