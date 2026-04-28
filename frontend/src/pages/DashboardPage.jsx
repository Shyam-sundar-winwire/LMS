import EventRepeatRoundedIcon from "@mui/icons-material/EventRepeatRounded";
import PendingActionsRoundedIcon from "@mui/icons-material/PendingActionsRounded";
import VerifiedRoundedIcon from "@mui/icons-material/VerifiedRounded";
import { Button, Grid, List, ListItem, ListItemText, Stack, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { InlineErrorState } from "../components/InlineErrorState";
import { LoadingScreen } from "../components/LoadingScreen";
import { PageHeader } from "../components/PageHeader";
import { SectionCard } from "../components/SectionCard";
import { StatCard } from "../components/StatCard";
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
      <PageHeader
        eyebrow="Workspace overview"
        title={`Welcome back, ${user.fullName.split(" ")[0]}`}
        chips={[user.role, `${summary.availableBalanceDays} days available`]}
      />

      <Grid container spacing={2.5}>
        <Grid item xs={12} md={4}>
          <StatCard
            label="Pending items"
            value={user.role === ROLES.MANAGER ? summary.teamPendingApprovals : summary.pendingLeaves}
            helper={user.role === ROLES.MANAGER ? "Requests waiting for your review." : "Your open leave requests right now."}
            accent="15, 61, 222"
          />
        </Grid>
        <Grid item xs={12} md={4}>
          <StatCard
            label="Available balance"
            value={`${summary.availableBalanceDays} days`}
            helper="Combined remaining balance across your leave allocation."
            accent="14, 116, 144"
          />
        </Grid>
        <Grid item xs={12} md={4}>
          <StatCard
            label={user.role === ROLES.MANAGER ? "Approved so far" : "Rejected so far"}
            value={user.role === ROLES.MANAGER ? summary.approvedLeaves : summary.rejectedLeaves}
            helper={user.role === ROLES.MANAGER ? "Requests already resolved successfully." : "Requests that were declined."}
            accent="217, 119, 6"
          />
        </Grid>
      </Grid>

      <SectionCard
        title="Today at a glance"
        action={
          <Button component={Link} to="/apply" variant="contained">
            Apply leave
          </Button>
        }
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
              Status
            </Typography>
            <Typography variant="h6">{summary.approvedLeaves} approved requests</Typography>
          </Grid>
        </Grid>
      </SectionCard>

      <Grid container spacing={2.5}>
        <Grid item xs={12} md={7}>
          <SectionCard title="Leave balances">
            <List disablePadding>
              {balances.map((balance) => (
                <ListItem
                  key={balance.leaveTypeId}
                  sx={{
                    px: 0,
                    py: 1.35,
                    borderBottom: "1px solid rgba(148, 163, 184, 0.14)"
                  }}
                >
                  <ListItemText primary={balance.leaveTypeName} secondary={`Year ${balance.year}`} />
                  <Typography fontWeight={700}>{balance.remainingDays} days</Typography>
                </ListItem>
              ))}
            </List>
          </SectionCard>
        </Grid>
        <Grid item xs={12} md={5}>
          <SectionCard title="Performance snapshot">
            <Stack spacing={2}>
              <Stack direction="row" spacing={1.5} alignItems="center">
                <PendingActionsRoundedIcon sx={{ color: "#4F6B92" }} />
                <Typography>
                  Pending: <strong>{summary.pendingLeaves}</strong>
                </Typography>
              </Stack>
              <Stack direction="row" spacing={1.5} alignItems="center">
                <VerifiedRoundedIcon sx={{ color: "#4F6B92" }} />
                <Typography>
                  Approved: <strong>{summary.approvedLeaves}</strong>
                </Typography>
              </Stack>
              <Stack direction="row" spacing={1.5} alignItems="center">
                <EventRepeatRoundedIcon sx={{ color: "#4F6B92" }} />
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
