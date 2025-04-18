import { useEffect, useState } from "react";
import axios from "axios";

function Tasks() {
  const [tasks, setTasks] = useState([]);
  const [error, setError] = useState("");
  const [newTaskTitle, setNewTaskTitle] = useState("");
  const [newTaskIsDone, setNewTaskIsDone] = useState(false);
  const [editingTask, setEditingTask] = useState(null);
  const [editedTitle, setEditedTitle] = useState("");
  const [editedDone, setEditedDone] = useState(false);
  const [filterDone, setFilterDone] = useState("all"); //all, true, false
  const [sortOrder, setSortOrder] = useState("asc"); //asc, desc
  const [searchQuery, setSearchQuery] = useState("");
  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 5;

  useEffect(() => {
    const token = localStorage.getItem("token");

    if (!token) {
      window.location.href = "/login";
      return;
    }

    fetchTasks(currentPage, pageSize, filterDone, sortOrder, searchQuery);
  }, [filterDone, sortOrder, searchQuery, currentPage]);

  const fetchTasks = async (
    page = 1,
    pageSize = 5,
    status = "all",
    sortOrder = "asc",
    searchQuery = ""
  ) => {
    const token = localStorage.getItem("token");

    if (!token) {
      setError("Unauthorized client!");
      return;
    }

    try {
      const response = await axios.get(
        `http://localhost:5055/api/Tasks/get-all?page=${page}&pageSize=${pageSize}&isDone=${status}&sort=${sortOrder}&search=${searchQuery}`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      if (response.data.success) {
        setTasks(response.data.data);
        setError(""); // сбрасываем ошибку
      } else {
        setError("Error getting tasks!");
      }
    } catch (err) {
      setError("Can't get tasks", err);
    }
  };

  const handleAddTask = async (e) => {
    e.preventDefault();
    const token = localStorage.getItem("token");
    if (!token) return setError("Unauthorized!");

    try {
      const response = await axios.post(
        "http://localhost:5055/api/Tasks/add-task",
        {
          title: newTaskTitle,
          isDone: newTaskIsDone,
        },
        {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );
      if (response.status === 201 || response.data.success) {
        setNewTaskTitle("");
        setNewTaskIsDone(false);
        fetchTasks();
        setCurrentPage(1);
        fetchTasks(1, pageSize, filterDone, sortOrder, searchQuery);
      } else {
        setError("Failed to add task");
      }
    } catch (err) {
      setError("Error adding task", err);
    }
  };

  const handleDelete = async (id) => {
    const token = localStorage.getItem("token");

    try {
      const response = await axios.delete(
        `http://localhost:5055/api/Tasks/delete-task/${id}`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      if (response.data.success) {
        setTasks((prev) => prev.filter((t) => t.id !== id));
        setCurrentPage(1);
        fetchTasks(1, pageSize, filterDone, sortOrder, searchQuery);
      } else {
        setError("Delete error");
      }
    } catch (err) {
      setError("Delete request error", err);
    }
  };

  const startEditing = async (task) => {
    setEditingTask(task.id);
    setEditedTitle(task.title);
    setEditedDone(task.isDone);
  };

  const handleUpdate = async () => {
    const token = localStorage.getItem("token");

    try {
      const response = await axios.put(
        `http://localhost:5055/api/Tasks/update-task/`,
        {
          id: editingTask,
          title: editedTitle,
          isDone: editedDone,
        },
        {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-type": "application/json",
          },
        }
      );

      if (response.data.success) {
        setTasks(
          tasks.map((t) =>
            t.id === editingTask
              ? {
                  ...t,
                  title: editedTitle,
                  isDone: editedDone,
                }
              : t
          )
        );
        setEditingTask(null);
      } else {
        setError("Can't to update task");
      }
    } catch (err) {
      setError("Update request error", err);
    }
  };

  const handleLogout = async () => {
    localStorage.removeItem("token");
    localStorage.removeItem("refreshToken");
    window.location.href = "/login";
  };

  const filteredTasks = tasks
    .filter((task) => {
      if (filterDone === "done" && !task.isDone) return false;
      if (filterDone === "notdone" && task.isDone) return false;
      return task.title.toLowerCase().includes(searchQuery.toLowerCase());
    })
    .sort((a, b) => {
      if (sortOrder === "asc") return a.title.localeCompare(b.title);
      return b.title.localeCompare(a.title);
    });

  return (
    <div>
      <h2>Your tasks</h2>
      <div style={{ marginBottom: "20px" }}>
        <label>Фильтр по статусу:</label>
        <select
          value={filterDone}
          onChange={(e) => setFilterDone(e.target.value)}
        >
          <option value="all">Все</option>
          <option value="true">✅ Выполненные</option>
          <option value="false">❌ Невыполненные</option>
        </select>

        <label style={{ marginLeft: "15px" }}>Сортировка:</label>
        <select
          value={sortOrder}
          onChange={(e) => setSortOrder(e.target.value)}
        >
          <option value="asc">A → Z</option>
          <option value="desc">Z → A</option>
        </select>
        <input
          type="text"
          placeholder="Поиск по названию"
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          style={{ marginLeft: "10px" }}
        />
      </div>
      {error && <p style={{ color: "red" }}>{error}</p>}
      <ul>
        {filteredTasks.map((task) =>
          editingTask === task.id ? (
            <div key={task.id}>
              <input
                value={editedTitle}
                onChange={(e) => setEditedTitle(e.target.value)}
              />
              <input
                type="checkbox"
                checked={editedDone}
                onChange={(e) => setEditedDone(e.target.checked)}
              />
              <button onClick={handleUpdate}>💾</button>
              <button onClick={() => setEditingTask(null)}>❌</button>
            </div>
          ) : (
            <li key={task.id}>
              <strong>{task.title}</strong> - {task.isDone ? "✅" : "❌"}
              <button onClick={() => handleDelete(task.id)}>❌</button>
              <button onClick={() => startEditing(task)}>✏️</button>
            </li>
          )
        )}
      </ul>

      <form onSubmit={handleAddTask} style={{ marginTop: "20px" }}>
        <input
          type="text"
          placeholder="New Task"
          value={newTaskTitle}
          onChange={(e) => setNewTaskTitle(e.target.value)}
          required
        />
        <label style={{ marginLeft: "10px" }}>
          Done?
          <input
            type="checkbox"
            checked={newTaskIsDone}
            onChange={(e) => setNewTaskIsDone(e.target.checked)}
          />
        </label>
        <button type="submit" style={{ marginLeft: "10px" }}>
          Add
        </button>
      </form>
      <div style={{ marginTop: "20px" }}>
        <button
          disabled={currentPage === 1}
          onClick={() => setCurrentPage((prev) => prev - 1)}
        >
          ← Назад
        </button>
        <span style={{ margin: "0 10px" }}>Страница: {currentPage}</span>
        <button onClick={() => setCurrentPage((prev) => prev + 1)}>
          Вперёд →
        </button>
      </div>
      <div>
        <button
          onClick={handleLogout}
          style={{ textAlign: "center", marginTop: "20px" }}
        >
          Выйти
        </button>
      </div>
    </div>
  );
}

export default Tasks;
