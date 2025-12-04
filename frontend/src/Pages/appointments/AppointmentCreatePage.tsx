import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import AppointmentForm from "./AppointmentForm";
import { createAppointment } from "./AppointmentService";
import { AppointmentCreatePayload } from "../../types/Appointment";
import { fetchAvailabilities } from "../availability/AvailabilityService";
import { Availability } from "../../types/Availability";


interface Option {
  value: number;
  label: string;
}

const AppointmentCreatePage: React.FC = () => {
  const navigate = useNavigate();
  const [availabilityOptions, setAvailabilityOptions] = useState<Option[]>([]);
  const [clientOptions] = useState<Option[]>([]);
  const isPersonnel = false;

  useEffect(() => {
    const load = async () => {
      const avail = await fetchAvailabilities() as Availability[];

      //  available slots = no appointment attached
      const free = avail.filter((a: any) => !a.appointmentId) ;

      setAvailabilityOptions(
        free.map((a: any) => ({
          value: a.id,
          label: `${a.personnelName ?? "Unknown"} - ${a.date.substring(0, 10)} ${a.startTime.substring(0, 5)}-${a.endTime.substring(0, 5)}`,
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
