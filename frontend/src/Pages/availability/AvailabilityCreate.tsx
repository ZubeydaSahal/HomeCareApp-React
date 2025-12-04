import React from "react";
import { useNavigate } from "react-router-dom";
import { Container, Row, Col } from "react-bootstrap";
import AvailabilityForm from "./AvailabilityForm";
import { Availability } from "../../types/Availability";
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
    <Container className="py-4">
      <Row className="justify-content-center">
        <Col xs={12} md={8} lg={6}>
          <h2 className="fw-bold mb-3">Create New Availability</h2>
          <div className="card border-1 border-dark bg-light">
            <div className="card-body p-4">
              <AvailabilityForm onAvailabilityChanged={handleAvailabilityCreated} />
            </div>
          </div>
        </Col>
      </Row>
    </Container>
  );
};

export default AvailabilityCreate;
