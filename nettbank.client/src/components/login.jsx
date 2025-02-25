import React from 'react';
import { Avatar, Button, TextField, Paper, Box, Typography } from '@mui/material';
import LockOutlinedIcon from '@mui/icons-material/LockOutlined';
import { data, Link } from 'react-router-dom';
import axios from 'axios';

function Login() {

    const handleSubmit = (event) => {
        event.preventDefault();
    
        // Create data object correctly
        const data = {
            Mail: event.target.email.value,
            Password: event.target.password.value
        };
    
        // Make POST request
        axios.post('http://localhost:5008/api/User/login', data)
            .then(response => {
                localStorage.setItem('userId', response.data); // Ensure response data is used
                window.location.href = '/dashboard'; // Redirect after setting user ID
            })
            .catch(error => {
                if (error.response && error.response.status === 401) {
                    // Show backend "Invalid username or password" message
                    alert(error.response.data.message);
                } else {
                    alert("An error occurred. Please try again.");
                }
            });
    };

  return (
    <Paper elevation={3} sx={{ p: 4, mt: 4 }}>
      <Box display="flex" flexDirection="column" alignItems="center">
        <Avatar sx={{ m: 1, bgcolor: 'primary.main' }}>
          <LockOutlinedIcon />
        </Avatar>
        <Typography component="h1" variant="h5">
          Log In
        </Typography>
        <Box component="form" onSubmit={handleSubmit} sx={{ mt: 1 }}>
          <TextField
            margin="normal"
            required
            fullWidth
            id="email"
            label="Email Address"
            name="email"
            autoComplete="email"
            autoFocus
          />
          <TextField
            margin="normal"
            required
            fullWidth
            name="password"
            label="Password"
            type="password"
            id="password"
            autoComplete="current-password"
          />
          <Button
            type="submit"
            fullWidth
            variant="contained"
            sx={{ mt: 3, mb: 2 }}
          >
            Log In
          </Button>
          <Link to="/signup">Dont have an account? Sign up</Link>
        </Box>
      </Box>
    </Paper>
  );
}

export default Login;
