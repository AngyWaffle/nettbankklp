import React from 'react';
import { Avatar, Button, TextField, Paper, Box, Typography } from '@mui/material';
import LockOutlinedIcon from '@mui/icons-material/LockOutlined';
import { Link } from 'react-router-dom';
import axios from 'axios';

/**
 * SignUp component renders a sign-up form for new users.
 * It collects user's full name, email, and password, processes the input,
 * and sends it to the server to create a new account. If successful, it redirects
 * the user to the login page. In case of an error, it displays an alert with the error message.
 */

function SignUp() {
    const handleSubmit = (event) => {
        event.preventDefault();
        
        const fullName = event.target.name.value.trim();
        const email = event.target.email.value.trim();
        const password = event.target.password.value.trim();
    
        const nameParts = fullName.split(" ");
        const lastName = nameParts.pop();
        const firstName = nameParts.join(" ") || lastName;
    
        const userData = {
            firstName: firstName,
            lastName: lastName,
            mail: email,
            password: password,
            salt: ""
        };

        axios.post("http://localhost:5008/api/User/CreateUser", userData)
            .then(response => {
                window.location.href = "/login";
            })
            .catch(error => {
                console.error("Error creating account:", error);
                alert(`Account creation failed: ${error.response?.data || "Please try again."}`);
            });
    };      
    
  return (
    <Paper elevation={3} sx={{ p: 4, mt: 4 }}>
      <Box display="flex" flexDirection="column" alignItems="center">
        <Avatar sx={{ m: 1, bgcolor: 'primary.main' }}>
          <LockOutlinedIcon />
        </Avatar>
        <Typography component="h1" variant="h5">
          Sign Up
        </Typography>
        <Box component="form" onSubmit={handleSubmit} sx={{ mt: 1 }}>
          <TextField
            margin="normal"
            required
            fullWidth
            id="name"
            label="Full Name"
            name="name"
            autoComplete="name"
            autoFocus
          />
          <TextField
            margin="normal"
            required
            fullWidth
            id="email"
            label="Email Address"
            name="email"
            autoComplete="email"
          />
          <TextField
            margin="normal"
            required
            fullWidth
            name="password"
            label="Password"
            type="password"
            id="password"
            autoComplete="new-password"
          />
          <Button
            type="submit"
            fullWidth
            variant="contained"
            sx={{ mt: 3, mb: 2 }}
          >
            Sign Up
          </Button>
          <Link to="/login">Already have an account? Log in</Link>
        </Box>
      </Box>
    </Paper>
  );
}

export default SignUp;
