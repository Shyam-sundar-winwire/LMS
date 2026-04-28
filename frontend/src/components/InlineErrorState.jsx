import { Alert, Button, Stack } from "@mui/material";

export const InlineErrorState = ({ message, actionLabel = "Try again", onRetry }) => (
  <Stack spacing={2} sx={{ maxWidth: 560 }}>
    <Alert severity="error" sx={{ borderRadius: 4 }}>{message}</Alert>
    {onRetry ? (
      <Button variant="outlined" onClick={onRetry} sx={{ alignSelf: "flex-start" }}>
        {actionLabel}
      </Button>
    ) : null}
  </Stack>
);
