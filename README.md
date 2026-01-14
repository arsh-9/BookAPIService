# **Open Library Books API (Intermediate Layer)**

## Overview

This project is a small, well-structured ASP.NET Core Web API that acts as an intermediate layer between clients and the Open Library API (https://openlibrary.org).

The API exposes endpoints for searching and listing books while applying:

- mapping and response flattening

- input handling

- caching

- retry and resilience policies

- clean separation of concerns

- Centralized exception handling

- Structured logging with ILogger

## Architecture & Design Principles

The solution follows standard clean architecture:

- Controllers handle HTTP concerns only

- Services encapsulate business rules and data shaping

- Clients manage external HTTP communication

- DTOs isolate external contracts from internal models

- Configuration is externalized using the IOptions pattern

## Endpoints

1. Search Books

```
GET /api/books/search?title=pride%20and%20prejudice
GET /api/books/search?query=pride
```

Behavior

- Supports filtering by title or query

- If both are provided, title takes precedence

- Returns 0–1 results

- Minimal response payload

Response Example

```
[
  {
    "title": "Pride and Prejudice",
    "coverUrl": "https://covers.openlibrary.org/b/id/8231996-L.jpg",
    "key": "/works/OL14986703W"
  }
]
```

2. List Books by Subject

```
GET /api/books/list?subject=adventure&limit=10&offset=0
```

Behavior

- Returns at most 10 books (limit is capped server-side)

- Results are flattened and normalized

- Designed for repeatable queries

Response Example

```
[
  {
    "title": "Pride and Prejudice",
    "publishYear": 1813,
    "authors": "Jane Austen",
    "subjects": "courtship, manners",
    "coverUrl": "https://covers.openlibrary.org/b/id/8231996-M.jpg"
  }
]
```

##Caching Strategy

- In-memory caching is applied to the List endpoint only

- Cache keys are derived from: subject + effective limit + offset

- Configurable absolute and sliding expiration

- Caching is implemented in the service layer, not controllers


## Prerequisites

Make sure you have the following installed:

- .NET 8 SDK

- Git

- Any IDE (Visual Studio / VS Code / Rider)

Verify installation:

```
dotnet --version
```

## How to Run the Project

- Clone the repository

```
git clone https://github.com/arsh-9/BookAPIService.git
cd BookAPIService
```

- Restore dependencies

```
dotnet restore
```

- Run the API

```
dotnet run
```





