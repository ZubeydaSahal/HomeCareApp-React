// src/pages/AvailabilityListPage.jsx
import React, { useState, useEffect } from 'react';

// Sett denne til din faktiske backend-URL (fra Swagger)
const API_URL = 'https://localhost:5001';

function AvailabilityListPage() {
  const [availabilities, setAvailabilities] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchAvailabilities = async () => {
    try {
      setLoading(true);
      setError(null);

      const response = await fetch(`${API_URL}/api/availability/list`, {
        credentials: 'include', // viktig hvis du bruker cookies/Identity
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data = await response.json();
      console.log('Availabilities from API:', data); // debug i konsoll

      setAvailabilities(data);
    } catch (err) {
      console.error('Error fetching availabilities:', err);
      setError('Kunne ikke hente ledige tider.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchAvailabilities();
  }, []);

  return (
    <div style={{ padding: '1rem' }}>
      <h1>Ledige tider (Availability)</h1>

      <button
        onClick={fetchAvailabilities}
        disabled={loading}
        style={{ marginBottom: '1rem' }}
      >
        {loading ? 'Laster...' : 'Oppdater liste'}
      </button>

      {error && <p style={{ color: 'red' }}>{error}</p>}

      <table border="1" cellPadding="8" cellSpacing="0">
        <thead>
          <tr>
            <th>Id</th>
            <th>PersonnelId</th>
            <th>Date</th>
            <th>Start</th>
            <th>End</th>
            <th>Notes</th>
          </tr>
        </thead>
        <tbody>
          {availabilities.map(a => (
            <tr key={a.id}>
              <td>{a.id}</td>
              <td>{a.personnelId}</td>
              <td>{a.date}</td>
              <td>{a.startTime}</td>
              <td>{a.endTime}</td>
              <td>{a.notes}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default AvailabilityListPage;
