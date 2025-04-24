import React, { useEffect, useState } from "react";
import axios from "axios";
import { useParams, useNavigate } from "react-router-dom";

interface Person {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  jobTitle: string;
  department: string;
  age: number;
  salary: number;
  houseAddress: string;
}

const EditPerson: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [person, setPerson] = useState<Person | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (!token) {
      setError("You are not logged in. Please sign in first.");
      navigate("/login");
      return;
    }

    if (id) {
      axios
        .get(`https://localhost:7201/api/person/${id}`, {
          headers: { Authorization: `Bearer ${token}` },
        })
        .then((response) => {
          setPerson(response.data);
        })
        .catch((error) => {
          console.error("Error fetching person:", error);
          if (error.response?.status === 401) {
            setError("Unauthorized. Please log in again.");
            navigate("/login");
          } else {
            setError("Failed to fetch person data.");
          }
        })
        .finally(() => setLoading(false));
    }
  }, [id, navigate]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (person) {
      setPerson({
        ...person,
        [e.target.name]: e.target.type === "number" ? +e.target.value : e.target.value,
      });
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const token = localStorage.getItem("token");

    if (!token) {
      setError("You are not logged in. Please sign in first.");
      navigate("/login");
      return;
    }

    if (person) {
      try {
        await axios.put("https://localhost:7201/api/person", person, {
          headers: { Authorization: `Bearer ${token}` },
        });
        alert("Person updated successfully!");
        navigate("/people");
      } catch (error) {
        console.error("Error updating person:", error);
        setError("Failed to update person");
      }
    }
  };

  if (loading) return <div className="loading">Loading...</div>;
  if (!person) return <div className="error-message">No person data found</div>;

  return (
    <div className="form-container">
      <h2 className="form-title">Edit Person</h2>
      {error && <div className="error-message">{error}</div>}
      
      <form onSubmit={handleSubmit} className="person-form">
        <div className="form-group">
          <label className="form-label">First Name</label>
          <input
            type="text"
            name="firstName"
            value={person.firstName}
            onChange={handleChange}
            className="form-input"
            required
          />
        </div>
        
        <div className="form-group">
          <label className="form-label">Last Name</label>
          <input
            type="text"
            name="lastName"
            value={person.lastName}
            onChange={handleChange}
            className="form-input"
            required
          />
        </div>
        
        <div className="form-group">
          <label className="form-label">Email</label>
          <input
            type="email"
            name="email"
            value={person.email}
            onChange={handleChange}
            className="form-input"
            required
          />
        </div>
        
        <div className="form-group">
          <label className="form-label">Job Title</label>
          <input
            type="text"
            name="jobTitle"
            value={person.jobTitle}
            onChange={handleChange}
            className="form-input"
            required
          />
        </div>
        
        <div className="form-group">
          <label className="form-label">Department</label>
          <input
            type="text"
            name="department"
            value={person.department}
            onChange={handleChange}
            className="form-input"
            required
          />
        </div>
        
        <div className="form-group">
          <label className="form-label">Age</label>
          <input
            type="number"
            name="age"
            value={person.age}
            onChange={handleChange}
            className="form-input"
            required
          />
        </div>
        
        <div className="form-group">
          <label className="form-label">Salary</label>
          <input
            type="number"
            name="salary"
            value={person.salary}
            onChange={handleChange}
            className="form-input"
            required
          />
        </div>
        
        <div className="form-group full-width">
          <label className="form-label">House Address</label>
          <input
            type="text"
            name="houseAddress"
            value={person.houseAddress}
            onChange={handleChange}
            className="form-input"
            required
          />
        </div>
        
        <div className="form-actions">
          <button type="button" className="cancel-button" onClick={() => navigate("/people")}>
            Cancel
          </button>
          <button type="submit" className="submit-button">
            Update Person
          </button>
        </div>
      </form>
    </div>
  );
};

export default EditPerson;