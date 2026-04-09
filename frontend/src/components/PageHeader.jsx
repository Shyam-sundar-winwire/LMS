import { Chip, Stack, Typography } from "@mui/material";

export const PageHeader = ({ eyebrow, title, subtitle, chips = [] }) => (
  <Stack spacing={1.25}>
    {eyebrow ? (
      <Typography variant="overline" color="primary.main">
        {eyebrow}
      </Typography>
    ) : null}
    <Typography variant="h4">{title}</Typography>
    {subtitle ? (
      <Typography color="text.secondary" sx={{ maxWidth: 760 }}>
        {subtitle}
      </Typography>
    ) : null}
    {chips.length ? (
      <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap>
        {chips.map((chip) => (
          <Chip key={chip} label={chip} size="small" />
        ))}
      </Stack>
    ) : null}
  </Stack>
);
