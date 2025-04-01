# Entity Relationship Diagram

```mermaid
erDiagram
    DOCTOR {
        NVARCHAR FirstName
        NVARCHAR LastName
        NVARCHAR Specialization
    }
    ASPNETUSERS {
        NVARCHAR UserName
        NVARCHAR NormalizedUserName
        NVARCHAR Email
        NVARCHAR NormalizedEmail
        BIT EmailConfirmed
        NVARCHAR PasswordHash
        NVARCHAR SecurityStamp
        NVARCHAR ConcurrencyStamp
        NVARCHAR PhoneNumber
        BIT PhoneNumberConfirmed
        BIT TwoFactorEnabled
        DATETIMEOFFSET LockoutEnd
        BIT LockoutEnabled
        INT AccessFailedCount
    }
    GUARDIAN {
        NVARCHAR FirstName
        NVARCHAR LastName
        NVARCHAR UserID
    }
    TREATMENT {
        NVARCHAR Name
    }
    PATIENT {
        NVARCHAR FirstName
        NVARCHAR LastName
        UUID GuardianID
        UUID TreatmentID
        UUID DoctorID
        NVARCHAR Avatar
    }
    JOURNALENTRY {
        UUID PatientID
        UUID GuardianID
        DATETIME Date
        NVARCHAR Content
    }
    APPOINMENT {
        NVARCHAR Name
        NVARCHAR Url
        NVARCHAR Description
        VARBINARY Image
        INT DurationInMinutes
    }
    STICKER {
        NVARCHAR Name
    }
    ASPNETROLES {
        NVARCHAR Name
        NVARCHAR NormalizedName
        NVARCHAR ConcurrencyStamp
    }
    ASPNETUSERCLAIMS {
        NVARCHAR UserId
        NVARCHAR ClaimType
        NVARCHAR ClaimValue
    }
    ASPNETROLECLAIMS {
        NVARCHAR RoleId
        NVARCHAR ClaimType
        NVARCHAR ClaimValue
    }

    %% Direct (One-to-Many) Relationships
    GUARDIAN }o--|| ASPNETUSERS : "belongs to"
    PATIENT }o--|| GUARDIAN : "has"
    PATIENT }o--|| TREATMENT : "assigned to"
    PATIENT }o--|| DOCTOR : "treated by"
    JOURNALENTRY }o--|| PATIENT : "refers to"
    JOURNALENTRY }o--|| GUARDIAN : "created by"
    
    %% Many-to-Many Relationships (instead of join tables)
    TREATMENT }|--|{ APPOINMENT : "schedules"
    PATIENT }|--|{ APPOINMENT : "completes"
    PATIENT }|--|{ STICKER : "owns"
    ASPNETUSERS }|--|{ ASPNETROLES : "has roles"
    
    %% Claims Relationships
    ASPNETUSERCLAIMS }o--|| ASPNETUSERS : "of"
    ASPNETROLECLAIMS }o--|| ASPNETROLES : "of"
