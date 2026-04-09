import { Paper, Stack, Typography } from "@mui/material";

export const EmptyState = ({ title, description }) => (
  <Paper sx={{ p: 4 }}>
    <Stack spacing={1}>
      <Typography variant="h6">{title}</Typography>
      <Typography color="text.secondary">{description}</Typography>
    </Stack>
  </Paper>
);
