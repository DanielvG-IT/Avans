# Relational Database Design

```mermaid
erDiagram
    DOCTOR {
        UUID ID PK
        string FirstName
        string LastName
        string Specialization
    }
    ASPNETUSERS {
        string Id PK
        string UserName
        string NormalizedUserName
        string Email
        string NormalizedEmail
        boolean EmailConfirmed
        string PasswordHash
        string SecurityStamp
        string ConcurrencyStamp
        string PhoneNumber
        boolean PhoneNumberConfirmed
        boolean TwoFactorEnabled
        datetimeoffset LockoutEnd
        boolean LockoutEnabled
        int AccessFailedCount
    }
    GUARDIAN {
        UUID ID PK
        string FirstName
        string LastName
        string UserID FK
    }
    TREATMENT {
        UUID ID PK
        string Name
    }
    PATIENT {
        UUID ID PK
        string FirstName
        string LastName
        UUID GuardianID FK
        UUID TreatmentID FK
        UUID DoctorID FK
        string Avatar
    }
    JOURNALENTRY {
        UUID ID PK
        UUID PatientID FK
        UUID GuardianID FK
        datetime Date
        string Content
    }
    APPOINMENT {
        UUID ID PK
        string Name
        string Url
        blob Image
        int DurationInMinutes
    }
    TREATMENT_APPOINMENT {
        UUID TreatmentID PK, FK
        UUID AppoinmentID PK, FK
        int Sequence
    }
    STICKER {
        UUID ID PK
        string Name
    }
    STICKERCOLLECTION {
        UUID ID PK
        UUID PatientID FK
        UUID StickerID FK
        datetime UnlockedDate
    }
    COMPLETEDAPPOINTMENTS {
        UUID ID PK
        UUID PatientID FK
        UUID AppointmentID FK
        datetime CompletedDate
    }
    ASPNETROLES {
        string Id PK
        string Name
        string NormalizedName
        string ConcurrencyStamp
    }
    ASPNETUSERCLAIMS {
        int Id PK
        string UserId FK
        string ClaimType
        string ClaimValue
    }
    ASPNETROLECLAIMS {
        int Id PK
        string RoleId FK
        string ClaimType
        string ClaimValue
    }
    ASPNETUSERROLES {
        string UserId PK, FK
        string RoleId PK, FK
    }

    %% One-to-Many Relationships
    GUARDIAN ||--|| ASPNETUSERS : "belongs to"
    PATIENT ||--|| GUARDIAN : "has guardian"
    PATIENT ||--|| TREATMENT : "receives treatment"
    PATIENT ||--|| DOCTOR : "is treated by"
    JOURNALENTRY ||--|| PATIENT : "is about"
    JOURNALENTRY ||--|| GUARDIAN : "is recorded by"
    COMPLETEDAPPOINTMENTS ||--|| PATIENT : "completed by"
    COMPLETEDAPPOINTMENTS ||--|| APPOINMENT : "completes appointment"
    STICKERCOLLECTION ||--|| PATIENT : "belongs to"
    STICKERCOLLECTION ||--|| STICKER : "can unlock"

    %% Many-to-Many Relationships via Join Tables
    TREATMENT ||--o{ TREATMENT_APPOINMENT : "has"
    APPOINMENT ||--o{ TREATMENT_APPOINMENT : "has"
    ASPNETUSERS ||--o{ ASPNETUSERCLAIMS : "has"
    ASPNETROLES ||--o{ ASPNETROLECLAIMS : "has"
    ASPNETUSERS ||--o{ ASPNETUSERROLES : "assigned to"
    ASPNETROLES ||--o{ ASPNETUSERROLES : "assigned to"
