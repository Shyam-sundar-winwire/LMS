import { Alert } from "@mui/material";
import { useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import loginModalSideImage from "../assets/login-modal-side.jpg";
import "./LoginPage.css";

export const LoginPage = () => {
  const navigate = useNavigate();
  const { login, loading } = useAuth();
  const [form, setForm] = useState({ email: "", password: "" });
  const [error, setError] = useState("");
  const [isOpen, setIsOpen] = useState(false);
  const wheelIntentRef = useRef(0);
  const lastScrollYRef = useRef(0);

  const openLoginModal = () => {
    setIsOpen(true);
  };

  useEffect(() => {
    const handleScroll = () => {
      const currentScrollY = window.scrollY;
      const isScrollingDown = currentScrollY > lastScrollYRef.current;

      lastScrollYRef.current = currentScrollY;

      if (isScrollingDown && currentScrollY > window.innerHeight * 0.42 && !isOpen) {
        openLoginModal();
      }
    };

    const handleWheel = (event) => {
      if (isOpen || event.deltaY <= 0) {
        return;
      }

      wheelIntentRef.current += event.deltaY;

      if (wheelIntentRef.current > 320) {
        wheelIntentRef.current = 0;
        openLoginModal();
      }
    };

    window.addEventListener("scroll", handleScroll, { passive: true });
    window.addEventListener("wheel", handleWheel, { passive: true });
    return () => {
      window.removeEventListener("scroll", handleScroll);
      window.removeEventListener("wheel", handleWheel);
    };
  }, [isOpen]);

  useEffect(() => {
    document.body.style.overflow = isOpen ? "hidden" : "initial";

    const handleKeyDown = (event) => {
      if (event.key === "Escape") {
        setIsOpen(false);
      }

      if (event.key === "Enter" && !isOpen) {
        event.preventDefault();
        openLoginModal();
      }
    };

    document.addEventListener("keydown", handleKeyDown);
    return () => {
      document.body.style.overflow = "initial";
      document.removeEventListener("keydown", handleKeyDown);
    };
  }, [isOpen]);

  const handleSubmit = async (event) => {
    event.preventDefault();
    setError("");

    try {
      await login(form.email, form.password);
      navigate("/", { replace: true });
    } catch (err) {
      setError(err.userMessage || err.response?.data?.message || "Unable to sign in.");
    }
  };

  return (
    <main className="login-modal-page">
      {!isOpen ? (
        <div className="login-scroll-down">
          <span>Leave Management System</span>
        </div>
      ) : null}

      <div className="login-photo-stage" />

      <section className={`login-modal ${isOpen ? "is-open" : ""}`} aria-label="Login modal">
        <div className="login-modal-container">
          <form className="login-modal-left" onSubmit={handleSubmit}>
            <h1 className="login-modal-title">Welcome!</h1>
            <p className="login-modal-desc">Sign in to continue to Leave Management System.</p>

            {error ? (
              <Alert severity="error" className="login-modal-error">
                {error}
              </Alert>
            ) : null}

            <label className="login-input-block" htmlFor="email">
              <span className="login-input-label">Email</span>
              <input
                type="email"
                name="email"
                id="email"
                placeholder="Email"
                value={form.email}
                onChange={(event) => setForm((prev) => ({ ...prev, email: event.target.value }))}
                required
              />
            </label>

            <label className="login-input-block" htmlFor="password">
              <span className="login-input-label">Password</span>
              <input
                type="password"
                name="password"
                id="password"
                placeholder="Password"
                value={form.password}
                onChange={(event) => setForm((prev) => ({ ...prev, password: event.target.value }))}
                required
              />
            </label>

            <div className="login-modal-actions">
              <button className="login-input-button" type="submit" disabled={loading}>
                {loading ? "Signing in..." : "Login"}
              </button>
            </div>
          </form>

          <div className="login-modal-right">
            <img src={loginModalSideImage} alt="" />
          </div>

          <button
            className="login-close-button"
            type="button"
            aria-label="Close login modal"
            onClick={() => setIsOpen(false)}
          >
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 50 50" aria-hidden="true">
              <path d="M 25 3 C 12.86158 3 3 12.86158 3 25 C 3 37.13842 12.86158 47 25 47 C 37.13842 47 47 37.13842 47 25 C 47 12.86158 37.13842 3 25 3 z M 25 5 C 36.05754 5 45 13.94246 45 25 C 45 36.05754 36.05754 45 25 45 C 13.94246 45 5 36.05754 5 25 C 5 13.94246 13.94246 5 25 5 z M 16.990234 15.990234 A 1.0001 1.0001 0 0 0 16.292969 17.707031 L 23.585938 25 L 16.292969 32.292969 A 1.0001 1.0001 0 1 0 17.707031 33.707031 L 25 26.414062 L 32.292969 33.707031 A 1.0001 1.0001 0 1 0 33.707031 32.292969 L 26.414062 25 L 33.707031 17.707031 A 1.0001 1.0001 0 0 0 32.980469 15.990234 A 1.0001 1.0001 0 0 0 32.292969 16.292969 L 25 23.585938 L 17.707031 16.292969 A 1.0001 1.0001 0 0 0 16.990234 15.990234 z" />
            </svg>
          </button>
        </div>

        <button className="login-modal-button" type="button" onClick={openLoginModal}>
          Click here to login
        </button>
      </section>
    </main>
  );
};
