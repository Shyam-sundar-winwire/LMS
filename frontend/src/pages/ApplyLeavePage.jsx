import { Alert, Button, Chip, Grid, MenuItem, Stack, TextField, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import { InlineErrorState } from "../components/InlineErrorState";
import { LoadingScreen } from "../components/LoadingScreen";
import { PageHeader } from "../components/PageHeader";
import { SectionCard } from "../components/SectionCard";
import { leaveService } from "../services/leaveService";

export const ApplyLeavePage = () => {
  const [leaveTypes, setLeaveTypes] = useState([]);
  const [balances, setBalances] = useState([]);
  const [form, setForm] = useState({
    leaveTypeId: "",
    startDate: "",
    endDate: "",
    reason: ""
  });
  const [feedback, setFeedback] = useState(null);
  const [submitting, setSubmitting] = useState(false);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState("");

  const loadFormData = async () => {
    setLoading(true);
    setLoadError("");
    try {
      const [types, currentBalances] = await Promise.all([
        leaveService.getLeaveTypes(),
        leaveService.getBalances()
      ]);
      setLeaveTypes(types);
      setBalances(currentBalances);
      if (types.length) {
        setForm((prev) => ({ ...prev, leaveTypeId: prev.leaveTypeId || types[0].id }));
      }
    } catch (err) {
      setLoadError(err.userMessage || "Unable to load leave form data.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadFormData();
  }, []);

  const handleChange = (field) => (event) => {
    setForm((prev) => ({ ...prev, [field]: event.target.value }));
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    setFeedback(null);
    setSubmitting(true);
    try {
      await leaveService.applyLeave({
        leaveTypeId: Number(form.leaveTypeId),
        startDate: form.startDate,
        endDate: form.endDate,
        reason: form.reason
      });
      setFeedback({ type: "success", message: "Leave request submitted successfully." });
      setForm((prev) => ({ ...prev, startDate: "", endDate: "", reason: "" }));
      setBalances(await leaveService.getBalances());
    } catch (err) {
      setFeedback({ type: "error", message: err.userMessage || "Unable to submit leave request." });
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return <LoadingScreen message="Loading leave types and balances..." />;
  }

  if (loadError) {
    return <InlineErrorState message={loadError} onRetry={loadFormData} />;
  }

  const selectedBalance = balances.find((balance) => balance.leaveTypeId === Number(form.leaveTypeId));

  return (
    <Stack spacing={3}>
      <PageHeader
        eyebrow="Request leave"
        title="Apply leave"
        chips={selectedBalance ? [`${selectedBalance.remainingDays} days left in ${selectedBalance.leaveTypeName}`] : []}
      />

      <Grid container spacing={2.5}>
        <Grid item xs={12} md={7}>
          <SectionCard title="Apply for leave">
            <Stack component="form" spacing={2} onSubmit={handleSubmit}>
              {feedback ? <Alert severity={feedback.type}>{feedback.message}</Alert> : null}
              <TextField label="Leave type" select value={form.leaveTypeId} onChange={handleChange("leaveTypeId")} fullWidth>
                {leaveTypes.map((type) => (
                  <MenuItem key={type.id} value={type.id}>
                    {type.name}
                  </MenuItem>
                ))}
              </TextField>
              <Grid container spacing={2}>
                <Grid item xs={12} md={6}>
                  <TextField
                    label="Start date"
                    type="date"
                    value={form.startDate}
                    onChange={handleChange("startDate")}
                    InputLabelProps={{ shrink: true }}
                    fullWidth
                  />
                </Grid>
                <Grid item xs={12} md={6}>
                  <TextField
                    label="End date"
                    type="date"
                    value={form.endDate}
                    onChange={handleChange("endDate")}
                    InputLabelProps={{ shrink: true }}
                    fullWidth
                  />
                </Grid>
              </Grid>
              <TextField label="Reason" value={form.reason} onChange={handleChange("reason")} multiline minRows={4} fullWidth />
              <Button type="submit" variant="contained" disabled={submitting}>
                {submitting ? "Submitting..." : "Submit request"}
              </Button>
            </Stack>
          </SectionCard>
        </Grid>
        <Grid item xs={12} md={5}>
          <SectionCard title="Available balances">
            <Stack spacing={2}>
              {balances.map((balance) => (
                <Stack
                  key={balance.leaveTypeId}
                  direction="row"
                  justifyContent="space-between"
                  alignItems="center"
                  sx={{ py: 1.2, borderBottom: "1px solid rgba(148, 163, 184, 0.16)" }}
                >
                  <Typography>{balance.leaveTypeName}</Typography>
                  <Typography fontWeight={700}>{balance.remainingDays} days</Typography>
                </Stack>
              ))}
              {selectedBalance ? (
                <Chip
                  label={`${selectedBalance.leaveTypeName}: ${selectedBalance.remainingDays} days available`}
                  variant="outlined"
                  sx={{ alignSelf: "flex-start", backgroundColor: "rgba(255,255,255,0.72)" }}
                />
              ) : null}
            </Stack>
          </SectionCard>
        </Grid>
      </Grid>
    </Stack>
  );
};
