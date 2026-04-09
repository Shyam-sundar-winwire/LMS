import LoginRoundedIcon from "@mui/icons-material/LoginRounded";
import {
  Alert,
  Box,
  Button,
  Container,
  Grid,
  Paper,
  Stack,
  TextField,
  Typography
} from "@mui/material";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

const demoAccounts = [
  { label: "Admin", email: "admin@leaveapp.com", password: "Admin@123" },
  { label: "HR", email: "hr@leaveapp.com", password: "HR@123" },
  { label: "Manager", email: "manager@leaveapp.com", password: "Manager@123" },
  { label: "Employee", email: "employee@leaveapp.com", password: "Employee@123" }
];

export const LoginPage = () => {
  const navigate = useNavigate();
  const { login, loading } = useAuth();
  const [form, setForm] = useState({ email: demoAccounts[3].email, password: demoAccounts[3].password });
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
      <Container maxWidth="lg">
        <Grid container spacing={4} alignItems="center">
          <Grid item xs={12} md={6}>
            <Stack spacing={3}>
              <Typography variant="h3">Leave Management System</Typography>
              <Grid container spacing={2}>
                {demoAccounts.map((account) => (
                  <Grid item xs={12} sm={6} key={account.label}>
                    <Paper
                      sx={{
                        p: 2.25,
                        cursor: "pointer",
                        transition: "transform 180ms ease, box-shadow 180ms ease",
                        "&:hover": {
                          transform: "translateY(-3px)"
                        }
                      }}
                      onClick={() => setForm({ email: account.email, password: account.password })}
                    >
                      <Typography fontWeight={700}>{account.label}</Typography>
                      <Typography variant="body2" color="text.secondary">
                        {account.email}
                      </Typography>
                    </Paper>
                  </Grid>
                ))}
              </Grid>
            </Stack>
          </Grid>

          <Grid item xs={12} md={6}>
            <Paper
              sx={{
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
          </Grid>
        </Grid>
      </Container>
    </Box>
  );
};
