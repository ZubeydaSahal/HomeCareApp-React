// src/Pages/appointments/AppointmentCreatePage.tsx
import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import AppointmentForm from "./AppointmentForm";
import { createAppointment } from "./AppointmentService";
import { AppointmentCreatePayload } from "../../types/Appointment";
import { fetchAvailabilities } from "../AvailabilityService";  

interface Option {
  value: number;
  label: string;
}

const AppointmentCreatePage: React.FC = () => {
  const navigate = useNavigate();
  const [availabilityOptions, setAvailabilityOptions] = useState<Option[]>([]);
  const [clientOptions] = useState<Option[]>([]);
  const isPersonnel = false; // du er Patient her

  useEffect(() => {
    const load = async () => {
      const avail = await fetchAvailabilities();  // GET /api/availability/list

      // ledige slots = ingen appointment tilknyttet
      const free = avail.filter((a: any) => !a.appointmentId);

      setAvailabilityOptions(
        free.map((a: any) => ({
          value: a.id, // ⬅️ dette må være Availability.Id
          label: `${a.personnelName ?? "Unknown"} - ${a.date.substring(0, 10)} ${a.startTime.substring(0,5)}-${a.endTime.substring(0,5)}`,
        }))
      );
    };
    load();
  }, []);

  const handleSubmit = async (payload: AppointmentCreatePayload) => {
    console.log("Create payload:", payload); // se i konsollen
    await createAppointment(payload);
    navigate("/appointments");
  };

  return (
    <AppointmentForm
      isPersonnel={isPersonnel}
      clientOptions={clientOptions}
      availabilityOptions={availabilityOptions}
      onSubmit={handleSubmit}
    />
  );
};

export default AppointmentCreatePage;
