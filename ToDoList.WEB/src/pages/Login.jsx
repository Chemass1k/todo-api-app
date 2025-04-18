import { useState } from "react";
import axios from "axios";
import { Link } from "react-router-dom";

function Login() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleLogin = async (e) => {
    e.preventDefault();
    setError("");

    try {
      const response = await axios.post(
        "http://localhost:5055/login",
        {
          username,
          password,
        },
        {
          headers: {
            "Content-Type": "application/json",
          },
        }
      );

      console.log(response.data.data);
      const { token, refreshToken } = response.data.data;

      // Сохраняем токены
      localStorage.setItem("token", token);
      localStorage.setItem("refreshToken", refreshToken);

      // Перенаправляем (если используешь React Router)
      window.location.href = "/tasks";
    } catch (err) {
      setError("Неверный логин или пароль", err);
    }
  };

  return (
    <div>
      <form onSubmit={handleLogin}>
        <h2>Вход</h2>
        <input
          type="text"
          placeholder="Имя пользователя"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          required
        />
        <input
          type="password"
          placeholder="Пароль"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
        <button type="submit">Войти</button>
        {error && <p style={{ color: "red" }}>{error}</p>}
      </form>
      <div>
        <p style={{ marginTop: "10px" }}>
          Нет аккаунта? <Link to="/register">Зарегистрируйтесь</Link>
        </p>
      </div>
    </div>
  );
}

export default Login;
