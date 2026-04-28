import { Paper, Stack, Typography } from "@mui/material";

export const SectionCard = ({ title, subtitle, children, action }) => (
  <Paper
    sx={{
      overflow: "hidden",
      p: { xs: 2, md: 2.25 },
      backgroundColor: "#ffffff"
    }}
  >
    <Stack spacing={1.75}>
      <Stack
        direction={{ xs: "column", sm: "row" }}
        justifyContent="space-between"
        alignItems={{ xs: "flex-start", sm: "center" }}
        spacing={1}
      >
        <Stack spacing={0.5}>
          <Typography variant="h6" sx={{ letterSpacing: "-0.02em" }}>
            {title}
          </Typography>
          {subtitle ? <Typography color="text.secondary" sx={{ maxWidth: 760 }}>{subtitle}</Typography> : null}
        </Stack>
        {action}
      </Stack>
      {children}
    </Stack>
  </Paper>
);
