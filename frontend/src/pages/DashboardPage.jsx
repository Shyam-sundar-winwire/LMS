import EventRepeatRoundedIcon from "@mui/icons-material/EventRepeatRounded";
import PendingActionsRoundedIcon from "@mui/icons-material/PendingActionsRounded";
import VerifiedRoundedIcon from "@mui/icons-material/VerifiedRounded";
import { Button, Grid, List, ListItem, ListItemText, Stack, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { InlineErrorState } from "../components/InlineErrorState";
import { LoadingScreen } from "../components/LoadingScreen";
import { SectionCard } from "../components/SectionCard";
import { dashboardService } from "../services/dashboardService";
import { leaveService } from "../services/leaveService";
import { ROLES } from "../utils/constants";

export const DashboardPage = () => {
  const { user } = useAuth();
  const [summary, setSummary] = useState(null);
  const [balances, setBalances] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const loadDashboard = async () => {
    setLoading(true);
    setError("");
    try {
      const [summaryResponse, balanceResponse] = await Promise.all([
        dashboardService.getSummary(),
        leaveService.getBalances()
      ]);
      setSummary(summaryResponse);
      setBalances(balanceResponse);
    } catch (err) {
      setError(err.userMessage || "Unable to load dashboard data.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadDashboard();
  }, []);

  if (loading) {
    return <LoadingScreen message="Preparing your dashboard..." />;
  }

  if (error || !summary) {
    return <InlineErrorState message={error || "Dashboard data is unavailable."} onRetry={loadDashboard} />;
  }

  return (
    <Stack spacing={3}>
      <SectionCard
        title="Summary"
        subtitle=""
      >
        <Grid container spacing={2}>
          <Grid item xs={12} md={4}>
            <Typography variant="body2" color="text.secondary">
              Pending
            </Typography>
            <Typography variant="h6">
              {user.role === ROLES.MANAGER
                ? `${summary.teamPendingApprovals} pending approvals`
                : `${summary.pendingLeaves} pending requests`}
            </Typography>
          </Grid>
          <Grid item xs={12} md={4}>
            <Typography variant="body2" color="text.secondary">
              Balance
            </Typography>
            <Typography variant="h6">{summary.availableBalanceDays} days remaining</Typography>
          </Grid>
          <Grid item xs={12} md={4}>
            <Typography variant="body2" color="text.secondary">
              Action
            </Typography>
            <Button component={Link} to="/apply" variant="contained" sx={{ mt: 1 }}>
              Apply leave
            </Button>
          </Grid>
        </Grid>
      </SectionCard>

      <Grid container spacing={2.5}>
        <Grid item xs={12} md={7}>
          <SectionCard title="Leave balances" subtitle="">
            <List disablePadding>
              {balances.map((balance) => (
                <ListItem key={balance.leaveTypeId} sx={{ px: 0 }}>
                  <ListItemText primary={balance.leaveTypeName} secondary={`Year ${balance.year}`} />
                  <Typography fontWeight={700}>{balance.remainingDays} days</Typography>
                </ListItem>
              ))}
            </List>
          </SectionCard>
        </Grid>
        <Grid item xs={12} md={5}>
          <SectionCard title="Details" subtitle="">
            <Stack spacing={2}>
              <Stack direction="row" spacing={1.5} alignItems="center">
                <PendingActionsRoundedIcon color="warning" />
                <Typography>
                  Pending: <strong>{summary.pendingLeaves}</strong>
                </Typography>
              </Stack>
              <Stack direction="row" spacing={1.5} alignItems="center">
                <VerifiedRoundedIcon color="success" />
                <Typography>
                  Approved: <strong>{summary.approvedLeaves}</strong>
                </Typography>
              </Stack>
              <Stack direction="row" spacing={1.5} alignItems="center">
                <EventRepeatRoundedIcon color="primary" />
                <Typography>
                  {user.role === ROLES.MANAGER
                    ? `Team approvals: ${summary.teamPendingApprovals}`
                    : `Rejected: ${summary.rejectedLeaves}`}
                </Typography>
              </Stack>
            </Stack>
          </SectionCard>
        </Grid>
      </Grid>
    </Stack>
  );
};
