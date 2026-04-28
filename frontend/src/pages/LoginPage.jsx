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
    <Box
      sx={{
        minHeight: "100vh",
        display: "flex",
        alignItems: "center",
        py: { xs: 3, md: 5 },
        background: "radial-gradient(circle at top, rgba(47, 109, 246, 0.08), transparent 28%)"
      }}
    >
      <Container maxWidth="sm">
        <Paper
          sx={{
            px: { xs: 3, md: 4 },
            py: { xs: 3.5, md: 4 },
            borderRadius: 4,
            maxWidth: 460,
            mx: "auto",
            backgroundColor: "#ffffff"
          }}
        >
          <Stack spacing={3}>
            <Stack spacing={0.75} alignItems="center" textAlign="center">
              <Typography variant="h4">Sign in</Typography>
            </Stack>

            <Stack component="form" spacing={2.25} onSubmit={handleSubmit}>
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
          </Stack>
        </Paper>
      </Container>
    </Box>
  );
};
