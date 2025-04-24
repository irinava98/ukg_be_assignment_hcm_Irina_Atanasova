import React, { useEffect, useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";

interface Person {
  id: number;
  firstName: string;
  lastName: string;
  jobTitle: string;
  department: string;
  age: number;
  salary: number;
  houseAddress: string;
}

const PeopleList: React.FC = () => {
  const navigate = useNavigate();
  const [people, setPeople] = useState<Person[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const token = localStorage.getItem("token");
  const role = localStorage.getItem("role");
  const currentDepartment = localStorage.getItem("department");

  useEffect(() => {
    if (!token) {
      navigate("/login");
      return;
    }

    const fetchPeople = async () => {
      try {
        setLoading(true);
        const res = await axios.get("https://localhost:7201/api/person", {
          headers: { Authorization: `Bearer ${token}` },
        });

        let filteredPeople = res.data;

        if (role === "Manager") {
          filteredPeople = res.data.filter((p: Person) => p.department === currentDepartment);
        }

        setPeople(filteredPeople);
      } catch (err) {
        console.error("Failed to fetch people", err);
        setError("Error fetching people");
      } finally {
        setLoading(false);
      }
    };

    fetchPeople();
  }, [token, role, currentDepartment, navigate]);

  const handleDelete = async (id: number) => {
    if (!window.confirm("Are you sure you want to delete this person?")) return;

    try {
      await axios.delete(`https://localhost:7201/api/person/${id}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      setPeople(prev => prev.filter(p => p.id !== id));
    } catch (err) {
      console.error("Delete error:", err);
      setError("Failed to delete person");
    }
  };

  if (loading) return <div className="loading">Loading...</div>;
  if (error) return <div className="error-message">{error}</div>;

  return (
    <div className="people-container">
      <div className="people-header">
        <h2>People Records</h2>
        {role === "HRAdmin" && (
          <button 
            onClick={() => navigate("/add")}
            className="add-button"
          >
            Add New Person
          </button>
        )}
      </div>
      
      <div className="table-container">
        <table className="people-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Name</th>
              <th>Job Title</th>
              <th>Department</th>
              <th>Age</th>
              <th>Salary</th>
              <th>Address</th>
              {(role === "Manager" || role === "HRAdmin") && <th>Actions</th>}
            </tr>
          </thead>
          <tbody>
            {people.map(p => (
              <tr key={p.id}>
                <td>{p.id}</td>
                <td>{p.firstName} {p.lastName}</td>
                <td>{p.jobTitle}</td>
                <td>{p.department}</td>
                <td>{p.age}</td>
                <td>${p.salary.toLocaleString()}</td>
                <td>{p.houseAddress}</td>
                {(role === "Manager" || role === "HRAdmin") && (
                  <td className="action-buttons">
                    <button 
                      onClick={() => navigate(`/edit/${p.id}`)}
                      className="edit-button"
                    >
                      Edit
                    </button>
                    {role === "HRAdmin" && (
                      <button
                        onClick={() => handleDelete(p.id)}
                        className="delete-button"
                      >
                        Delete
                      </button>
                    )}
                  </td>
                )}
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default PeopleList;