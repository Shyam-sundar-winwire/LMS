import { Button, Stack, Typography } from "@mui/material";
import { Link } from "react-router-dom";

export const NotFoundPage = () => (
  <Stack spacing={2} alignItems="flex-start">
    <Typography variant="h4">Page not found</Typography>
    <Typography color="text.secondary">The page you requested does not exist or you may not have access to it.</Typography>
    <Button component={Link} to="/" variant="contained">
      Back to dashboard
    </Button>
  </Stack>
);
