import { format } from "date-fns";
import { Alert, Button, Stack, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, TextField } from "@mui/material";
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
  const [comments, setComments] = useState({});
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
      await leaveService.reviewLeave(id, { approve, comment: comments[id] || "" });
      setComments((prev) => ({ ...prev, [id]: "" }));
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
        eyebrow="Review center"
        title="Approvals"
        chips={[`${items.length} waiting`]}
      />
      <SectionCard title="Approval queue">
        {feedback ? <Alert severity={feedback.type}>{feedback.message}</Alert> : null}
        {!items.length ? (
          <EmptyState title="Queue is clear" description="There are no pending requests waiting for your action right now." />
        ) : (
          <TableContainer sx={{ borderRadius: 3, overflowX: "auto" }}>
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell>Employee</TableCell>
                  <TableCell>Leave type</TableCell>
                  <TableCell>Dates</TableCell>
                  <TableCell>Days</TableCell>
                  <TableCell>Status</TableCell>
                  <TableCell>Reason</TableCell>
                  <TableCell>Comment</TableCell>
                  <TableCell align="right">Actions</TableCell>
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
                    <TableCell sx={{ minWidth: 160, maxWidth: 220, whiteSpace: "normal", wordBreak: "break-word" }}>{item.reason}</TableCell>
                    <TableCell sx={{ minWidth: 160 }}>
                      <TextField
                        size="small"
                        placeholder="Add a note"
                        value={comments[item.id] || ""}
                        onChange={(event) =>
                          setComments((prev) => ({
                            ...prev,
                            [item.id]: event.target.value
                          }))
                        }
                        fullWidth
                      />
                    </TableCell>
                    <TableCell align="right" sx={{ minWidth: 156 }}>
                      <Stack direction="row" spacing={1} justifyContent="flex-end">
                        <Button size="small" variant="contained" color="success" onClick={() => reviewLeave(item.id, true)}>
                          Approve
                        </Button>
                        <Button size="small" variant="outlined" color="error" onClick={() => reviewLeave(item.id, false)}>
                          Reject
                        </Button>
                      </Stack>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        )}
      </SectionCard>
    </Stack>
  );
};
