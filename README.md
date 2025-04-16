# Todo List Application

Esta aplicación es una solución para gestionar tareas (todos) que incluye tanto un backend en .NET como un frontend en Angular. El proyecto implementa las funcionalidades especificadas en las tareas del ejercicio, que se pueden encontrar en [Docs/tasks.md](Docs/tasks.md).

## Estructura del Repositorio

- **Rama `app-consola`**: Contiene la primera implementación de la tarea como aplicación de consola en .NET, que corresponde a la tarea principal descrita en [Docs/tasks.md](Docs/tasks.md).
- **Rama `main`**: Contiene la implementación completa con el backend API y el frontend Angular, correspondiente a las tareas extra del ejercicio.

## Arquitectura

### Backend (.NET)

La solución backend está organizada en capas:

- **TodoListApp.Core**:

  - Contiene la lógica de negocio y las entidades
  - Implementa el patrón Repository con `InMemoryTodoListRepository`
  - Incluye validaciones de negocio en `TodoListService`
  - Define interfaces y modelos

- **TodoListApp.Api**:

  - API REST implementada con .NET 9
  - Usa el patrón Minimal API con módulos
  - Expone endpoints para gestionar todos y su progreso

- **TodoListApp.Tests**:
  - Tests unitarios usando xUnit
  - Cubre casos de éxito y error
  - Valida reglas de negocio

### Frontend (Angular)

- **TodoListAppClient**:
  - Aplicación Angular moderna
  - Arquitectura por componentes
  - Servicios para comunicación HTTP
  - Estilos CSS modernos y responsivos

## Cómo Ejecutar el Proyecto

### Backend

1. Navegar al directorio de la API:

```bash
cd TodoListAppSol/TodoListApp.Api
```

2. Restaurar dependencias y compilar:

```bash
dotnet restore
dotnet build
```

3. Ejecutar la API:

```bash
dotnet run
```

La API estará disponible en `https://localhost:5056`

### Frontend

1. Navegar al directorio del cliente:

```bash
cd TodoListAppClient
```

2. Instalar dependencias:

```bash
npm install
```

3. Ejecutar el servidor de desarrollo:

```bash
ng serve
```

La aplicación estará disponible en `http://localhost:4200`

## Características Principales

- Gestión completa de tareas (crear, actualizar, eliminar)
- Sistema de progreso con validaciones
- Interfaz de usuario moderna y responsiva
- Validaciones en frontend y backend
- Tests unitarios completos
- Comunicación asíncrona sin recargas de página

## Colección Postman

Se incluye una colección de Postman (`TodoListApi.postman_collection.json`) para probar la API directamente.

## Notas de Desarrollo

- El backend implementa todas las validaciones requeridas en la especificación original
- El frontend incluye mejoras visuales como barras de progreso y temas oscuro/claro
- La arquitectura permite fácil extensión y mantenimiento
- Se han implementado pruebas unitarias exhaustivas

## Requisitos del Sistema

- .NET 9 SDK
- Node.js (versión 18 o superior)
- Angular CLI

## Pruebas (Testing)

Para ejecutar las pruebas unitarias y de integración:

### Backend (.NET)

Navega al directorio de la solución y ejecuta:

```bash
cd TodoListAppSol
dotnet test
```

### Frontend (Angular)

Navega al directorio de la aplicación cliente y ejecuta:

```bash
cd TodoListAppClient
ng test
```
