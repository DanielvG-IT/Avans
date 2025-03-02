# CoreLink.WebApi | v1 Documentation

## Version: 1.0.0

### Servers

- **URL**: `avansict2226789.azurewebsites.net`

## Endpoints

### /account/register

**POST**  
Registers a new user.

#### Request Body

- **Content-Type**: `application/json`
- **Schema**: `RegisterRequest`
  
#### Responses

- **200 OK**  
- **400 Bad Request**  
  - **Content-Type**: `application/problem+json`
  - **Schema**: `HttpValidationProblemDetails`

---

### /account/login

**POST**  
Logs in a user.

#### Parameters

- **useCookies** (Query, optional)  
  - Type: `boolean`
- **useSessionCookies** (Query, optional)  
  - Type: `boolean`

#### Request Body

- **Content-Type**: `application/json`
- **Schema**: `LoginRequest`
  
#### Responses

- **200 OK**
  - **Content-Type**: `application/json`
  - **Schema**: `AccessTokenResponse`

---

### /environments

**GET**  
Retrieves a list of user environments.

#### Responses

- **200 OK**
  - **Content-Type**: `application/json`
  - **Schema**: `array of Environment2D`

**POST**  
Adds a new environment.

#### Request Body

- **Content-Type**: `application/json`
- **Schema**: `Environment2D`
  
#### Responses

- **200 OK**
  - **Content-Type**: `application/json`
  - **Schema**: `Environment2D`

---

### /environments/{environmentId}

**PUT**  
Edits an existing environment.

#### Parameters

- **environmentId** (Path, required)  
  - Type: `string`, Format: `uuid`

#### Request Body

- **Content-Type**: `application/json`
- **Schema**: `Environment2D`
  
#### Responses

- **200 OK**

**DELETE**  
Removes an environment.

#### Parameters

- **environmentId** (Path, required)  
  - Type: `string`, Format: `uuid`

#### Responses

- **200 OK**

---

### /environments/{environmentId}/objects

**GET**  
Retrieves objects in the environment.

#### Parameters

- **environmentId** (Path, required)  
  - Type: `string`, Format: `uuid`

#### Responses

- **200 OK**
  - **Content-Type**: `application/json`
  - **Schema**: `array of Object2D`

**POST**  
Adds a new object to the environment.

#### Parameters

- **environmentId** (Path, required)  
  - Type: `string`, Format: `uuid`

#### Request Body

- **Content-Type**: `application/json`
- **Schema**: `Object2D`
  
#### Responses

- **200 OK**
  - **Content-Type**: `application/json`
  - **Schema**: `Object2D`

---

### /environments/{environmentId}/objects/{objectId}

**PUT**  
Edits an object in the environment.

#### Parameters

- **environmentId** (Path, required)  
  - Type: `string`, Format: `uuid`
- **objectId** (Path, required)  
  - Type: `string`, Format: `uuid`

#### Request Body

- **Content-Type**: `application/json`
- **Schema**: `Object2D`
  
#### Responses

- **200 OK**

**DELETE**  
Removes an object from the environment.

#### Parameters

- **environmentId** (Path, required)  
  - Type: `string`, Format: `uuid`
- **objectId** (Path, required)  
  - Type: `string`, Format: `uuid`

#### Responses

- **200 OK**

---

## Components

### Schemas

#### AccessTokenResponse

```json
{
  "required": ["accessToken", "expiresIn", "refreshToken"],
  "type": "object",
  "properties": {
    "tokenType": { "type": "string", "nullable": true },
    "accessToken": { "type": "string" },
    "expiresIn": { "type": "integer", "format": "int64" },
    "refreshToken": { "type": "string" }
  }
}
```

#### LoginRequest

```json
{
  "required": ["usernameOrEmail", "password"],
  "type": "object",
  "properties": {
    "usernameOrEmail": { "type": "string" },
    "password": { "type": "string" }
  }
}
```

#### RegisterRequest

```json
{
  "required": ["email", "username", "password"],
  "type": "object",
  "properties": {
    "email": { "type": "string" },
    "username": { "type": "string" },
    "password": { "type": "string" }
  }
}
```

#### Environment2D

```json
{
  "required": ["name", "maxLength", "maxHeight"],
  "type": "object",
  "properties": {
    "id": { "type": "string", "format": "uuid" },
    "name": { "type": "string", "nullable": true },
    "ownerUserId": { "type": "string", "nullable": true },
    "maxLength": { "maximum": 200, "minimum": 20, "type": "integer", "format": "int32" },
    "maxHeight": { "maximum": 100, "minimum": 10, "type": "integer", "format": "int32" }
  }
}
```

#### Object2D

```json
{
  "required": ["name", "environmentId", "objectType", "imageId"],
  "type": "object",
  "properties": {
  "environmentId": { "type": "string", "format": "uuid" },
  "prefabId": { "type": "string", },
  "positionX": { "type": "float", "format": "float" },
  "positionY": { "type": "float", "format": "float" },
  "scaleX": { "type": "float", "format": "float" },
  "scaleY": { "type": "float", "format": "float" },
  "rotationZ": { "type": "float", "format": "float" },
  "sortingLayer": { "type": "integer", "format": "int32" },
  "id": { "type": "string", "format": "uuid" },
  }
}
```

### Security Schemes

#### BearerAuth

```json
{
  "type": "http",
  "scheme": "bearer",
  "bearerFormat": "JWT"
}
```
