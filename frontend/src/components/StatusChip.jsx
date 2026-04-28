import { Chip } from "@mui/material";

const STATUS_STYLES = {
  Pending: {
    backgroundColor: "rgba(183, 121, 31, 0.12)",
    color: "#8A5A14",
    borderColor: "rgba(183, 121, 31, 0.16)"
  },
  Approved: {
    backgroundColor: "rgba(24, 137, 91, 0.12)",
    color: "#176A48",
    borderColor: "rgba(24, 137, 91, 0.16)"
  },
  Rejected: {
    backgroundColor: "rgba(197, 59, 59, 0.11)",
    color: "#9F2F2F",
    borderColor: "rgba(197, 59, 59, 0.16)"
  }
};

export const StatusChip = ({ status }) => {
  const styles = STATUS_STYLES[status] || {
    backgroundColor: "rgba(90, 107, 132, 0.1)",
    color: "#4F5F76",
    borderColor: "rgba(90, 107, 132, 0.16)"
  };

  return (
    <Chip
      label={status}
      size="small"
      sx={{
        minWidth: 92,
        justifyContent: "center",
        fontWeight: 700,
        letterSpacing: "-0.01em",
        border: "1px solid",
        ...styles
      }}
    />
  );
};
