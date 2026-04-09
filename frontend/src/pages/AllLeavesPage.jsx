import { format } from "date-fns";
import { useEffect, useState } from "react";
import { Stack, Table, TableBody, TableCell, TableHead, TableRow } from "@mui/material";
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
        eyebrow=""
        title="All leave requests"
        subtitle=""
      />
      <SectionCard title="All leave requests" subtitle="">
        {!leaves.length ? (
          <EmptyState title="No records found" description="" />
        ) : (
          <Table>
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
                  <TableCell>{leave.reason}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        )}
      </SectionCard>
    </Stack>
  );
};
