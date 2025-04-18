import { useState } from "react";
import axios from "axios";

function Register() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState("");

  const handleRegister = async (e) => {
    e.preventDefault();
    setError("");

    if (password != confirmPassword) {
      setError("Пароли не совпадают");
      return;
    }

    try {
      const response = await axios.post("http://localhost:5055/register", {
        username,
        password,
        passwordConfirmation: confirmPassword,
      });

      console.log(response.data);
      if (response.data.success === true) {
        alert("Регистрация успешна!");
        window.location.href = "/login";
      } else {
        setError("Ошибка регистрации!");
      }
    } catch (err) {
      setError("Ошибка запроса: ", err.message);
    }
  };

  return (
    <div style={{ textAlign: "center" }}>
      <h2>Регистрация</h2>
      <form onSubmit={handleRegister}>
        <input
          type="text"
          placeholder="Username"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
        />
        <br />
        <input
          type="text"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />
        <br />
        <input
          type="text"
          placeholder="Confirm Password"
          value={confirmPassword}
          onChange={(e) => setConfirmPassword(e.target.value)}
        />
        <br />
        {error && <p style={{ color: "red" }}>{error}</p>}
        <button type="submit">Зарегистрироваться</button>
      </form>
    </div>
  );
}

export default Register;
