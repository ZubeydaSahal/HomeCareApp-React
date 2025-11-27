import React from "react";
import { Button, Card, Col, Row } from "react-bootstrap";
import type { Availability } from "../types/Availability";

interface AvailabilityGridProps {
  availabilities: Availability[];
  apiUrl: string; // ikke brukt nå, men greit å ha hvis du vil bruke bilder senere
  onAvailabilityDeleted?: (id: number) => void;

}

const AvailabilityGrid: React.FC<AvailabilityGridProps> = ({
  availabilities,
  apiUrl,
  onAvailabilityDeleted,

}) => {
  return (
    <div>
      <Row xs={1} sm={2} md={3} lg={4} className="g-4">
        {availabilities.map((a) => (
          <Col key={a.id}>
            <Card className="h-100">
              {/* Her kunne du brukt apiUrl hvis du senere har bilder, f.eks:
                  <Card.Img src={`${apiUrl}/images/${a.personnelId}.jpg`} ... /> 
              */}

              <Card.Body className="d-flex flex-column">
                <Card.Title>{a.date}</Card.Title>

                <Card.Subtitle className="mb-2 text-muted">
                  {a.startTime} – {a.endTime}
                </Card.Subtitle>

                <Card.Text>
                  <strong>Personnel:</strong> {a.personnelId}
                </Card.Text>

                {a.notes && (
                  <><><Card.Text>
                    <strong>Notes:</strong> {a.notes}
                  </Card.Text><div className="mt-auto d-flex justify-content-end">
                      <Button href={`availability/edit/${a.id}`} variant="primary">Edit</Button>
                    </div></><div className="mt-auto d-flex justify-content-between gap-2">
                      <Button href={`availability/delete/${a.id}`} variant="danger">Delete</Button>
                    </div></>
                )}
              </Card.Body>
            </Card>
          </Col>
        ))}
      </Row>
    </div>
  );
};

export default AvailabilityGrid;
