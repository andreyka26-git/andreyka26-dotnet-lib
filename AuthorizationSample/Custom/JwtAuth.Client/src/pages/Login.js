import React, { useState } from 'react';
import axios from 'axios';
import {Link, Routes, Route, useNavigate} from 'react-router-dom';


function LoginPage() {
    const navigate = useNavigate();

    const [userName, setUserName] = useState("andreyka26_");
    const [password, setPassword] = useState("Mypass1*");

    function handleSubmit(event) {
        event.preventDefault();

        const loginPayload = {
            userName: userName,
            password: password
        };
        
        axios.post("https://localhost:7000/authorization/token", loginPayload)
        .then(response => {
            const token = response.data.authorizationToken;
        
            localStorage.setItem("token", token);
            if (token) {
                axios.defaults.headers.common["Authorization"] = `Bearer ${token}`;
            }
        
            navigate('/');
        })
        .catch(err => console.log(err));
    }

    function handleUserNameChange(event) {
        setUserName({value: event.target.value});
    }

    function handlePasswordhange(event) {
        setPassword({value: event.target.value});
    }

    return (
        <div>
            Login Page
            <form onSubmit={handleSubmit}>
                <label>
                    User Name:
                    <input type="text" value={userName} onChange={handleUserNameChange} />
                </label>
                <label>
                    Password:
                    <input type="text" value={password} onChange={handlePasswordhange} />
                </label>
                <input type="submit" value="Submit" />
            </form>
        </div>
    );
}
export default LoginPage;