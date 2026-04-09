import TrendingUpRoundedIcon from "@mui/icons-material/TrendingUpRounded";
import { Paper, Stack, Typography } from "@mui/material";

export const StatCard = ({ label, value, helper, accent }) => (
  <Paper
    sx={{
      p: 3,
      borderRadius: 6,
      overflow: "hidden",
      position: "relative",
      background: `linear-gradient(160deg, rgba(${accent}, 0.22), rgba(255,255,255,0.92))`,
      transition: "transform 200ms ease, box-shadow 200ms ease",
      "&::before": {
        content: '""',
        position: "absolute",
        inset: 0,
        background: "linear-gradient(180deg, rgba(255,255,255,0.22), transparent)"
      },
      "&:hover": {
        transform: "translateY(-4px)",
        boxShadow: "0 30px 50px rgba(15, 23, 42, 0.12)"
      }
    }}
  >
    <Stack spacing={1.25} sx={{ position: "relative" }}>
      <TrendingUpRoundedIcon sx={{ color: "primary.main" }} />
      <Typography color="text.secondary">{label}</Typography>
      <Typography variant="h4">{value}</Typography>
      <Typography variant="body2" color="text.secondary">
        {helper}
      </Typography>
    </Stack>
  </Paper>
);
