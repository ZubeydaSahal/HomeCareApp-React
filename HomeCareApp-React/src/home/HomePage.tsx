// src/home/HomePage.jsx
import React from "react";
import Carousel from "react-bootstrap/Carousel";
import Image from "react-bootstrap/Image";

// Bruk samme API_URL som i AvailabilityListPage
const API_URL = import.meta.env.VITE_API_URL;

const HomePage = () => {
  return (
    <div className="text-center">
      <h1 className="display-4 mb-4">Welcome to HomeCare</h1>

      <Carousel>
        <Carousel.Item>
          {/* Bytt til faktisk bilde du har, f.eks. lagres under wwwroot/images i backend */}
          <Image
            src={`${API_URL}/images/homecare1.jpg`}
            className="d-block w-100"
            alt="Care at home"
          />
          <Carousel.Caption>
            <h3>Personalized care</h3>
            <p>Plan and follow up home visits for your patients.</p>
          </Carousel.Caption>
        </Carousel.Item>

        <Carousel.Item>
          <Image
            src={`${API_URL}/images/homecare2.jpg`}
            className="d-block w-100"
            alt="Nurse visiting"
          />
          <Carousel.Caption>
            <h3>Availability overview</h3>
            <p>See when personnel are available in one place.</p>
          </Carousel.Caption>
        </Carousel.Item>

        <Carousel.Item>
          <Image
            src={`${API_URL}/images/homecare3.jpg`}
            className="d-block w-100"
            alt="Family and patient"
          />
          <Carousel.Caption>
            <h3>Secure communication</h3>
            <p>Appointments and information gathered in one system.</p>
          </Carousel.Caption>
        </Carousel.Item>
      </Carousel>
    </div>
  );
};

export default HomePage;
