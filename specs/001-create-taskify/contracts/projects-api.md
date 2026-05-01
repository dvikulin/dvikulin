# Projects API Contract

**Base URL**: `/api/projects`  
**Authentication**: None (initial phase)  

## Endpoints

### GET /api/projects
**Description**: Get all projects  
**Response**: 200 OK
```json
[
  {
    "id": 1,
    "name": "Website Redesign",
    "description": "Redesign company website",
    "createdAt": "2026-05-01T10:00:00Z",
    "teamMembers": [
      {
        "id": 1,
        "name": "Alice Johnson",
        "role": "Product Manager"
      }
    ]
  }
]
```

### POST /api/projects
**Description**: Create a new project  
**Request**:
```json
{
  "name": "New Project",
  "description": "Project description"
}
```
**Response**: 201 Created
```json
{
  "id": 2,
  "name": "New Project",
  "description": "Project description",
  "createdAt": "2026-05-01T10:00:00Z"
}
```

### GET /api/projects/{id}
**Description**: Get project by ID  
**Response**: 200 OK or 404 Not Found

### PUT /api/projects/{id}
**Description**: Update project  
**Request**: Same as POST  
**Response**: 200 OK or 404 Not Found

### DELETE /api/projects/{id}
**Description**: Delete project  
**Response**: 204 No Content or 404 Not Found

### POST /api/projects/{id}/members
**Description**: Add team member to project  
**Request**:
```json
{
  "userId": 2
}
```
**Response**: 200 OK or 400 Bad Request

### DELETE /api/projects/{id}/members/{userId}
**Description**: Remove team member from project  
**Response**: 204 No Content or 404 Not Found

## Error Responses

- 400 Bad Request: Invalid input data
- 404 Not Found: Project or user not found
- 500 Internal Server Error: Server error

## Validation Rules

- Project name: Required, 1-200 characters, unique
- Description: Optional, max 1000 characters
- User must exist to be added as member
- Cannot add duplicate members