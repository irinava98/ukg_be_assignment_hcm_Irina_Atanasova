import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import axios from "axios";

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

const PersonDetails: React.FC = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const [person, setPerson] = useState<Person | null>(null);

    const token = localStorage.getItem("token");
    const role = localStorage.getItem("role");
    const personId = localStorage.getItem("personId");
    const department = localStorage.getItem("department");

    useEffect(() => {
        if (!token) {
            navigate("/login");
            return;
        }

        const fetchPerson = async () => {
            try {
                const res = await axios.get(`https://localhost:7201/api/person/${id}`, {
                    headers: { Authorization: `Bearer ${token}` },
                });

                const p = res.data;

                const isSelf = id === personId;
                const sameDepartment = p.department === department;

                const accessDenied =
                    (role === "Employee" && !isSelf) ||
                    (role === "Manager" && !sameDepartment);

                if (accessDenied) {
                    alert("Access Denied");
                    navigate("/people");
                    return;
                }

                setPerson(p);
            } catch (err) {
                console.error("Failed to fetch person details:", err);
                navigate("/people");
            }
        };

        fetchPerson();
    }, [id, token, role, personId, department, navigate]);

    if (!person) return <p>Loading...</p>;

    return (
        <div style={{ padding: "2rem" }}>
            <h2>{person.firstName} {person.lastName}'s Profile</h2>
            <p><strong>Job Title:</strong> {person.jobTitle}</p>
            <p><strong>Department:</strong> {person.department}</p>
            <p><strong>Age:</strong> {person.age}</p>
            <p><strong>Salary:</strong> ${person.salary}</p>
            <p><strong>Address:</strong> {person.houseAddress}</p>
        </div>
    );
};

export default PersonDetails;