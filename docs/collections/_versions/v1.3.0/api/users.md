---
title: Users
nav:
  parent: API
  order: 4
  icon: 👥
---

The User Management API becomes available only after you enable the Service Module. Along with this, the Authentication API endpoint will also be accessible.

## 🔑 Authentication API
The Authentication API is used to authenticate users and obtain a JWT token for accessing protected endpoints.

- `POST /api/auth/token` – Authenticates a user using email and password, returning a JWT token.<br>
  The token must be included in the Authorization header as a Bearer token for API requests.

## 👥 User Management API (Admin Only)
Admin-only endpoints allow you to manage users, including creating, updating, and deleting users.


- `POST /api/users` – Create a new user.
- `PUT /api/users/{email}` – Promote or demote a user, and make them active or inactive.
- `PATCH /api/users/{email}` – Change a user’s password.
- `DELETE /api/users/{email}` – Soft-delete a user (users remain in the database but cannot be restored).

---

## 🔑 User Management Rules
- ✔️ Admin users are the only ones who can manage other users, including creation, deletion, activation, deactivation, and promotion/demotion.
- ✔️ Only active users can interact with the API.
- ✔️ Soft-delete ensures the user remains in the database but cannot interact with the API. Once soft-deleted, the user cannot be restored.
- ✔️ A new user with the same email can be created after a soft-deletion.

--- 

## 📧 Email and Password Requirements
To maintain security and consistency, the following requirements must be met:

- **Email**: Must be a valid email format. It will not be used for communication or marketing purposes.
- **Password**: Must be at least 10 characters long.

---

## 🔒 Recommended Practice: Creating a New Admin
For enhanced security, follow these steps:

1. **Create a New Admin User**: After the Service Module is configured and the default admin is set, create a new user account and promote it to admin status.
2. **Deactivate the Default Admin**: Once a new admin is established, deactivate the default admin account. This reduces the risk of exposing default credentials and enhances system security.
