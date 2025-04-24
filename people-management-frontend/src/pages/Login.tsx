import React, { useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";

interface LoginResponse {
  token: string;
  role: string;
  personId: number;
  department?: string;
  username: string;
}

interface LoginProps {
  setIsAuthenticated: (value: boolean) => void;
}

const Login: React.FC<LoginProps> = ({ setIsAuthenticated }) => {
  const navigate = useNavigate();
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError("");

    try {
      const response = await axios.post<LoginResponse>(
        "https://localhost:7201/api/auth/login",
        { username, password }
      );

      const { token, role, personId, department, username: responseUsername } = response.data;

      localStorage.setItem("token", token);
      localStorage.setItem("username", responseUsername);
      localStorage.setItem("role", role);
      localStorage.setItem("personId", personId.toString());
      if (department) {
        localStorage.setItem("department", department);
      }

      setIsAuthenticated(true);

      if (role === "Employee") {
        navigate(`/people/${personId}`);
      } else {
        navigate("/people");
      }
    } catch (err) {
      console.error("Login failed", err);
      setError("Invalid username or password.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="form-container" style={{ maxWidth: "500px" }}>
      <h2 className="form-title">Login</h2>
      {error && <div className="error-message">{error}</div>}
      
      <form onSubmit={handleLogin} className="person-form" style={{ gridTemplateColumns: "1fr" }}>
        <div className="form-group full-width">
          <label className="form-label">Username</label>
          <input
            type="text"
            className="form-input"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
            disabled={loading}
          />
        </div>
        
        <div className="form-group full-width">
          <label className="form-label">Password</label>
          <input
            type="password"
            className="form-input"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            disabled={loading}
          />
        </div>
        
        <div className="form-actions">
          <button 
            type="submit" 
            className="submit-button"
            disabled={loading}
          >
            {loading ? "Signing in..." : "Sign In"}
          </button>
        </div>
      </form>
    </div>
  );
};

export default Login;