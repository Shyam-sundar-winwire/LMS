import { createTheme } from "@mui/material/styles";

export const appTheme = createTheme({
  palette: {
    mode: "light",
    primary: {
      main: "#2F6DF6",
      light: "#5B8DFF",
      dark: "#1F4FB9"
    },
    secondary: {
      main: "#4F6B92",
      light: "#7891B2",
      dark: "#324766"
    },
    success: {
      main: "#18895B"
    },
    warning: {
      main: "#B7791F"
    },
    error: {
      main: "#C53B3B"
    },
    background: {
      default: "#F3F6FB",
      paper: "#FFFFFF"
    },
    text: {
      primary: "#10233F",
      secondary: "#5A6B84"
    }
  },
  shape: {
    borderRadius: 16
  },
  typography: {
    fontFamily: "'Sora', 'Plus Jakarta Sans', 'Segoe UI', sans-serif",
    h2: {
      fontSize: "3rem",
      fontWeight: 800,
      letterSpacing: "-0.05em"
    },
    h3: {
      fontSize: "2.25rem",
      fontWeight: 800,
      letterSpacing: "-0.05em"
    },
    h4: {
      fontSize: "1.7rem",
      fontWeight: 800,
      letterSpacing: "-0.04em"
    },
    h5: {
      fontSize: "1.24rem",
      fontWeight: 800
    },
    h6: {
      fontSize: "1rem",
      fontWeight: 700
    },
    body1: {
      fontSize: "0.94rem"
    },
    body2: {
      fontSize: "0.85rem"
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
          border: "1px solid rgba(162, 179, 201, 0.2)",
          boxShadow: "0 10px 28px rgba(16, 35, 63, 0.06)",
          backgroundImage: "none"
        }
      }
    },
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: 12,
          minHeight: 38,
          paddingInline: 15,
          paddingBlock: 7,
          fontWeight: 700,
          fontSize: "0.88rem",
          letterSpacing: "-0.01em",
          boxShadow: "none",
          transition: "background-color 180ms ease, border-color 180ms ease, box-shadow 180ms ease",
          "&:hover": {
            boxShadow: "none"
          }
        },
        contained: {
          backgroundImage: "none"
        },
        outlined: {
          borderWidth: 1,
          "&:hover": {
            borderWidth: 1
          }
        }
      }
    },
    MuiChip: {
      styleOverrides: {
        root: {
          borderRadius: 10,
          height: 26,
          fontWeight: 600
        }
      }
    },
    MuiTextField: {
      defaultProps: {
        variant: "outlined"
      },
      styleOverrides: {
        root: {
          "& .MuiOutlinedInput-root": {
            minHeight: 42,
            borderRadius: 12,
            backgroundColor: "rgba(248, 250, 252, 0.94)"
          },
          "& .MuiInputBase-input": {
            paddingTop: 10,
            paddingBottom: 10
          }
        }
      }
    },
    MuiTableCell: {
      styleOverrides: {
        head: {
          fontWeight: 700,
          color: "#4A5F7E",
          paddingTop: 10,
          paddingBottom: 10,
          borderBottom: "1px solid rgba(133, 153, 180, 0.22)"
        },
        root: {
          paddingTop: 10,
          paddingBottom: 10,
          borderBottom: "1px solid rgba(133, 153, 180, 0.16)"
        }
      }
    },
    MuiTableRow: {
      styleOverrides: {
        root: {
          transition: "background-color 160ms ease"
        }
      }
    }
  }
});
