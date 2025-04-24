import React, { useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";

const AddPerson: React.FC = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    email: "",
    jobTitle: "",
    department: "",
    age: 0,
    salary: 0,
    houseAddress: "",
  });
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState("");

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: name === "age" || name === "salary" ? Number(value) : value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setIsSubmitting(true);
    
    const token = localStorage.getItem("token");
    if (!token) {
      setError("Authentication required. Please login again.");
      setIsSubmitting(false);
      return;
    }

    try {
      const response = await axios.post("https://localhost:7201/api/person", formData, {
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json"
        }
      });

      if (response.status === 201) {
        alert("Person added successfully!");
        navigate("/people");
      }
    } catch (error) {
      let errorMessage = "Failed to add person";
      if (axios.isAxiosError(error)) {
        if (error.response?.status === 401) {
          errorMessage = "Unauthorized: You don't have HR Admin privileges";
        } else if (error.response?.data) {
          errorMessage = error.response.data;
        }
      }
      setError(errorMessage);
      console.error("Error adding person:", error);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div style={{
      padding: "2rem",
      maxWidth: "600px",
      margin: "0 auto",
      backgroundColor: "#f8f9fa",
      borderRadius: "8px",
      boxShadow: "0 2px 4px rgba(0,0,0,0.1)"
    }}>
      <h2 style={{ 
        color: "#2c3e50",
        marginBottom: "1.5rem",
        textAlign: "center"
      }}>
        Add New Person
      </h2>
      
      {error && (
        <div style={{
          color: "#721c24",
          backgroundColor: "#f8d7da",
          padding: "0.75rem",
          borderRadius: "4px",
          marginBottom: "1rem",
          border: "1px solid #f5c6cb"
        }}>
          {error}
        </div>
      )}

      <form onSubmit={handleSubmit} style={{ display: "grid", gap: "1.25rem" }}>
        {Object.entries(formData).map(([key, value]) => (
          <div key={key} style={{ display: "flex", flexDirection: "column" }}>
            <label style={{
              marginBottom: "0.5rem",
              fontWeight: "500",
              color: "#2c3e50"
            }}>
              {key.charAt(0).toUpperCase() + key.slice(1).replace(/([A-Z])/g, ' $1')}:
            </label>
            <input
              type={typeof value === "number" ? "number" : "text"}
              name={key}
              value={value}
              onChange={handleChange}
              required
              style={{
                padding: "0.75rem",
                border: "1px solid #ced4da",
                borderRadius: "4px",
                fontSize: "1rem"
              }}
            />
          </div>
        ))}
        
        <button 
          type="submit"
          disabled={isSubmitting}
          style={{
            padding: "0.75rem",
            backgroundColor: isSubmitting ? "#95a5a6" : "#2c3e50",
            color: "white",
            border: "none",
            borderRadius: "4px",
            cursor: isSubmitting ? "not-allowed" : "pointer",
            fontSize: "1rem",
            fontWeight: "500",
            transition: "background-color 0.3s",
            marginTop: "1rem"
          }}
        >
          {isSubmitting ? "Adding..." : "Add Person"}
        </button>
      </form>
    </div>
  );
};

export default AddPerson;