import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Login from "./pages/Login";
import Tasks from "./pages/Task";
import Register from "./pages/Register";

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/tasks" element={<Tasks />} />
        <Route path="/register" element={<Register />} />
        <Route path="*" element={<Login />} /> {/* всё остальное — Login */}
      </Routes>
    </Router>
  );
}

export default App;
