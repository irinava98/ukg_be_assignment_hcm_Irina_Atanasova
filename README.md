# People Management System API

## Overview

The **People Management System API** is a backend service built with **ASP.NET Core 8.0** to manage people (employees, managers, and HR admins) within an organization. The system uses **JWT authentication** to secure access and **role-based access control** (RBAC) to limit what users can do based on their roles.

The available roles are:
- **HRAdmin**: Full access to manage users and data.
- **Manager**: Can only view and manage people in their department.
- **Employee**: Can only view and edit their own profile.

## Features

### 1. **Role-Based Access Control (RBAC)**
- **HRAdmin**: Can create, read, update, and delete anyone's data.
- **Manager**: Can only view and update people within their department.
- **Employee**: Can only view and update their own profile.

### 2. **CRUD Operations for People**
- **Create (POST)**: HRAdmin can add new people to the system.
- **Read (GET)**: 
  - HRAdmin can view all people.
  - Managers can only view people in their department.
  - Employees can only view their own profile.
- **Update (PUT)**: 
  - HRAdmin and Managers can update people's data.
  - Managers can only update people in their department.
- **Delete (DELETE)**: HRAdmin can delete people from the system.

### 3. **User Registration via API**
- User registration (for HRAdmin, Manager, and Employee roles) is handled via the backend API.
- **HRAdmin** is the only role that can create new users.

### 4. **Authentication & Authorization**
- **JWT tokens** are used to authenticate users.
- After logging in, users receive a JWT token that must be included in the header for all requests that require authentication.

---

## Getting Started

### Prerequisites
- **.NET 8.0 or higher**
- **A database** (the API uses an in-memory database by default but can be configured to use other databases like SQL Server or MySQL).

### Installation

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/irinava98/ukg_be_assignment_hcm_Irina_Atanasova.git
