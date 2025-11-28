// src/appointments/AppointmentForm.tsx
import React, { useState } from "react";
import { Form, Button } from "react-bootstrap";
import { AppointmentCreatePayload } from "../../types/Appointment";

interface Option {
  value: string | number;
  label: string;
}

interface AppointmentFormProps {
  isPersonnel: boolean;
  clientOptions: Option[];
  availabilityOptions: Option[];
  initialValues?: Partial<AppointmentCreatePayload>;
  onSubmit: (payload: AppointmentCreatePayload) => void | Promise<void>;
}

const AppointmentForm: React.FC<AppointmentFormProps> = ({
  isPersonnel,
  clientOptions,
  availabilityOptions,
  initialValues,
  onSubmit,
}) => {
  const [availabilityId, setAvailabilityId] = useState<number>(
    initialValues?.availabilityId ?? (availabilityOptions[0]?.value as number) ?? 0
  );
  const [clientId, setClientId] = useState<string>(initialValues?.clientId ?? "");
  const [taskDescription, setTaskDescription] = useState<string>(
    initialValues?.taskDescription ?? ""
  );
  const [startTime, setStartTime] = useState<string>(
    initialValues?.startTime ?? ""
  );
  const [endTime, setEndTime] = useState<string>(
    initialValues?.endTime ?? ""
  );
  const [status, setStatus] = useState<string>(
    initialValues?.status ?? "Booked"
  );
  const [submitting, setSubmitting] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);

    const payload: AppointmentCreatePayload = {
      availabilityId,
      clientId: clientId || undefined,
      taskDescription,
      startTime, // "HH:mm"
      endTime,
      status,
    };

    try {
      await onSubmit(payload);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className={isPersonnel ? "personnel-page" : ""}>
      <h2 className="fw-bold mb-3">New Appointment</h2>
      <hr />

      <div className="row">
        <div className="col-md-6">
          <Form onSubmit={handleSubmit}>
            {/* Client (kun for personnel/admin) */}
            {isPersonnel && (
              <Form.Group className="mb-3">
                <Form.Label>Client</Form.Label>
                <Form.Select
                  value={clientId}
                  onChange={(e) => setClientId(e.target.value)}
                >
                  <option value="">-- Select client --</option>
                  {clientOptions.map((c) => (
                    <option key={c.value} value={c.value}>
                      {c.label}
                    </option>
                  ))}
                </Form.Select>
              </Form.Group>
            )}

            {/* Availability */}
            <Form.Group className="mb-3">
              <Form.Label>Available Day/Slot</Form.Label>
              <Form.Select
                value={availabilityId}
                onChange={(e) => setAvailabilityId(Number(e.target.value))}
              >
                {availabilityOptions.map((opt) => (
                  <option key={opt.value} value={opt.value}>
                    {opt.label}
                  </option>
                ))}
              </Form.Select>
            </Form.Group>

            {/* TaskDescription */}
            <Form.Group className="mb-3">
              <Form.Label>Task(s)</Form.Label>
              <Form.Control
                type="text"
                placeholder="e.g., medication reminder"
                value={taskDescription}
                onChange={(e) => setTaskDescription(e.target.value)}
              />
            </Form.Group>

            {/* StartTime */}
            <Form.Group className="mb-3">
              <Form.Label>Start</Form.Label>
              <Form.Control
                type="time"
                value={startTime}
                onChange={(e) => setStartTime(e.target.value)}
              />
            </Form.Group>

            {/* EndTime */}
            <Form.Group className="mb-3">
              <Form.Label>End</Form.Label>
              <Form.Control
                type="time"
                value={endTime}
                onChange={(e) => setEndTime(e.target.value)}
              />
            </Form.Group>

            {/* Status */}
            <Form.Group className="mb-3">
              <Form.Label>Status</Form.Label>
              <Form.Select
                value={status}
                onChange={(e) => setStatus(e.target.value)}
              >
                <option value="Booked">Booked</option>
                <option value="Completed">Completed</option>
                <option value="Cancelled">Cancelled</option>
              </Form.Select>
            </Form.Group>

            <Button type="submit" variant="primary" disabled={submitting}>
              {submitting ? "Saving..." : "Create"}
            </Button>
          </Form>
        </div>
      </div>
    </div>
  );
};

export default AppointmentForm;
