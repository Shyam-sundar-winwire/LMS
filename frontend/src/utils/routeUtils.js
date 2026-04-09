import DashboardRoundedIcon from "@mui/icons-material/DashboardRounded";
import EventAvailableRoundedIcon from "@mui/icons-material/EventAvailableRounded";
import FactCheckRoundedIcon from "@mui/icons-material/FactCheckRounded";
import AssignmentRoundedIcon from "@mui/icons-material/AssignmentRounded";
import GroupsRoundedIcon from "@mui/icons-material/GroupsRounded";
import { ROLES } from "./constants";

export const getNavigationItems = (role) => {
  const common = [
    { label: "Dashboard", path: "/", icon: DashboardRoundedIcon },
    { label: "Apply Leave", path: "/apply", icon: EventAvailableRoundedIcon },
    { label: "My Leaves", path: "/my-leaves", icon: AssignmentRoundedIcon }
  ];

  if (role === ROLES.MANAGER) {
    return [...common, { label: "Approvals", path: "/approvals", icon: FactCheckRoundedIcon }];
  }

  if (role === ROLES.HR || role === ROLES.ADMIN) {
    return [
      ...common,
      { label: "Approvals", path: "/approvals", icon: FactCheckRoundedIcon },
      { label: "All Leaves", path: "/all-leaves", icon: GroupsRoundedIcon }
    ];
  }

  return common;
};
