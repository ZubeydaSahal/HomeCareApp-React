import React from "react";

const HomePage: React.FC = () => {
  return (
    <div className="text-center">
      <h1 className="display-4 mb-4">Welcome to HomeCare</h1>
      <p className="lead">
        Manage home visits, personnel availability and patient appointments in one place.
      </p>

      <div className="mt-4">
        <p>
          Use the navigation bar to:
        </p>
        <ul className="list-unstyled">
          <li>• See and manage availabilities</li>
          <li>• Create and edit appointments</li>
          <li>• Log in as personnel, patient or admin</li>
        </ul>
      </div>
    </div>
  );
};

export default HomePage;
