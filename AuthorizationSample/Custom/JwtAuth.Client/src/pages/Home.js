import React, { useState, useEffect } from 'react';
import { getResources } from '../services/Api'

function HomePage() {
    const [data, setData] = useState("default");
    
    useEffect(() => {
        async function prefetch() {
            const response = await getResources();
            console.log(response);
            setData(response);
        }

        prefetch();
    });

    async function sendRequests() {
        getResources();
        getResources();
        getResources();
    }

    return (
        <div>
            <button onClick={sendRequests}>Send 3 requests</button>
            Home Page {data}
        </div>
    );
}
export default HomePage;