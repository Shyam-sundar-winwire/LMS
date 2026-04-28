import { CircularProgress, Paper, Stack, Typography } from "@mui/material";

export const LoadingScreen = ({ message = "Loading workspace..." }) => (
  <Paper
    sx={{
      minHeight: "42vh",
      display: "grid",
      placeItems: "center",
      backgroundColor: "#ffffff"
    }}
  >
    <Stack alignItems="center" justifyContent="center" spacing={2}>
      <CircularProgress />
      <Typography color="text.secondary">{message}</Typography>
    </Stack>
  </Paper>
);
