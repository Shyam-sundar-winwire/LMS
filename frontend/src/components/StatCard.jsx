import TrendingUpRoundedIcon from "@mui/icons-material/TrendingUpRounded";
import { Box, Paper, Stack, Typography } from "@mui/material";

export const StatCard = ({ label, value, helper, accent }) => (
  <Paper
    sx={{
      p: 2,
      borderRadius: 4,
      minHeight: "100%",
      backgroundColor: "#ffffff",
      borderTop: `3px solid rgba(${accent}, 0.85)`
    }}
  >
    <Stack spacing={1}>
      <Box
        sx={{
          width: 34,
          height: 34,
          display: "grid",
          placeItems: "center",
          borderRadius: 2.5,
          backgroundColor: `rgba(${accent}, 0.16)`
        }}
      >
        <TrendingUpRoundedIcon sx={{ color: `rgba(${accent}, 1)`, fontSize: 18 }} />
      </Box>
      <Typography color="text.secondary" sx={{ fontWeight: 600 }}>
        {label}
      </Typography>
      <Typography variant="h5" sx={{ letterSpacing: "-0.03em" }}>{value}</Typography>
      <Typography variant="body2" color="text.secondary">
        {helper}
      </Typography>
    </Stack>
  </Paper>
);
