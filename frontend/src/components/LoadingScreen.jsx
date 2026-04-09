import { Stack, CircularProgress, Typography } from "@mui/material";

export const LoadingScreen = ({ message = "Loading workspace..." }) => (
  <Stack alignItems="center" justifyContent="center" spacing={2} sx={{ minHeight: "40vh" }}>
    <CircularProgress />
    <Typography color="text.secondary">{message}</Typography>
  </Stack>
);
