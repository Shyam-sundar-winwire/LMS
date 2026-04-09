import LogoutRoundedIcon from "@mui/icons-material/LogoutRounded";
import MenuRoundedIcon from "@mui/icons-material/MenuRounded";
import {
  AppBar,
  Avatar,
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

const drawerWidth = 280;

export const AppShell = ({ children }) => {
  const { user, logout } = useAuth();
  const location = useLocation();
  const [mobileOpen, setMobileOpen] = useState(false);
  const navigationItems = useMemo(() => getNavigationItems(user?.role), [user?.role]);

  const drawer = (
    <Stack sx={{ height: "100%", p: 2.5 }} spacing={2.5}>
      <List sx={{ display: "grid", gap: 0.75 }}>
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
                borderRadius: 4.5,
                py: 1.3,
                transition: "transform 180ms ease, background-color 180ms ease",
                backgroundColor: isActive ? "rgba(20, 71, 230, 0.12)" : "transparent",
                "&:hover": {
                  transform: "translateX(4px)"
                }
              }}
            >
              <ListItemIcon sx={{ minWidth: 40 }}>
                <Icon color={isActive ? "primary" : "inherit"} />
              </ListItemIcon>
              <ListItemText primary={item.label} />
            </ListItemButton>
          );
        })}
      </List>

      <Box sx={{ flexGrow: 1 }} />

      <Paper sx={{ p: 2 }}>
        <Stack direction="row" spacing={1.5} alignItems="center">
          <Avatar sx={{ bgcolor: "primary.main" }}>{user?.fullName?.charAt(0)}</Avatar>
          <Box sx={{ flexGrow: 1 }}>
            <Typography fontWeight={600}>{user?.fullName}</Typography>
            <Typography variant="body2" color="text.secondary">
              {user?.role}
            </Typography>
          </Box>
          <IconButton onClick={logout}>
            <LogoutRoundedIcon />
          </IconButton>
        </Stack>
      </Paper>
    </Stack>
  );

  return (
    <Box sx={{ display: "flex", minHeight: "100vh" }}>
      <AppBar
        position="fixed"
        color="transparent"
        elevation={0}
        sx={{
          width: { md: `calc(100% - ${drawerWidth}px)` },
          ml: { md: `${drawerWidth}px` },
          backgroundColor: "rgba(238,243,251,0.74)",
          backdropFilter: "blur(18px)",
          borderBottom: "1px solid rgba(255,255,255,0.7)"
        }}
      >
        <Toolbar>
          <IconButton sx={{ display: { md: "none" }, mr: 1 }} onClick={() => setMobileOpen(true)}>
            <MenuRoundedIcon />
          </IconButton>
          <Box>
            <Typography variant="h6">{user?.fullName}</Typography>
            <Typography variant="body2" color="text.secondary">
              {user?.role}
            </Typography>
          </Box>
          <Box sx={{ flexGrow: 1 }} />
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

      <Box component="main" sx={{ flexGrow: 1, p: { xs: 2, md: 4 }, pt: { xs: 11, md: 13 } }}>
        {children}
      </Box>
    </Box>
  );
};
