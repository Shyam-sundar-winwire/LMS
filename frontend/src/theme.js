import { createTheme } from "@mui/material/styles";

export const appTheme = createTheme({
  palette: {
    mode: "light",
    primary: {
      main: "#1447E6"
    },
    secondary: {
      main: "#0891B2"
    },
    background: {
      default: "#EEF3FB",
      paper: "rgba(255,255,255,0.86)"
    },
    text: {
      primary: "#0F172A",
      secondary: "#475569"
    }
  },
  shape: {
    borderRadius: 24
  },
  typography: {
    fontFamily: "'Plus Jakarta Sans', 'Segoe UI', sans-serif",
    h3: {
      fontWeight: 800,
      letterSpacing: "-0.04em"
    },
    h4: {
      fontWeight: 800,
      letterSpacing: "-0.03em"
    },
    h5: {
      fontWeight: 800
    },
    h6: {
      fontWeight: 700
    },
    button: {
      fontWeight: 600,
      textTransform: "none"
    }
  },
  components: {
    MuiPaper: {
      styleOverrides: {
        root: {
          backdropFilter: "blur(20px)",
          border: "1px solid rgba(255,255,255,0.75)",
          boxShadow: "0 24px 55px rgba(15, 23, 42, 0.08)"
        }
      }
    },
    MuiCssBaseline: {
      styleOverrides: {
        body: {
          backgroundAttachment: "fixed"
        }
      }
    },
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: 999,
          paddingInline: 20,
          transition: "transform 180ms ease, box-shadow 180ms ease, background-color 180ms ease",
          "&:hover": {
            transform: "translateY(-2px)",
            boxShadow: "0 14px 24px rgba(20, 71, 230, 0.18)"
          }
        }
      }
    },
    MuiTextField: {
      defaultProps: {
        variant: "outlined"
      }
    }
  }
});
