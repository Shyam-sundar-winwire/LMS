import InsertChartOutlinedRoundedIcon from "@mui/icons-material/InsertChartOutlinedRounded";
import { Box, Paper, Stack, Typography } from "@mui/material";

export const EmptyState = ({ title, description }) => (
  <Paper
    sx={{
      p: 3.5,
      textAlign: "center",
      backgroundColor: "#ffffff"
    }}
  >
    <Stack spacing={1.5} alignItems="center">
      <Box
        sx={{
          width: 56,
          height: 56,
          display: "grid",
          placeItems: "center",
          borderRadius: "50%",
          backgroundColor: "rgba(47, 109, 246, 0.08)"
        }}
      >
        <InsertChartOutlinedRoundedIcon color="primary" />
      </Box>
      <Typography variant="h6">{title}</Typography>
      <Typography color="text.secondary" sx={{ maxWidth: 460 }}>
        {description}
      </Typography>
    </Stack>
  </Paper>
);
