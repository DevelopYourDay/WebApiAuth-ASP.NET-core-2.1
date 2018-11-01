#  Welcome to the WebApiAuth-ASP.NET-core-2.1

### ASP.NET Core 2.1 API that supports user registration, login with JWT authentication and user management.

### The application is configured to use the Entity Framework Core InMemory provider which allows EF Core to create and connect to local database rather than you having to install a real database server.

**Controllers** - define the end points / routes for the web api, controllers are the entry point into the web api from client applications via http requests.

**Services** - contain business logic, validation and database access code.

**Entities** - represent the application data that is stored in the database.

**Dtos** - data transfer objects used by controllers to expose a limited set of entity data via the api, and for model binding data from HTTP requests to controller action methods.

**Helpers** - anything that doesn't fit into the above folders.
