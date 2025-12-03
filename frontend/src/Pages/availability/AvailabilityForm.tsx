import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Form, Button } from "react-bootstrap";
import { Availability } from "../../types/Availability";

interface AvailabilityFormProps {
  onAvailabilityChanged: (newAvailability: Availability) => void;
  availabilityId?: number;
  isUpdate?: boolean;
  initialData?: Availability;
}

const AvailabilityForm: React.FC<AvailabilityFormProps> = ({
  onAvailabilityChanged,
  availabilityId,
  isUpdate = false,
  initialData,
}) => {
  const [date, setDate] = useState<string>(initialData?.date || "");
  const [startTime, setStartTime] = useState<string>(
    initialData?.startTime || ""
  );
  const [endTime, setEndTime] = useState<string>(initialData?.endTime || "");
  const [notes, setNotes] = useState<string>(initialData?.notes || "");
  const [error, setError] = useState<string | null>(null);

  const navigate = useNavigate();
  //goes back to previous page
  const onCancel = () => {
    navigate(-1);
  };

  const handleSubmit = (event: React.FormEvent) => {
    event.preventDefault();
    setError(null);
    //validating
    if (!date || !startTime || !endTime) {
      setError("Date, start time og end time er påkrevd.");
      return;
    }

    const newAvailability: Availability = {
      id: availabilityId ?? 0, //cant ignore the backend needs it
      personnelId: "",
      date,
      startTime,
      endTime,
      notes: notes || null,
    };

    onAvailabilityChanged(newAvailability);
  };

  return (
    <Form onSubmit={handleSubmit}>
      <Form.Group controlId="formAvailabilityDate" className="mb-3">
        <Form.Label>Date</Form.Label>
        <Form.Control
          type="date"
          value={date}
          onChange={(e) => setDate(e.target.value)}
          required
        />
      </Form.Group>

      <Form.Group controlId="formAvailabilityStartTime" className="mb-3">
        <Form.Label>Start time</Form.Label>
        <Form.Control
          type="time"
          value={startTime}
          onChange={(e) => setStartTime(e.target.value)}
          required
        />
      </Form.Group>

      <Form.Group controlId="formAvailabilityEndTime" className="mb-3">
        <Form.Label>End time</Form.Label>
        <Form.Control
          type="time"
          value={endTime}
          onChange={(e) => setEndTime(e.target.value)}
          required
        />
      </Form.Group>

      <Form.Group controlId="formAvailabilityNotes" className="mb-3">
        <Form.Label>Notes</Form.Label>
        <Form.Control
          as="textarea"
          rows={3}
          placeholder="Optional notes about this availability"
          value={notes}
          onChange={(e) => setNotes(e.target.value)}
        />
      </Form.Group>

      {error && <p style={{ color: "red" }}>{error}</p>}

      <Button variant="primary" type="submit">
        create availability{" "}
      </Button>
      <Button variant="secondary" onClick={onCancel} className="ms-2">
        Cancel
      </Button>
    </Form>
  );
};

export default AvailabilityForm;
