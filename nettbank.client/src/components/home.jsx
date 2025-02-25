import React from 'react';
import { Box, Typography, Button } from '@mui/material';
import { Link } from 'react-router-dom';

function Home() {
  return (
    <Box
      display="flex"
      flexDirection="column"
      alignItems="center"
      justifyContent="center"
      minHeight="80vh"
      textAlign="center"
    >
      <Typography variant="h2" component="h1" gutterBottom>
        Velkommen til KLP – mer enn bare pensjon
      </Typography>
      <Typography variant="h5" component="p" gutterBottom>
        Vi tilbyr deg en trygg og god fremtid med våre tjenester innen pensjon, bank og forsikring.
      </Typography>
      <Box mt={4}>
        <Button
          component={Link}
          to="/signup"
          variant="contained"
          color="primary"
          sx={{ mr: 2 }}
        >
          Get Started
        </Button>
        <Button
          component={Link}
          to="/login"
          variant="outlined"
          color="primary"
        >
          Log In
        </Button>
      </Box>
    </Box>
  );
}

export default Home;
