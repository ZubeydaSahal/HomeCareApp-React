import React from "react";
import { useNavigate } from "react-router-dom";
import AvailabilityForm from "./AvailabilityForm";
import { Availability } from "../types/Availability";
import * as AvailabiliyService from "./AvailabilityService";

const AvailabilityCreate: React.FC = () => {
  const navigate = useNavigate();
//called when a new availability is created
  const handleAvailabilityCreated = async (availability: Availability) => {
    try {
      await AvailabiliyService.createAvailability(availability);
      navigate("/availability");
    } catch (error) {
      console.error("There was a problem with the fetch operation:", error);
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
