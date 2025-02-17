# Functional Requirements

## Must Have

-   **User Registration**

    -   User can register with a unique username and password.
    -   Password requirements:
        -   Minimum 10 characters.
        -   Must contain at least one lowercase, one uppercase, one numeric, and one non-alphanumeric character.

-   **User Login**

    -   Users can log in with a username and password.
    -   Error message is displayed when credentials are incorrect.

-   **Create a New 2D World**

    -   User must be logged in.
    -   User provides a name for the world.
    -   The world name must be unique for the user.
    -   Name length: 1-25 characters.
    -   Maximum 5 personal worlds per user.
    -   The new world is saved.

-   **View Existing 2D Worlds**

    -   User must be logged in.
    -   The list displays the user's created 2D worlds.

-   **View a Specific 2D World**

    -   User must be logged in.
    -   Objects within the world are displayed correctly.
    -   Users can only view their own worlds.

-   **Add a 2D Object**

    -   User must be logged in.
    -   At least 3 object types to choose from.
    -   The object is saved.

-   **Delete a 2D World**
    -   User must be logged in.
    -   Associated objects are also deleted.

## Should Have

-   **Modify Object Properties**

    -   Users can modify position, rotation, and scale.
    -   Changes are displayed and saved.

-   **Delete a 2D Object**

    -   User must be logged in.
    -   Object is removed from the view and saved.

-   **Move Camera in 2D World**

    -   Users can move the camera using arrow keys.

-   **Set World Size on Creation**

    -   Length (X): 20-200.
    -   Height (Y): 10-100.
    -   Objects must stay within these bounds.

-   **Share a 2D World**

    -   Users can share a world with another user via username.
    -   No feedback on username existence to prevent user enumeration.

-   **Object Animations**
    -   Objects animate when placed in a world.
    -   Objects animate on click.

## Nice to Have

-   **Save Actions Manually**
    -   Created, modified, and deleted objects are saved only when the user clicks "Save".
