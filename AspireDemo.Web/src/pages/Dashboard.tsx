import {useEffect, useState} from 'react'
import axios from 'axios'
import Button from "../components/Button.tsx";

const Dashboard = () => {
    const handleLogin = () => {
        const currentUrl = window.location.href;
        window.location.href = `/auth/login?redirectUrl=${currentUrl}`
    }

    const handleLogout = () => {
        const currentUrl = window.location.href;
        window.location.href = `/auth/logout?redirectUrl=${currentUrl}`
    }

    const [authState, setAuthState] = useState<string>("");
    
    useEffect(() => {
        axios.get("/auth/me")
            .then(result => setAuthState(JSON.stringify(result.data, null, 2)))
    }, [])
    return (
        <>
            <header>
                <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
                    <h1 className="text-3xl font-bold leading-tight tracking-tight text-gray-900">Dashboard</h1>
                </div>
            </header>
            <main>
                <div className="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
                    <div className="flex gap-4">
                        <Button click={handleLogin}>Log in</Button>
                        <Button click={handleLogout}>Log out</Button>
                    </div>
                    <div className="mt-4">
                        <code className="block whitespace-pre overflow-x-scroll">
                            {authState}
                        </code>
                    </div>
                </div>
            </main>
        </>
    )
}

export default Dashboard