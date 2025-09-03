# 🐾 Petcare API

REST API built with **ASP.NET Core 8** and **Entity Framework Core + SQLite**.  
Supports **Pets, Owners, and Appointments** with full CRUD operations, relationships, and auto-generated Swagger/OpenAPI docs.

---

## 🚀 Run Locally

Clone and run:

```bash
git clone https://github.com/CJRodr/petcare-api.git
cd petcare-api
dotnet run
```

Swagger UI will be available at:
👉 http://localhost:5191/swagger


## 📚 Endpoints (MVP)
Pets
GET /pets → list pets (supports page, pageSize, ownerId filters)
GET /pets/{id} → get single pet by ID
POST /pets → create a new pet
PUT /pets/{id} → update pet
DELETE /pets/{id} → delete pet
Owners
GET /owners → list owners
GET /owners/{id} → get owner with pets
POST /owners → create new owner
PUT /owners/{id} → update owner
DELETE /owners/{id} → delete owner
Appointments
GET /appointments → list appointments (filter by petId)
GET /appointments/{id} → get appointment details
POST /appointments → create appointment for a pet
PUT /appointments/{id} → update appointment
DELETE /appointments/{id} → cancel appointment


## 🧪 Example Usage 
Create an owner:
curl -X POST http://localhost:5191/owners \
 -H "Content-Type: application/json" \
 -d '{"name":"Carolina","email":"carolina@example.com"}'

Create a pet for that owner:

curl -X POST http://localhost:5191/pets \
 -H "Content-Type: application/json" \
 -d '{"name":"Rover","species":"Dog","ownerId":1}'
 
Create an appointment for that pet:

curl -X POST http://localhost:5191/appointments \
 -H "Content-Type: application/json" \
 -d '{"petId":1,"when":"2025-09-03T15:00:00","reason":"Checkup"}'


## 🛠 Tech Stack
.NET 8
ASP.NET Core Web API
Entity Framework Core + SQLite
Swagger/OpenAPI

## 📌 Status
MVP complete with persistence, filtering, and relationships.
Future improvements: authentication, deployment, and automated tests.


