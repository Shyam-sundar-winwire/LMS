import CalendarMonthRoundedIcon from "@mui/icons-material/CalendarMonthRounded";
import LogoutRoundedIcon from "@mui/icons-material/LogoutRounded";
import MenuRoundedIcon from "@mui/icons-material/MenuRounded";
import {
  AppBar,
  Avatar,
  Button,
  Box,
  Drawer,
  IconButton,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Paper,
  Stack,
  Toolbar,
  Typography
} from "@mui/material";
import { useMemo, useState } from "react";
import { NavLink, useLocation } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { getNavigationItems } from "../utils/routeUtils";

const drawerWidth = 258;

export const AppShell = ({ children }) => {
  const { user, logout } = useAuth();
  const location = useLocation();
  const [mobileOpen, setMobileOpen] = useState(false);
  const navigationItems = useMemo(() => getNavigationItems(user?.role), [user?.role]);
  const today = new Intl.DateTimeFormat("en-US", { month: "long", day: "numeric", year: "numeric" }).format(new Date());

  const drawer = (
    <Stack
      sx={{
        height: "100%",
        p: 1.5,
        backgroundColor: "rgba(244, 248, 252, 0.78)",
        backdropFilter: "blur(16px)",
        borderRight: "1px solid rgba(162, 179, 201, 0.18)"
      }}
      spacing={1.5}
    >
      <Paper
        sx={{
          p: 1.5,
          borderRadius: 4,
          backgroundColor: "rgba(255, 255, 255, 0.86)",
          backdropFilter: "blur(12px)"
        }}
      >
        <Stack spacing={0.5}>
          <Typography variant="h6">Leave Management</Typography>
          <Typography variant="body2" color="text.secondary">
            {user?.role}
          </Typography>
        </Stack>
      </Paper>

      <List sx={{ display: "grid", gap: 0.5 }}>
        {navigationItems.map((item) => {
          const Icon = item.icon;
          const isActive = location.pathname === item.path;

          return (
            <ListItemButton
              key={item.path}
              component={NavLink}
              to={item.path}
              onClick={() => setMobileOpen(false)}
              sx={{
                borderRadius: 3.5,
                py: 0.9,
                px: 1,
                transition: "background-color 180ms ease, border-color 180ms ease",
                backgroundColor: isActive ? "rgba(47, 109, 246, 0.1)" : "transparent",
                boxShadow: isActive ? "inset 0 0 0 1px rgba(47, 109, 246, 0.12)" : "none",
                "&:hover": {
                  backgroundColor: "rgba(255,255,255,0.82)"
                }
              }}
            >
              <ListItemIcon sx={{ minWidth: 34 }}>
                <Icon color={isActive ? "primary" : "inherit"} />
              </ListItemIcon>
              <ListItemText primary={item.label} />
            </ListItemButton>
          );
        })}
      </List>

      <Box sx={{ flexGrow: 1 }} />

      <Paper sx={{ p: 1.5, borderRadius: 4, backgroundColor: "rgba(255, 255, 255, 0.86)", backdropFilter: "blur(12px)" }}>
        <Stack direction="row" spacing={1.25} alignItems="center" sx={{ mb: 1 }}>
          <Avatar sx={{ width: 34, height: 34, fontSize: 14, bgcolor: "primary.main" }}>{user?.fullName?.charAt(0)}</Avatar>
          <Box sx={{ flexGrow: 1 }}>
            <Typography fontWeight={600}>{user?.fullName}</Typography>
            <Typography variant="body2" color="text.secondary">
              {user?.role}
            </Typography>
          </Box>
        </Stack>
        <Button fullWidth variant="outlined" startIcon={<LogoutRoundedIcon />} onClick={logout}>
          Sign out
        </Button>
      </Paper>
    </Stack>
  );

  return (
    <Box
      sx={{
        display: "flex",
        minHeight: "100vh",
        backgroundImage: "linear-gradient(rgba(245, 248, 252, 0.58), rgba(238, 243, 249, 0.72)), var(--app-photo-bg)",
        backgroundSize: "cover",
        backgroundPosition: "center",
        backgroundAttachment: "fixed"
      }}
    >
      <AppBar
        position="fixed"
        color="transparent"
        elevation={0}
        sx={{
          width: { md: `calc(100% - ${drawerWidth}px)` },
          ml: { md: `${drawerWidth}px` },
          backgroundColor: "rgba(243, 246, 251, 0.72)",
          backdropFilter: "blur(14px)",
          borderBottom: "1px solid rgba(162, 179, 201, 0.18)"
        }}
      >
        <Toolbar sx={{ gap: 1.25, minHeight: "64px !important" }}>
          <IconButton sx={{ display: { md: "none" }, mr: 1 }} onClick={() => setMobileOpen(true)}>
            <MenuRoundedIcon />
          </IconButton>
          <Box>
            <Typography variant="h6">Overview</Typography>
            <Typography variant="body2" color="text.secondary">
              {user?.fullName}
            </Typography>
          </Box>
          <Box sx={{ flexGrow: 1 }} />
          <Stack direction="row" spacing={1} alignItems="center" sx={{ display: { xs: "none", sm: "flex" } }}>
            <Paper sx={{ px: 1.25, py: 0.75, borderRadius: 3, backgroundColor: "rgba(255,255,255,0.82)", backdropFilter: "blur(12px)" }}>
              <Stack direction="row" spacing={1} alignItems="center">
                <CalendarMonthRoundedIcon sx={{ fontSize: 18, color: "primary.main" }} />
                <Typography variant="body2" fontWeight={600}>
                  {today}
                </Typography>
              </Stack>
            </Paper>
          </Stack>
        </Toolbar>
      </AppBar>

      <Box component="nav" sx={{ width: { md: drawerWidth }, flexShrink: { md: 0 } }}>
        <Drawer
          variant="temporary"
          open={mobileOpen}
          onClose={() => setMobileOpen(false)}
          ModalProps={{ keepMounted: true }}
          sx={{ display: { xs: "block", md: "none" }, "& .MuiDrawer-paper": { width: drawerWidth, border: "none" } }}
        >
          {drawer}
        </Drawer>
        <Drawer
          variant="permanent"
          open
          sx={{
            display: { xs: "none", md: "block" },
            "& .MuiDrawer-paper": {
              width: drawerWidth,
              boxSizing: "border-box",
              border: "none",
              backgroundColor: "transparent"
            }
          }}
        >
          {drawer}
        </Drawer>
      </Box>

      <Box component="main" sx={{ flexGrow: 1, px: { xs: 1.25, md: 2 }, pb: { xs: 1.5, md: 2 }, pt: { xs: 9.5, md: 10.25 } }}>
        <Box sx={{ maxWidth: 1240, mx: "auto" }}>
          {children}
        </Box>
      </Box>
    </Box>
  );
};
