import React, { useState, useEffect } from "react";
import { Form, Button, Container, Row, Col } from "react-bootstrap";
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
  availabilityOptions,
  initialValues,
  onSubmit,
}) => {
  const [availabilityId, setAvailabilityId] = useState<number>(
    initialValues?.availabilityId ?? 0
  );
  const [clientId, setClientId] = useState<string>(
    initialValues?.clientId ?? ""
  );
  const [taskDescription, setTaskDescription] = useState<string>(
    initialValues?.taskDescription ?? ""
  );
  const [startTime, setStartTime] = useState<string>(
    initialValues?.startTime ?? ""
  );
  const [endTime, setEndTime] = useState<string>(initialValues?.endTime ?? "");
  const [status, setStatus] = useState<string>(
    initialValues?.status ?? "Booked"
  );
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    if (
      !initialValues?.availabilityId &&
      availabilityOptions.length > 0 &&
      availabilityId === 0
    ) {
      setAvailabilityId(Number(availabilityOptions[0].value));
    }
  }, [availabilityOptions, initialValues, availabilityId]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);

    if (!availabilityId) {
      alert("You must choose an available time slot.");
      setSubmitting(false);
      return;
    }

    const payload: AppointmentCreatePayload = {
      availabilityId,
      clientId: clientId || undefined,
      taskDescription,
      startTime,
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
    <Container className={`py-4 ${isPersonnel ? "personnel-page" : ""}`}>
      <Row className="justify-content-center">
        <Col xs={12} md={8} lg={6}>
          <h2 className="fw-bold mb-3">New Appointment</h2>
          <hr />

          <Form onSubmit={handleSubmit}>
            <Form.Group className="mb-3">
              <Form.Label>Available Day/Slot</Form.Label>
              <Form.Select
                value={availabilityId || ""}
                onChange={(e) => setAvailabilityId(Number(e.target.value))}
              >
                <option value="">-- Select slot --</option>
                {availabilityOptions.map((opt) => (
                  <option key={opt.value} value={opt.value}>
                    {opt.label}
                  </option>
                ))}
              </Form.Select>
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Task(s)</Form.Label>
              <Form.Control
                type="text"
                placeholder="e.g., medication reminder"
                value={taskDescription}
                onChange={(e) => setTaskDescription(e.target.value)}
              />
            </Form.Group>

            <Row className="g-3">
              <Col xs={12} md={6}>
                <Form.Group className="mb-3">
                  <Form.Label>Start</Form.Label>
                  <Form.Control
                    type="time"
                    value={startTime}
                    onChange={(e) => setStartTime(e.target.value)}
                  />
                </Form.Group>
              </Col>
              <Col xs={12} md={6}>
                <Form.Group className="mb-3">
                  <Form.Label>End</Form.Label>
                  <Form.Control
                    type="time"
                    value={endTime}
                    onChange={(e) => setEndTime(e.target.value)}
                  />
                </Form.Group>
              </Col>
            </Row>

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
        </Col>
      </Row>
    </Container>
  );
};

export default AppointmentForm;
