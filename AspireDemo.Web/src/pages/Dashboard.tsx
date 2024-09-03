import {useEffect} from 'react'
import {useSelector} from 'react-redux'
import Button from "../components/Button";
import {AuthorizedStatus} from "../models/AuthorizedStatus";
import axios from 'axios';

const Dashboard = () => {

    const authState = useSelector<RootState, AuthorizedStatus>(state => state.user);

    const handleLogin = () => {
        const currentUrl = window.location.href;
        window.location.href = `/auth/login?redirectUrl=${currentUrl}`
    }

    const handleLogout = () => {
        const currentUrl = window.location.href;
        window.location.href = `/auth/logout?redirectUrl=${currentUrl}`
    }

    const triggerEmail = async () => {
        await axios.post("/api/email", {});
    }
   
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
                        {
                            authState.isAuthenticated ? (
                                <>
                                    <Button click={handleLogout}>Logout</Button>        
                                    <Button click={triggerEmail}>Send Email</Button>        
                                </>
                            ) : (
                                <Button click={handleLogin}>Login</Button>        
                            )
                        }
                    </div>
                    <div className="mt-4">
                        <code className="block whitespace-pre overflow-x-scroll">
                            {JSON.stringify(authState, null, 2)}
                        </code>
                    </div>
                </div>
            </main>
        </>
    )
}

export default Dashboard