# OptiMilk Microservices Solution

This solution consists of two .NET 9 microservices designed for modern cattle management and milking yield analytics. 
Each service is container-ready and leverages resilient, scalable patterns for cloud deployment.
Each microservice has its own database context and exposes its API using Scalar for efficient, strongly-typed queries and commands.

## Kafka Integration
The CattleManagement.API acts as the producer and emits a CattleDeletedEvent to a Kafka topic (for example cattle.updates); MilkingYield.API is the consumer and subscribes to that topic to react when a cattle record is deleted (delete or archive related milking-yield records, maintain referential integrity, emit compensating events, etc.).

- Producer: `CattleManagement.API`
    - Publishes `CattleDeletedEvent` to a configured topic (example: `cattle.updates` or `cattle.deleted`).
    - Typical event payload:
  ```json
  {
    "CattleId": "00000000-0000-0000-0000-000000000000",
    "DeletedAt": "2025-11-13T12:00:00Z",
    "DeletedBy": "user@example.com",
    "Reason": "Deceased"
  }

### Sumarized Flow
**Key points:**

- **Producer**: CattleManagement.API
- **Publishes CattleDeletedEvent**: (key = cattle id) to a configured topic (for example cattle.updates or cattle.deleted).
- **Event payload**: typically includes CattleId, DeletedAt, optional Reason and DeletedBy.
- Configure broker via KAFKA_BOOTSTRAP_SERVERS and topic name via a config key (for example CATTLE_UPDATE_TOPIC).
- Consumer: MilkingYield.API
- Subscribes using a consumer group (for example milkingyield-consumer-group).
- On receipt of CattleDeletedEvent it should:
- Validate and deserialize the event.
- Check idempotency (skip if already handled).
- Soft-delete or remove all related milking-yield records, or mark them as orphaned/archived.
- Persist changes in a transaction and optionally publish follow-up events.
- Handle transient failures with retries and send failing messages to a dead-letter topic.


## Microservices Overview

### 1. CattleManagement.API

**Purpose:**  
Manages cattle data, including registration, tracking, and health records.

**Main Features:**
- **Entity Framework Core (SQL Server):** Robust data access and migrations.
- **OpenAPI Support:** API documentation and testing via Swagger UI.
- **Docker Support:** Ready for containerization and orchestration.
- **Scalar API Exposure:**  
  - Uses `Scalar.AspNetCore` to expose scalar endpoints for efficient, strongly-typed queries and commands.

### 2. MilkingYield.API

**Purpose:**
Manages milking yield data, including recording yields, analyzing trends, and generating reports.

**Main Features:**
- **Entity Framework Core (PostgreSQL):** Advanced data access and migrations.
- **OpenAPI Support:** API documentation and testing via Swagger UI.
- **Docker Support:** Ready for containerization and orchestration.
- **Scalar API Exposure:**  
  - Uses `Scalar.AspNetCore` for API documentation
- **Resilience Patterns:**
  - **Load Balancer:**  
    - Utilizes `Microsoft.Extensions.Http.Resilience` to distribute HTTP requests across multiple service instances, improving scalability and fault tolerance.
  - **Circuit Breaker:**  
    - Implements circuit breaker policies via `Polly` and `Microsoft.Extensions.Http.Resilience` to automatically detect and isolate failing service calls, preventing cascading failures and improving overall reliability.

## Key Technologies

- **.NET 9**
- **Entity Framework Core (SQL Server & PostgreSQL)**
- **Polly** (Resilience, Circuit Breaker)
- **Microsoft.Extensions.Http.Resilience** (Load Balancing, Circuit Breaker)
- **Docker** (Linux containers)
- **OpenAPI/Scalar**

## API Exposure

Both microservices expose their APIs using Scalar, enabling efficient, strongly-typed queries and commands. This approach supports modern CQRS patterns and high-performance data access.

[Cattle Management API Docs](http://localhost:8080/scalar/v1#tag/cattle/get/api/Cattle)

[Milking Yield API Docs](http://localhost:5000/scalar/v1#tag/milkingyield/get/api/MilkingYield)

Furthemore we can access to the Milking Yield API through Load Balancer at: [Load Balancer URL](http://localhost/api/MilkingYield)

## Deployment

- Both services are configured for Linux containers and can be orchestrated using Docker Compose.
- Ready for cloud-native deployment and scaling.

---
## Get Started
To run the microservices locally, ensure you have Docker installed and execute the provided Docker Compose file. Each service will be accessible via its designated ports.

1. **Clone the repository.**
- ```bash
   git clone https://github.com/skeytor/OptiMilk
  ```
2. **Run Docker Compose:**
   ```bash
   docker-compose up --build
   ```
3. **Access the APIs:**
   - Cattle Management API: `http://localhost:8080`
   - Milking Yield API: `http://localhost:5000`
   - Load Balancer for Milking Yield API: `http://localhost/api/MilkingYield`

For further details, refer to each microservice's documentation and source code.