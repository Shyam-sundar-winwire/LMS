import { Chip, Stack, Typography } from "@mui/material";

export const PageHeader = ({ eyebrow, title, subtitle, chips = [] }) => (
  <Stack spacing={0.65} sx={{ mb: 0.25 }}>
    {eyebrow ? (
      <Typography variant="overline" color="text.secondary" sx={{ letterSpacing: "0.12em", fontWeight: 700 }}>
        {eyebrow}
      </Typography>
    ) : null}
    <Typography variant="h4" sx={{ maxWidth: 900, letterSpacing: "-0.03em" }}>
      {title}
    </Typography>
    {subtitle ? (
      <Typography color="text.secondary" sx={{ maxWidth: 760, fontSize: { xs: "0.9rem", md: "0.93rem" } }}>
        {subtitle}
      </Typography>
    ) : null}
    {chips.length ? (
      <Stack direction="row" spacing={0.75} flexWrap="wrap" useFlexGap>
        {chips.map((chip) => (
          <Chip
            key={chip}
            label={chip}
            size="small"
            variant="outlined"
            sx={{ backgroundColor: "rgba(255,255,255,0.72)", borderColor: "rgba(162, 179, 201, 0.24)" }}
          />
        ))}
      </Stack>
    ) : null}
  </Stack>
);
