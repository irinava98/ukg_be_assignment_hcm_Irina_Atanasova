import React, { useEffect, useState } from "react";
import { BrowserRouter as Router, Routes, Route, Link, useNavigate } from "react-router-dom";
import Login from "./pages/Login";
import PeopleList from "./pages/PeopleList";
import PersonDetails from "./pages/PersonDetails";
import EditPerson from "./pages/EditPerson";
import AddPerson from "./pages/AddPerson";
import "./index.css";

interface NavigationProps {
  isAuthenticated: boolean;
  setIsAuthenticated: (value: boolean) => void;
}

const Navigation: React.FC<NavigationProps> = ({ isAuthenticated, setIsAuthenticated }) => {
  const navigate = useNavigate();
  const [role, setRole] = useState<string | null>(null);

  useEffect(() => {
    const checkAuth = () => {
      const role = localStorage.getItem("role");
      setRole(role);
    };

    checkAuth();
    window.addEventListener('storage', checkAuth);
    return () => window.removeEventListener('storage', checkAuth);
  }, []);

  const logout = () => {
    localStorage.clear();
    setIsAuthenticated(false);
    setRole(null);
    navigate("/login");
  };

  return (
    <nav className="navbar">
      <Link to="/" className="nav-logo">People Management</Link>
      <div className="nav-links">
        {isAuthenticated ? (
          <>
            {(role === "Manager" || role === "HRAdmin") && (
              <Link to="/people" className="nav-link">People</Link>
            )}
            {role === "Employee" && (
              <Link to={`/people/${localStorage.getItem("personId")}`} className="nav-link">
                My Profile
              </Link>
            )}
            {role === "HRAdmin" && (
              <Link to="/add" className="nav-link">Add Person</Link>
            )}
            <button onClick={logout} className="auth-button logout-button">
              <span className="button-icon">🚪</span> Logout
            </button>
          </>
        ) : (
          <Link to="/login" className="auth-button login-button">
            <span className="button-icon">🔑</span> Login
          </Link>
        )}
      </div>
    </nav>
  );
};

const App: React.FC = () => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  useEffect(() => {
    const checkAuth = () => {
      const token = localStorage.getItem("token");
      setIsAuthenticated(!!token);
    };

    checkAuth();
    window.addEventListener('storage', checkAuth);
    return () => window.removeEventListener('storage', checkAuth);
  }, []);

  return (
    <Router>
      <Navigation isAuthenticated={isAuthenticated} setIsAuthenticated={setIsAuthenticated} />
      <Routes>
        <Route path="/login" element={<Login setIsAuthenticated={setIsAuthenticated} />} />
        <Route path="/people" element={<PeopleList />} />
        <Route path="/people/:id" element={<PersonDetails />} />
        <Route path="/edit/:id" element={<EditPerson />} />
        <Route path="/add" element={<AddPerson />} />
        <Route path="/" element={<Home />} />
      </Routes>
    </Router>
  );
};

const Home = () => (
  <div className="form-container">
    <h2 className="form-title">Welcome to People Management System</h2>
    <p style={{ textAlign: "center" }}>Please use the navigation above to access the system</p>
  </div>
);

export default App;