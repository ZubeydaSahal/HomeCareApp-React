import React, { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import AppointmentForm from "./AppointmentForm";
import { getAppointment, updateAppointment } from "./AppointmentService";
import { Appointment, AppointmentCreatePayload } from "../../types/Appointment";
import { Availability } from "../../types/Availability";


import * as AvailabilityService from "../availability/AvailabilityService";

interface Option {
  value: string | number;
  label: string;
}

const AppointmentUpdatePage: React.FC = () => {
  const { appointmentId } = useParams<{ appointmentId: string }>();
  const navigate = useNavigate();

  const [availabilityOptions, setAvailabilityOptions] = useState<Option[]>([]);
  const [clientOptions] = useState<Option[]>([]); // can be filled later with real patients
  const [initialValues, setInitialValues] =
    useState<AppointmentCreatePayload | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // TODO: connect to actual role from AuthContext later
  const isPersonnel = true;

  useEffect(() => {
    const load = async () => {
      try {
        if (!appointmentId) {
          setError("Missing appointment id in URL.");
          return;
        }

        const id = Number(appointmentId);

        // 1) Fetch the appointment itself
        const appt: Appointment = await getAppointment(id);

        // 2) Fetch all availabilities
        const avail = await AvailabilityService.fetchAvailabilities() as Availability[];

        // 3) Allow:
        //    - slots that are not booked
        //    - the slot that this appointment already uses
        const options: Option[] = avail
          .filter((a: any) => !a.appointmentId || a.appointmentId === appt.id)
          .map((a: any) => ({
            value: a.id,
            label: `${a.personnelName ?? "Unknown"} - ${a.date.substring(0, 10)} ${a.startTime.substring(0, 5)}-${a.endTime.substring(0, 5)}`,
          }));

        setAvailabilityOptions(options);

        // 4) Set initial values for form (AppointmentCreatePayload)
        setInitialValues({
          availabilityId: appt.availabilityId,
          clientId: appt.clientId ?? "",
          taskDescription: appt.taskDescription ?? "",
          // backend typically sends "HH:mm:ss" → cut to "HH:mm"
          startTime: appt.startTime.substring(0, 5),
          endTime: appt.endTime.substring(0, 5),
          status: appt.status,
        });
      } catch (err: any) {
        console.error(err);
        setError(err.message ?? "Failed to load appointment.");
      } finally {
        setLoading(false);
      }
    };

    load();
  }, [appointmentId]);

  const handleSubmit = async (payload: AppointmentCreatePayload) => {
    if (!appointmentId) return;
    await updateAppointment(Number(appointmentId), payload);
    navigate("/appointments");
  };

  if (loading) return <p>Loading appointment...</p>;
  if (error) return <p className="text-danger">{error}</p>;
  if (!initialValues) return <p>Appointment not found.</p>;

  return (
    <AppointmentForm
      isPersonnel={isPersonnel}
      clientOptions={clientOptions}
      availabilityOptions={availabilityOptions}
      initialValues={initialValues}
      onSubmit={handleSubmit}
    />
  );
};

export default AppointmentUpdatePage;
