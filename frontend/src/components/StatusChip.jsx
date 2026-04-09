import { Chip } from "@mui/material";
import { STATUS_COLORS } from "../utils/constants";

export const StatusChip = ({ status }) => (
  <Chip label={status} color={STATUS_COLORS[status] || "default"} size="small" />
);
