import axios from 'axios';

let isRefreshing = false;
let refreshedRequests = [];

axios.interceptors.response.use(
    (res) =>  res,
    async (error) => {
        const originalConfig = error.config;
        const token = localStorage.getItem("token");
        
        if (!token || !isUnauthorizedError(error)) {
            return Promise.reject(error);
        }
        
        if (isRefreshing) {     
            refreshedRequests.push(originalConfig);
            return Promise.reject(error);
        }

        isRefreshing = true;

        try {
            const [newToken, newRefreshToken] = await renewToken();
            localStorage.setItem("token", newToken);
            localStorage.setItem("refreshToken", newRefreshToken);

            originalConfig.headers.Authorization = `Bearer ${newToken}`;

            try {
                await axios.request(originalConfig);
            } catch(innerError) {
                if (isUnauthorizedError(innerError)) {
                    throw innerError;
                }

                return Promise.reject(innerError);                    
            }

            for (let config of refreshedRequests) {
                config.headers.Authorization = `Bearer ${newToken}`;
                await axios.request(config);
            }
        } catch (err) {
            localStorage.removeItem("token");
            localStorage.removeItem("refreshToken");

            window.location = `${window.location.origin}/login`;
        } finally {
            isRefreshing = false;
        }
    },
)

function isUnauthorizedError(error) {
    const {
        response: { status, statusText },
    } = error;

    return status === 401;
}

export async function authenticate(userName, password) {
    const loginPayload = {
        userName: userName,
        password: password
    };

    const response = await axios.post("https://localhost:7000/authorization/token", loginPayload);
    
    const token = response.data.authorizationToken;
    const refreshToken = response.data.refreshToken;

    return [token, refreshToken];
}

export async function renewToken() {
    const refreshToken = localStorage.getItem("refreshToken");

    if (!refreshToken)
        throw new Error('refresh token does not exist');

    const refreshPayload = {
        refreshToken: refreshToken
    };

    const response = await axios.post("https://localhost:7000/authorization/refresh", refreshPayload);
    const token = response.data.authorizationToken;
    const newRefreshToken = response.data.refreshToken;

    return [token, newRefreshToken];
}

export async function getResources() {
    const headers = withAuth();

    const options = {
        headers: headers
    }

    const response = await axios.get("https://localhost:7000/api/resources", options);
    const data = response.data;

    return data;
}

function withAuth(headers) {
    const token = localStorage.getItem("token");

    if (!token) {
        window.location = `${window.location.origin}/login`;
    }

    if (!headers) {
        headers = { };
    }
    
    headers.Authorization = `Bearer ${token}`

    return headers;
}
