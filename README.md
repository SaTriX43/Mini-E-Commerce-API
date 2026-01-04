# 🛒 Mini E-Commerce API

API REST desarrollada con **ASP.NET Core .NET 8** que simula un sistema completo de ventas online, implementando autenticación segura, manejo de roles, carrito de compras, órdenes y control de stock.

---

## 🚀 Tecnologías
- ASP.NET Core Web API (.NET 8)
- Entity Framework Core
- SQL Server
- JWT + Refresh Tokens
- Arquitectura en capas
- Result Pattern
- Middleware de manejo de errores

---

## 🔐 Autenticación y Seguridad
- Registro y Login
- JWT con Refresh Tokens persistidos en BD
- Logout real
- Roles:
  - **Admin**
  - **User**
- Ownership en recursos sensibles (órdenes)

---

## 🧩 Funcionalidades

### 👤 Usuario
- Ver productos
- Agregar / quitar productos del carrito
- Crear órdenes
- Pagar órdenes (simulación)
- Cancelar órdenes
- Ver historial y detalles de órdenes

### 👑 Admin
- CRUD de productos
- CRUD de categorías
- Ver todas las órdenes
- Ver órdenes por usuario
- Ver detalles de cualquier orden

---

## 📦 Reglas de Negocio
- El stock se valida y descuenta al pagar
- No se permite comprar sin stock
- El total de la orden se calcula en backend
- Una orden solo puede cancelarse si está en estado `Pending`
- El carrito se vacía al pagar la orden

---

## 🏗️ Arquitectura
- Controllers → entrada HTTP
- Services → reglas de negocio
- Repositories → persistencia
- Unidad de Trabajo para manejo transaccional
- DTOs para desacoplar dominio y API

---

## 📌 Estados de Orden
- `Pending`
- `Paid`
- `Cancelled`

---

## 🧪 Proyecto con enfoque educativo
Este proyecto fue desarrollado como parte de un plan de formación Backend .NET, priorizando:
- Buenas prácticas
- Código mantenible
- Lógica real de negocio
- Seguridad

---

## 📄 Autor
**Santiago González**  
Backend .NET Developer (Junior)
