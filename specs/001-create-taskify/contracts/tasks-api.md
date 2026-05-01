# Tasks API Contract

**Base URL**: `/api/tasks`  
**Authentication**: None (initial phase)  

## Endpoints

### GET /api/tasks?projectId={id}
**Description**: Get tasks for a project  
**Response**: 200 OK
```json
[
  {
    "id": 1,
    "projectId": 1,
    "title": "Design homepage",
    "description": "Create homepage mockup",
    "status": "To Do",
    "assignee": {
      "id": 1,
      "name": "Alice Johnson",
      "role": "Product Manager"
    },
    "createdAt": "2026-05-01T10:00:00Z",
    "updatedAt": "2026-05-01T10:00:00Z",
    "comments": [
      {
        "id": 1,
        "content": "Looks good",
        "author": {
          "id": 2,
          "name": "Bob Smith"
        },
        "createdAt": "2026-05-01T11:00:00Z"
      }
    ]
  }
]
```

### POST /api/tasks
**Description**: Create a new task  
**Request**:
```json
{
  "projectId": 1,
  "title": "New Task",
  "description": "Task description",
  "assigneeId": 1
}
```
**Response**: 201 Created (same as GET response)

### GET /api/tasks/{id}
**Description**: Get task by ID  
**Response**: 200 OK or 404 Not Found

### PUT /api/tasks/{id}
**Description**: Update task  
**Request**:
```json
{
  "title": "Updated Task",
  "description": "Updated description",
  "status": "In Progress",
  "assigneeId": 2
}
```
**Response**: 200 OK or 404 Not Found

### DELETE /api/tasks/{id}
**Description**: Delete task  
**Response**: 204 No Content or 404 Not Found

### PUT /api/tasks/{id}/status
**Description**: Update task status (for drag-and-drop)  
**Request**:
```json
{
  "status": "Done"
}
```
**Response**: 200 OK or 400 Bad Request

### POST /api/tasks/{id}/comments
**Description**: Add comment to task  
**Request**:
```json
{
  "content": "This looks good"
}
```
**Response**: 201 Created
```json
{
  "id": 2,
  "content": "This looks good",
  "author": { "id": 1, "name": "Alice Johnson" },
  "createdAt": "2026-05-01T12:00:00Z"
}
```

### PUT /api/tasks/{id}/comments/{commentId}
**Description**: Edit comment (only by author)  
**Request**:
```json
{
  "content": "Updated comment"
}
```
**Response**: 200 OK or 403 Forbidden

### DELETE /api/tasks/{id}/comments/{commentId}
**Description**: Delete comment (only by author)  
**Response**: 204 No Content or 403 Forbidden

## Error Responses

- 400 Bad Request: Invalid input, invalid status, assignee not in project
- 403 Forbidden: Attempting to edit/delete others' comments
- 404 Not Found: Task, project, or comment not found
- 500 Internal Server Error: Server error

## Validation Rules

- Title: Required, 1-300 characters
- Description: Optional, max 2000 characters
- Status: Must be one of: "To Do", "In Progress", "In Review", "Done"
- Assignee: Must be a member of the project (if specified)
- Comment content: Required, 1-1000 characters
- Only comment authors can edit/delete their comments