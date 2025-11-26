import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'

import HomePage from './home/HomePage'
import { Container } from 'react-bootstrap'

function App() {
  return (
    <>
      <NavMenu/>
      <Container>
      <Router>
        <Router>
          <Route path="/" element={<HomePage/>} />
          <Route path="/item" element={<item/>} />
          
        </Router>



      </Router>

      </Container>
    </>
  )
}

export default App
