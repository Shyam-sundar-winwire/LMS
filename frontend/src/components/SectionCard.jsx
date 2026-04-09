import { Paper, Stack, Typography } from "@mui/material";

export const SectionCard = ({ title, subtitle, children, action }) => (
  <Paper
    sx={{
      p: { xs: 2.5, md: 3 },
      background: "linear-gradient(180deg, rgba(255,255,255,0.92), rgba(255,255,255,0.76))",
      transition: "transform 220ms ease, box-shadow 220ms ease",
      "&:hover": {
        transform: "translateY(-2px)",
        boxShadow: "0 30px 60px rgba(15, 23, 42, 0.1)"
      }
    }}
  >
    <Stack spacing={2.5}>
      <Stack
        direction={{ xs: "column", sm: "row" }}
        justifyContent="space-between"
        alignItems={{ xs: "flex-start", sm: "center" }}
        spacing={1.5}
      >
        <Stack spacing={0.5}>
          <Typography variant="h6">{title}</Typography>
          {subtitle ? <Typography color="text.secondary">{subtitle}</Typography> : null}
        </Stack>
        {action}
      </Stack>
      {children}
    </Stack>
  </Paper>
);
