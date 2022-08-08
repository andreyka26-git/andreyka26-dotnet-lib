import axios from 'axios';
import React, { useState, useEffect } from 'react';

function HomePage() {
    const [data, setData] = useState("default");

    useEffect(() => {
        const token = localStorage.getItem("token");

        if (token) {
            axios.defaults.headers.common["Authorization"] = `Bearer ${token}`;
        }
        
        if (data == "default") {
            axios.get("https://localhost:7000/api/resources")
            .then(response => {
                const data = response.data;
                
                setData(data);
            })
            .catch(err => console.log(err));
        }
    });
    
    return (
        <div>
            Home Page {data}
        </div>
    );
}
export default HomePage;