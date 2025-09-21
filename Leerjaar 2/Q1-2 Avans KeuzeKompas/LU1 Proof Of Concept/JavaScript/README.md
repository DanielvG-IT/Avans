# Project JavaScript Sakila

## What it is

Full‑stack Node.js + Express web app that uses Handlebars for server-rendered views and MySQL (Sakila‑like schema) for data. Dark‑themed Bootstrap 5 UI with pagination, filters and full CRUD flows. Dockerized for production; includes scripts for development and Cypress end‑to‑end testing.

## Public site

-   Home (/): three server‑provided carousels — Popular, Longest, and Cheapest movies.
-   Movies list (/movies): search, filter by category/rating, sort (title/year/length/rate), and pagination.
-   Movie details (/movies/:id) with optional JSON output.
-   About page (/about).

## Authentication & account

-   JWT authentication with httpOnly cookies, refresh token rotation, SameSite=strict, secure in non‑dev environments.
-   Login (/auth/login), Register (/auth/register), Token refresh (/auth/token).
-   Profile (/auth/profile): upload/preview/delete avatar (multipart via multer), reset password, logout, delete account.

## Customer features

-   Dashboard (/customer): customer details and rentals grouped as Active, Future, Past.
-   Rental metrics: totals, active count, overdue count.
-   Edit profile (/customer/edit): name, contact, address, store, active flag.

## Staff features

-   Staff overview (/staff) and dashboard (/staff/dashboard).
-   CRM (/staff/crm): search/filter customers by status/store, sort by last name/create date, compact pagination window with “…” gaps.
-   Create/edit customer flows (/staff/crm/new, /staff/crm/:customerId/edit) and detailed customer view with rental breakdown.
-   Movie management: create/edit movies (/movies/new, /movies/:id/edit).
-   Rent flows (staff only): start rental from customer or movie contexts; create rental API (/rentals/new).
-   JSON endpoints for typeahead and quick selects (movie/customer search).

## Middleware & security

-   Role guards: optionalCustomerAuthWeb, requireCustomerAuthWeb, requireStaffAuthWeb (redirects), requireCustomerAuthApi, requireStaffAuthApi (JSON 401/403).
-   Refresh token endpoint issues new access tokens when valid.
-   Structured error handling and logging via winston; friendly error page.

## Data & services

-   Layered DAO/service access (mysql2) for movies, customers, rentals, stores, staff, etc.
-   Avatar storage encoded in DB (base64 + format) and image helper utilities.

## Testing, tooling & deployment

-   Cypress E2E test(s) (notably Customer Edit flow).
-   Nodemon for dev; npm scripts for start/dev/test.
-   Dockerfile for production image (non‑root user, npm ci, port 3000).

## Notes & UX

-   Movie listing TODO: parity in sort rendering.
-   Customer forms normalize mixed key shapes (snake/camel).
-   Register requires store selection and ToS acceptance; staff actions enforced server‑side.

# LICENSE

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.
