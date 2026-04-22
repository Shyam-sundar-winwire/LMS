import LoginRoundedIcon from "@mui/icons-material/LoginRounded";
import {
  Alert,
  Box,
  Button,
  Container,
  Paper,
  Stack,
  TextField,
  Typography
} from "@mui/material";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export const LoginPage = () => {
  const navigate = useNavigate();
  const { login, loading } = useAuth();
  const [form, setForm] = useState({ email: "", password: "" });
  const [error, setError] = useState("");

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
    <Box sx={{ minHeight: "100vh", display: "flex", alignItems: "center", py: 4 }}>
      <Container maxWidth="sm">
        <Stack spacing={3} alignItems="center">
          <Typography variant="h3" textAlign="center">
            Leave Management System
          </Typography>
          <Paper
            sx={{
              width: "100%",
              p: { xs: 3, md: 4 },
              background: "linear-gradient(180deg, rgba(255,255,255,0.94), rgba(255,255,255,0.78))"
            }}
          >
            <Stack component="form" spacing={2.5} onSubmit={handleSubmit}>
              <Typography variant="h5">Sign in</Typography>
              {error ? <Alert severity="error">{error}</Alert> : null}
              <TextField
                label="Email"
                type="email"
                value={form.email}
                onChange={(event) => setForm((prev) => ({ ...prev, email: event.target.value }))}
                fullWidth
              />
              <TextField
                label="Password"
                type="password"
                value={form.password}
                onChange={(event) => setForm((prev) => ({ ...prev, password: event.target.value }))}
                fullWidth
              />
              <Button type="submit" variant="contained" size="large" startIcon={<LoginRoundedIcon />} disabled={loading}>
                {loading ? "Signing in..." : "Continue"}
              </Button>
            </Stack>
          </Paper>
        </Stack>
      </Container>
    </Box>
  );
};
