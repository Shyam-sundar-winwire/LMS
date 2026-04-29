import { CircularProgress, Paper, Stack, Typography } from "@mui/material";

export const LoadingScreen = ({ message = "Loading workspace..." }) => (
  <Paper
    sx={{
      minHeight: "42vh",
      display: "grid",
      placeItems: "center",
      backgroundColor: "rgba(255, 255, 255, 0.86)",
      backdropFilter: "blur(12px)"
    }}
  >
    <Stack alignItems="center" justifyContent="center" spacing={2}>
      <CircularProgress />
      <Typography color="text.secondary">{message}</Typography>
    </Stack>
  </Paper>
);
