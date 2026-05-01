# Notifications API Contract

**Base URL**: `/api/notifications`  
**Authentication**: None (initial phase)  
**Technology**: SignalR for real-time notifications  

## WebSocket Connection

**Hub URL**: `/notifications`  
**Methods**:
- `JoinProject(string projectId)`: Join project notification group
- `LeaveProject(string projectId)`: Leave project notification group

## Server-to-Client Events

### TaskUpdated
**Description**: Sent when a task is updated (status change, assignment, etc.)  
**Payload**:
```json
{
  "type": "TaskUpdated",
  "projectId": "1",
  "taskId": "1",
  "task": {
    "id": 1,
    "title": "Design homepage",
    "status": "In Progress",
    "assignee": { "id": 1, "name": "Alice Johnson" }
  },
  "updatedBy": { "id": 2, "name": "Bob Smith" },
  "timestamp": "2026-05-01T10:30:00Z"
}
```

### TaskCreated
**Description**: Sent when a new task is created  
**Payload**:
```json
{
  "type": "TaskCreated",
  "projectId": "1",
  "task": { /* full task object */ },
  "createdBy": { "id": 1, "name": "Alice Johnson" },
  "timestamp": "2026-05-01T10:00:00Z"
}
```

### CommentAdded
**Description**: Sent when a comment is added to a task  
**Payload**:
```json
{
  "type": "CommentAdded",
  "projectId": "1",
  "taskId": "1",
  "comment": {
    "id": 1,
    "content": "Looks good",
    "author": { "id": 2, "name": "Bob Smith" },
    "createdAt": "2026-05-01T11:00:00Z"
  }
}
```

### ProjectUpdated
**Description**: Sent when project details change  
**Payload**:
```json
{
  "type": "ProjectUpdated",
  "projectId": "1",
  "project": { /* full project object */ },
  "updatedBy": { "id": 1, "name": "Alice Johnson" },
  "timestamp": "2026-05-01T09:00:00Z"
}
```

## REST Endpoints (Fallback)

### GET /api/notifications/recent?projectId={id}&since={timestamp}
**Description**: Get recent notifications for polling fallback  
**Response**: 200 OK
```json
[
  {
    "id": "123",
    "type": "TaskUpdated",
    "projectId": "1",
    "data": { /* event payload */ },
    "timestamp": "2026-05-01T10:30:00Z"
  }
]
```

## Connection Management

- Clients connect to SignalR hub on application start
- Join project groups when viewing Kanban boards
- Leave groups when navigating away
- Automatic reconnection on connection loss
- Fallback to polling if WebSocket unavailable

## Performance Considerations

- Notifications sent only to project members
- Debounced updates for rapid changes (e.g., drag-and-drop)
- Lightweight payloads to minimize bandwidth
- Server-side filtering to prevent spam

## Security

- Input validation on all notification data
- XSS prevention in notification content
- Rate limiting to prevent abuse
- Authentication hooks ready for future auth implementation