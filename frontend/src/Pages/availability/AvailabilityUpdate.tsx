import React, { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import AvailabilityForm from "./AvailabilityForm";
import { Availability } from "../../types/Availability";
import { fetchAvailabilities, updateAvailability } from "./AvailabilityService";

const AvailabilityUpdate: React.FC = () => {
  const { availabilityId } = useParams<{ availabilityId: string }>();
  const navigate = useNavigate();

  const [availability, setAvailability] = useState<Availability | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const loadAvailability = async () => {
      if (!availabilityId) {
        setError("Mangler availability-id i URL-en.");
        setLoading(false);
        return;
      }

      try {
        const data = await fetchAvailabilities(availabilityId);
        setAvailability(data);
      } catch (err) {
        console.error("There was a problem with the fetch operation:", err);
        setError("Kunne ikke laste tilgjengeligheten.");
      } finally {
        setLoading(false);
      }
    };

    loadAvailability();
  }, [availabilityId]);

  const handleAvailabilityUpdated = async (updated: Availability) => {
    try {
      await updateAvailability(updated.id, updated);
      console.log("Availability updated successfully");
      navigate("/availability");
    } catch (error) {
      console.error("There was a problem with the update operation:", error);
      setError("Kunne ikke oppdatere tilgjengeligheten.");
    }
  };

  if (loading) return <p>Loading...</p>;
  if (error) return <p style={{ color: "red" }}>{error}</p>;
  if (!availability) return <p>No availability found</p>;

  return (
    <div>
      <h2>Update Availability</h2>
      <AvailabilityForm
        onAvailabilityChanged={handleAvailabilityUpdated}
        availabilityId={availability.id}
        initialData={availability}
        isUpdate={true}
      />
    </div>
  );
};

export default AvailabilityUpdate;
