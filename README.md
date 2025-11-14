# OptiMilk Microservices Solution

This solution consists of two .NET 9 microservices designed for modern cattle management and milking yield analytics. 
Each service is container-ready and leverages resilient, scalable patterns for cloud deployment.
Each microservice has its own database context and exposes its API using Scalar for efficient, strongly-typed queries and commands.

## Get Started
To run the microservices locally, ensure you have Docker installed and execute the provided Docker Compose file. Each service will be accessible via its designated ports.

1. **Clone the repository.**
- ```bash
   git clone https://github.com/skeytor/OptiMilk
   cd OptiMilk
  ```
2. **Run Docker Compose:**
   ```bash
   docker-compose up -d --build
   ```
3. **Access the APIs:**
   - Cattle Management API: `http://localhost:8080/scalar/v1`
   - Milking Yield API: `http://localhost:5000/scalar/v1`
   - Load Balancer for Milking Yield API: `http://localhost/api/MilkingYield`

For further details, refer to each microservice's documentation and source code.

## Takeaways
- **Cattle Management API:** It's the producer and manages cattle data with SQL Server backend.
- **Milking Yield API:** It's the consumer and manages milking yield data with PostgreSQL backend, incorporating load balancing and circuit breaker patterns for resilience.

## API Exposure

Both microservices expose their APIs using Scalar, enabling efficient, strongly-typed queries and commands. This approach supports modern CQRS patterns and high-performance data access.

[Cattle Management API Docs](http://localhost:8080/scalar/v1#tag/cattle/get/api/Cattle)

[Milking Yield API Docs](http://localhost:5000/scalar/v1#tag/milkingyield/get/api/MilkingYield)

Furthemore we can access to the Milking Yield API through Load Balancer at: [Load Balancer URL](http://localhost/api/MilkingYield)


## Kafka Integration

OptiMilk uses Kafka for lightweight eventing and eventual consistency between services.

Core points
- Topic topology:
  - `CattleEvents` � cattle lifecycle events (created, updated, deleted).
  - `MilkingEvents` � (reserved) events related to milking sessions (name available in config).
- Service responsibilities:
  - `CattleManagement.API` publishes cattle lifecycle events using the Kafka producer integration (`AddKafkaProducer`).
  - `MilkingYield.API` subscribes to cattle events using the Kafka consumer integration (`AddKafkaConsumer`) and reacts (e.g., to synchronize caches or trigger downstream processing).
- Configuration:
  - The integration expects a `Kafka` configuration section with the following properties:
    - `BootstrapServers` (comma-separated list, e.g. `kafka:29092` or `localhost:9092`)
    - `GroupId` (consumer group id)
    - `AutoOffsetReset` (e.g. `Earliest` | `Latest`)
    - `Topics` with `CattleEvents` and `MilkingEvents`
- Message contract (recommended JSON shape)
  - Cattle created example:
  ```json
    {
      "EventType": "CattleCreated",
      "TimestampUtc": "2025-11-13T12:00:00Z",
      "Payload": {
        "Id": "019a5b08-9dcb-784d-8107-923f2907dde5",
        "TagNumber": "A123",
        "Breed": "Angus",
        "DateOfBirth": "2020-05-15"
      }
    }
   ```
  - Milking session example:
  ```json
    {
      "EventType": "MilkingSessionCreated",
      "TimestampUtc": "2025-11-13T12:05:00Z",
      "Payload": {
        "Id": "d3f9c8a1-... ",
        "CowId": "019a5b08-9dcb-784d-8107-923f2907dde5",
        "YieldInLiters": 12.5,
        "RecordedAt": "2025-11-13T08:00:00Z"
      }
    }
  ``` 

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

## Deployment

- Both services are configured for Linux containers and can be orchestrated using Docker Compose.
- Ready for cloud-native deployment and scaling.

---