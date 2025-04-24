# 🚀 TrainX API

Welcome to the **TrainX API**, a robust backend solution for handling attachments, encryption, token management, and more. This API is designed to seamlessly integrate with Flutter or any client-side application, providing reliable and scalable functionality.

---

---

## ✨ Features

- **File Handling**: Upload, download, and manage attachments dynamically.
- **Encryption/Decryption**: Robust algorithms for secure data handling.
- **Token Management**: Generate, validate, and decode JWT tokens.
- **Serialization**: Convert objects to JSON strings and vice versa.
- **Scalable Architecture**: Easily extendable for additional features.
- **Swagger Integration**: Comprehensive API documentation.

---

## 🛠 Endpoints

### Attachments
- `POST /api/attachments/upload` - Upload files.
- `GET /api/attachments/download/{filename}` - Download files.
- `DELETE /api/attachments/delete/{filename}` - Delete files.

### Encryption
- `POST /api/encryption/encrypt` - Encrypt data with dynamic key generation.
- `POST /api/encryption/decrypt` - Decrypt data with the provided key.

### Token Management
- `POST /api/token/generate` - Generate a new JWT token.
- `POST /api/token/validate` - Validate a given JWT token.
- `POST /api/token/decode` - Decode a JWT token.

---

## 🚀 Installation

Follow these steps to set up the project locally:

1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/trainx-api.git
