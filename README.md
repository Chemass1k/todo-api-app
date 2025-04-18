# 📝 ToDoList — Full Stack Task Management App

## 📌 Описание

**ToDoList** — это полнофункциональное веб-приложение для управления задачами. Бэкенд написан на **ASP.NET Core Web API**, фронтенд — на **React**. Приложение поддерживает регистрацию, авторизацию по JWT, работу с задачами (создание, редактирование, удаление), фильтрацию, поиск, пагинацию и глобальную обработку ошибок.

---

## 🧱 Технологии

### Backend:

- ASP.NET Core 8 Web API
- Entity Framework Core
- SQL Server
- JWT Authorization (Access/Refresh токены)
- Serilog
- Middleware для глобальной обработки ошибок
- Архитектура: **DAL → BAL → API**

### Frontend:

- React
- Axios
- useState / useEffect
- CRUD-интерфейс + поиск + фильтрация + пагинация

---

## 🚀 Возможности

- ✅ Регистрация и вход с JWT
- ✅ Создание / редактирование / удаление задач
- ✅ Фильтрация задач по статусу
- ✅ Поиск задач по названию
- ✅ Сортировка A→Z / Z→A
- ✅ Пагинация
- ✅ Обработка ошибок на сервере
- ✅ Хранение токенов в localStorage

---

## 🛠️ Установка и запуск

### Backend:

bash:

- cd ToDoList.API
- dotnet ef database update
- dotnet run

### Frontend:

bash:

- cd ToDoList.WEB
- npm install
- npm run dev

## 📁 Структура проекта

ToDoList
│
├── ToDoList.API # ASP.NET Core Web API
├── ToDoList.BAL # Business Logic
├── ToDoList.DAL # Data Access Layer
└── ToDoList.WEB # React Frontend

👤 Автор
Andrii Chemnyi

- LinkedIn: www.linkedin.com/in/andrey-chemnyi
- GitHub: https://github.com/Chemass1k
- Email: andrew.codemaster@gmail.com

📦 Лицензия
Проект открыт для изучения и личного использования.
