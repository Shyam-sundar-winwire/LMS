import { format } from "date-fns";
import { Alert, Button, Stack, Table, TableBody, TableCell, TableHead, TableRow, TextField } from "@mui/material";
import { useEffect, useState } from "react";
import { EmptyState } from "../components/EmptyState";
import { InlineErrorState } from "../components/InlineErrorState";
import { LoadingScreen } from "../components/LoadingScreen";
import { PageHeader } from "../components/PageHeader";
import { SectionCard } from "../components/SectionCard";
import { StatusChip } from "../components/StatusChip";
import { leaveService } from "../services/leaveService";

const formatDate = (value) => format(new Date(value), "dd MMM yyyy");

export const ApprovalsPage = () => {
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [comment, setComment] = useState("");
  const [feedback, setFeedback] = useState(null);
  const [error, setError] = useState("");

  const loadItems = async () => {
    setLoading(true);
    setError("");
    try {
      setItems(await leaveService.getApprovals());
    } catch (err) {
      setError(err.userMessage || "Unable to load the approval queue.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const load = async () => {
      await loadItems();
    };

    load();
  }, []);

  const reviewLeave = async (id, approve) => {
    setFeedback(null);
    try {
      await leaveService.reviewLeave(id, { approve, comment });
      setComment("");
      setFeedback({ type: "success", message: `Leave request ${approve ? "approved" : "rejected"} successfully.` });
      await loadItems();
    } catch (err) {
      setFeedback({ type: "error", message: err.userMessage || "Unable to review leave request." });
    }
  };

  if (loading) {
    return <LoadingScreen message="Loading approval queue..." />;
  }

  if (error) {
    return <InlineErrorState message={error} onRetry={loadItems} />;
  }

  return (
    <Stack spacing={3}>
      <PageHeader
        eyebrow=""
        title="Approvals"
        subtitle=""
      />
      <SectionCard title="Approval queue" subtitle="">
      {feedback ? <Alert severity={feedback.type}>{feedback.message}</Alert> : null}
      {!items.length ? (
        <EmptyState title="No records found" description="" />
      ) : (
        <Stack spacing={2}>
          <TextField
            label="Review comment"
            value={comment}
            onChange={(event) => setComment(event.target.value)}
            placeholder="Comment"
            fullWidth
          />
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Employee</TableCell>
                <TableCell>Leave type</TableCell>
                <TableCell>Dates</TableCell>
                <TableCell>Days</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Reason</TableCell>
                <TableCell align="right">Action</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {items.map((item) => (
                <TableRow key={item.id} hover>
                  <TableCell>{item.employeeName}</TableCell>
                  <TableCell>{item.leaveTypeName}</TableCell>
                  <TableCell>
                    {formatDate(item.startDate)} - {formatDate(item.endDate)}
                  </TableCell>
                  <TableCell>{item.daysRequested}</TableCell>
                  <TableCell>
                    <StatusChip status={item.status} />
                  </TableCell>
                  <TableCell>{item.reason}</TableCell>
                  <TableCell align="right">
                    <Stack direction="row" spacing={1} justifyContent="flex-end">
                      <Button variant="contained" color="success" onClick={() => reviewLeave(item.id, true)}>
                        Approve
                      </Button>
                      <Button variant="outlined" color="error" onClick={() => reviewLeave(item.id, false)}>
                        Reject
                      </Button>
                    </Stack>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </Stack>
      )}
      </SectionCard>
    </Stack>
  );
};
