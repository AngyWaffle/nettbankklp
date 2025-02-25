import React, { useEffect, useState } from "react";
import axios from "axios";
import { Paper, Box, Typography, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Button, Modal, TextField, MenuItem, Select, InputLabel, FormControl, Collapse } from "@mui/material";

const Dashboard = () => {
    const [accounts, setAccounts] = useState([]);
    const [transactions, setTransactions] = useState({});
    const [openAccounts, setOpenAccounts] = useState({});
    const [errorMessage, setErrorMessage] = useState("");
    const [openModal, setOpenModal] = useState(false);
    const [selectedAccount, setSelectedAccount] = useState("");
    const [recipientAccount, setRecipientAccount] = useState("");
    const [amount, setAmount] = useState("");

    const userId = localStorage.getItem("userId");

    useEffect(() => {
        if (!userId) {
            setErrorMessage("You are not logged in. Please log in to view your accounts.");
            return;
        }
        fetchAccounts();
    }, [userId]);

    // Fetch accounts & update balance after a transaction
    const fetchAccounts = () => {
        axios.get(`http://localhost:5008/api/Account/GetAccounts/${userId}`)
            .then(response => {
                setAccounts(response.data);
            })
            .catch(error => {
                setErrorMessage("Failed to load accounts.");
                console.error("Error fetching accounts:", error);
            });
    };

    // Fetch transactions when "View Transactions" is clicked
    const fetchTransactions = (accountNumber) => {
        axios.get(`http://localhost:5008/api/Account/GetTransactions/${accountNumber}`)
            .then(response => {
                setTransactions(prev => ({ ...prev, [accountNumber]: response.data }));
            })
            .catch(error => {
                console.error("Error fetching transactions:", error);
            });
    };

    const handleLogout = () => {
        localStorage.removeItem("userId");
        window.location.href = "/login";
    };

    // Open/Close Modal Functions
    const handleOpenModal = () => setOpenModal(true);
    const handleCloseModal = () => {
        setOpenModal(false);
        setSelectedAccount("");
        setRecipientAccount("");
        setAmount("");
    };

    // Handle Transaction Submission & Auto Update Balances
    const handleTransactionSubmit = () => {
        if (!selectedAccount || !recipientAccount || !amount) {
            alert("Please fill in all fields.");
            return;
        }

        const transactionData = {
            accountNumber: Number(selectedAccount),
            accountNumberReceived: Number(recipientAccount),
            amount: parseFloat(amount),
            type: "Transfer",
            date: new Date().toISOString()
        };

        axios.post("http://localhost:5008/api/Account/Transaction", transactionData)
            .then(response => {
                alert(response.data.message || "Transaction Successful!");
                fetchAccounts(); // Refresh accounts & update balances
                fetchTransactions(selectedAccount); // Refresh transactions
                handleCloseModal();
            })
            .catch(error => {
                console.error("Error making transaction:", error);
                alert(`Transaction failed: ${error.response?.data || "Please try again."}`);
            });
    };

    // Toggle transactions visibility
    const toggleTransactions = (accountNumber) => {
        setOpenAccounts(prev => ({ ...prev, [accountNumber]: !prev[accountNumber] }));
        if (!transactions[accountNumber]) {
            fetchTransactions(accountNumber);
        }
    };

    return (
        <Paper elevation={3} sx={{ p: 4, mt: 4 }}>
            <Box display="flex" flexDirection="column" alignItems="center">
                <Typography component="h1" variant="h5" gutterBottom>
                    Your Accounts
                </Typography>

                {errorMessage ? (
                    <Typography color="error">{errorMessage}</Typography>
                ) : (
                    <TableContainer component={Paper} sx={{ mt: 2 }}>
                        <Table>
                            <TableHead>
                                <TableRow>
                                    <TableCell><strong>Account Number</strong></TableCell>
                                    <TableCell><strong>Account Type</strong></TableCell>
                                    <TableCell><strong>Balance</strong></TableCell>
                                    <TableCell><strong>Actions</strong></TableCell>
                                </TableRow>
                            </TableHead>
                            <TableBody>
                                {accounts.length > 0 ? (
                                    accounts.map((account) => (
                                        <React.Fragment key={account.accountNumber}>
                                            <TableRow>
                                                <TableCell>{account.accountNumber}</TableCell>
                                                <TableCell>{account.accountType}</TableCell>
                                                <TableCell>${account.balance.toFixed(2)}</TableCell>
                                                <TableCell>
                                                    <Button
                                                        variant="contained"
                                                        color="primary"
                                                        sx={{ fontSize: "0.8rem", mr: 1 }}
                                                        onClick={() => toggleTransactions(account.accountNumber)}
                                                    >
                                                        {openAccounts[account.accountNumber] ? "Hide Transactions" : "View Transactions"}
                                                    </Button>
                                                </TableCell>
                                            </TableRow>

                                            {/* Transactions Section (Collapsible) */}
                                            <TableRow>
                                                <TableCell colSpan={4}>
                                                    <Collapse in={openAccounts[account.accountNumber]} timeout="auto" unmountOnExit>
                                                        <Box sx={{ m: 2 }}>
                                                            <Typography variant="h6">Transaction History</Typography>
                                                            <Table size="small">
                                                                <TableHead>
                                                                    <TableRow>
                                                                        <TableCell><strong>Amount</strong></TableCell>
                                                                        <TableCell><strong>Type</strong></TableCell>
                                                                        <TableCell><strong>Sender</strong></TableCell>
                                                                        <TableCell><strong>Receiver</strong></TableCell>
                                                                        <TableCell><strong>Date</strong></TableCell>
                                                                    </TableRow>
                                                                </TableHead>
                                                                <TableBody>
                                                                    {transactions[account.accountNumber] && transactions[account.accountNumber].length > 0 ? (
                                                                        transactions[account.accountNumber].map((transaction) => (
                                                                            <TableRow key={transaction.id}>
                                                                                <TableCell>${transaction.amount.toFixed(2)}</TableCell>
                                                                                <TableCell>{transaction.type}</TableCell>
                                                                                <TableCell>{transaction.accountNumber}</TableCell>
                                                                                <TableCell>{transaction.accountNumberReceived}</TableCell>
                                                                                <TableCell>{new Date(transaction.date).toLocaleString()}</TableCell>
                                                                            </TableRow>
                                                                        ))
                                                                    ) : (
                                                                        <TableRow>
                                                                            <TableCell colSpan={6} align="center">
                                                                                No transactions found.
                                                                            </TableCell>
                                                                        </TableRow>
                                                                    )}
                                                                </TableBody>
                                                            </Table>
                                                        </Box>
                                                    </Collapse>
                                                </TableCell>
                                            </TableRow>
                                        </React.Fragment>
                                    ))
                                ) : (
                                    <TableRow>
                                        <TableCell colSpan={4} align="center">
                                            No accounts found.
                                        </TableCell>
                                    </TableRow>
                                )}
                            </TableBody>
                        </Table>
                    </TableContainer>
                )}

                {/* Make Transaction Button */}
                <Button 
                    variant="contained" 
                    color="primary" 
                    sx={{ mt: 3 }} 
                    onClick={handleOpenModal}
                >
                    Make Transaction
                </Button>

                <Button 
                    variant="contained" 
                    color="secondary" 
                    sx={{ mt: 2 }} 
                    onClick={handleLogout}
                >
                    Logout
                </Button>
            </Box>
        </Paper>
    );
};

export default Dashboard;
