// src/appointments/AppointmentCreatePage.tsx
import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import AppointmentForm from "./AppointmentFormt";
import { createAppointment } from "./AppointmentService";
import { AppointmentCreatePayload } from "../../types/Appointment";
import * as AvailabilityService from "./AppointmentService";

interface Option {
  value: string | number;
  label: string;
}

const AppointmentCreatePage: React.FC = () => {
  const navigate = useNavigate();
  const [availabilityOptions, setAvailabilityOptions] = useState<Option[]>([]);
  const [clientOptions] = useState<Option[]>([]); // TODO: hent pasienter fra API
  const isPersonnel = true; // TODO: bestem basert på innlogget bruker

  useEffect(() => {
    const load = async () => {
      const avail = await AvailabilityService.fetchAvailabilities();
      const free = avail.filter((a: any) => !a.isBooked);
      setAvailabilityOptions(
        free.map((a: any) => ({
          value: a.id,
          label: `${a.personnelName ?? "Unknown"} - ${a.date.substring(0, 10)} ${a.startTime.substring(0,5)}-${a.endTime.substring(0,5)}`,
        }))
      );
    };
    load();
  }, []);

  const handleSubmit = async (payload: AppointmentCreatePayload) => {
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
