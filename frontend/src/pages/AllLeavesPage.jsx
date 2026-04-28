import { format } from "date-fns";
import { useEffect, useState } from "react";
import { Stack, Table, TableBody, TableCell, TableContainer, TableHead, TableRow } from "@mui/material";
import { EmptyState } from "../components/EmptyState";
import { InlineErrorState } from "../components/InlineErrorState";
import { LoadingScreen } from "../components/LoadingScreen";
import { PageHeader } from "../components/PageHeader";
import { SectionCard } from "../components/SectionCard";
import { StatusChip } from "../components/StatusChip";
import { leaveService } from "../services/leaveService";

const formatDate = (value) => format(new Date(value), "dd MMM yyyy");

export const AllLeavesPage = () => {
  const [leaves, setLeaves] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const loadLeaves = async () => {
    setLoading(true);
    setError("");
    try {
      setLeaves(await leaveService.getAllLeaves());
    } catch (err) {
      setError(err.userMessage || "Unable to load organization leave requests.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadLeaves();
  }, []);

  if (loading) {
    return <LoadingScreen message="Loading all leave requests..." />;
  }

  if (error) {
    return <InlineErrorState message={error} onRetry={loadLeaves} />;
  }

  return (
    <Stack spacing={3}>
      <PageHeader
        eyebrow="Organization view"
        title="All leave requests"
        chips={[`${leaves.length} records`]}
      />
      <SectionCard title="All leave requests">
        {!leaves.length ? (
          <EmptyState title="No leave requests found" description="When employees begin submitting leave, organization-wide records will appear here." />
        ) : (
          <TableContainer sx={{ borderRadius: 3, overflowX: "auto" }}>
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell>Employee</TableCell>
                  <TableCell>Email</TableCell>
                  <TableCell>Leave type</TableCell>
                  <TableCell>Dates</TableCell>
                  <TableCell>Days</TableCell>
                  <TableCell>Status</TableCell>
                  <TableCell>Reason</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {leaves.map((leave) => (
                  <TableRow key={leave.id} hover>
                    <TableCell>{leave.employeeName}</TableCell>
                    <TableCell>{leave.employeeEmail}</TableCell>
                    <TableCell>{leave.leaveTypeName}</TableCell>
                    <TableCell>
                      {formatDate(leave.startDate)} - {formatDate(leave.endDate)}
                    </TableCell>
                    <TableCell>{leave.daysRequested}</TableCell>
                    <TableCell>
                      <StatusChip status={leave.status} />
                    </TableCell>
                    <TableCell sx={{ minWidth: 190, whiteSpace: "normal", wordBreak: "break-word" }}>{leave.reason}</TableCell>
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
