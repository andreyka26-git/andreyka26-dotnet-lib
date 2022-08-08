import { useNavigate } from 'react-router-dom';
import { useEffect } from 'react';
import axios from 'axios';

function CallbackPage() {
    const navigate = useNavigate();

    useEffect(() => {
        try {
            const query = new URLSearchParams(window.location.search);
            const tokenBase64 = query.get('token');

            const tokenJson = atob(tokenBase64);
            const token = JSON.parse(tokenJson);

            if (token) {
                localStorage.setItem("token", token.AuthorizationToken);
                axios.defaults.headers.common["Authorization"] = `Bearer ${token.AuthorizationToken}`;
            }
        }
        catch(e) {
            console.log(e.message);
        }

        navigate('/');
    });
}

export default CallbackPage;