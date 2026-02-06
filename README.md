# Barber Shop Management System

## Introduction

Welcome to the Barber Shop Management System! You have just started your new job as the lead developer for "Gerrit's Cuts," a trendy barber shop with a twist. The owner, Gerrit, has some... unique ways of doing business, including a non-transparent price calculation logic and a "special" approach to customer data.

The previous developer left in a hurry, leaving behind a half-finished system. Your mission is to complete the application, fix the messy data import, and build a modern frontend.

## Important Rules

1.  **Do NOT modification the Starter Code**: You must work within the boundaries of the provided skeleton. You are implementing the missing pieces, not rewriting the architecture.
2.  **Use Newest Angular Standards**: For the frontend, you must use the latest Angular features, including:
    - **Standalone Components** (No NgModules)
    - **Signals** for state management
    - **Control Flow Syntax** (@if, @for)

## Requirements

### 1. Data Importer (Backend)

We "acquired" a customer list from a competitor, but the data is a disaster. It's technically XML, but it's broken and invalid.

- **Task**: Implement the logic described in `Import_Logic.md`.
- **Goal**: Create a `LegacyFileFixer` that reads the broken XML stream, repairs it into valid XML, and then uses the existing parser to import `Appointment` records into the database.
- **Verification**: Ensure all data from `data/BrokenData.xml` (and `data/SampleData.xml`) imports correctly without crashing.

### 2. Web API Implementation (Backend)

The Web API shell is there, but the endpoints are effectively empty.

- **Task**: Complete the implementation of `WebApi/AppointmentEndpoints.cs`.
- **Endpoints to Implement**:
  - `GET /appointments`: Retrieve all appointments (including related services).
  - `GET /appointments/{id}`: Retrieve a specific appointment by ID.
  - `POST /appointments`: Create a new appointment.
  - `PUT /appointments/{id}`: Update an existing appointment.
  - `DELETE /appointments/{id}`: Delete an appointment.
- **Requirement**: Ensure proper error handling (e.g., returning 404 if an ID isn't found).

### 3. Price Calculation (Backend)

- **Task**: Implement the obscure price calculation logic.
- **Specification**: See [Price_Calculation.md](Price_Calculation.md) for the complete pricing rules and requirements.
- **Goal**: Ensure that when appointments are retrieved, their total price reflects these complex rules.
- **Important**: The price must be calculated server-side per appointment, not stored in the database.

### 4. Frontend (Typescript/Web)

The frontend is currently bare-bones. You need to bring it to life!

- **Features**:
  - **Dashboard**: Display a list of upcoming appointments.
  - **Editor**: Create a form to add and edit appointments.
  - **Integration**: Connect your frontend to the backend API you implemented in step 2.
  - **UX**: Make it look beautiful! Gerrit likes "sharp" designs... but remembers, he is a bit of a scammer.
  - **Quirks**: Add some "funny" design choices that hint at ripping off customers (e.g., hidden fees text, pre-checked "tip" boxes, etc.).

## Getting Started

1.  **Backend**: Open the `FullStackApp.sln` solution.
    - Run `dotnet run --project AppHost` to start the backend services.
    - Check the Swagger UI to see the available (but currently unimplemented) endpoints.
2.  **Frontend**: Navigate to `Frontend/`.
    - Run `npm install` to get dependencies.
    - Run `npm start` (or the equivalent script) to launch the development server.

Good luck! You're going to need it.
