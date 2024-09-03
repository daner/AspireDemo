import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import { useEffect } from 'react'
import Navbar from './components/Navbar'
import { checkAuthorizedStatus } from './reducers/userReducer';
import { useAppDispatch } from "./store";
import { NavigationItem } from "./models/Navigation";
import Weather from "./pages/Weather";
import Dashboard from "./pages/Dashboard";
import Messages from "./pages/Messages";
import Search from "./pages/Search";

const navigation: NavigationItem[] = [
    { name: 'Dashboard', href: '/' },
    { name: 'Weather', href: '/weather' },
    { name: 'Search', href: '/search' },
    { name: 'Messages', href: '/messages' },
]

const App = () => {

    const dispatch = useAppDispatch();

    useEffect(() => {
        dispatch(checkAuthorizedStatus())
    }, [])

    return (
        <>
            <Router>
                <div className="min-h-full">
                    <Navbar navigation={navigation} />
                    <div className="py-5">
                        <Routes>
                            <Route path="/" element={<Dashboard />} />
                            <Route path="/weather" element={<Weather />} />
                            <Route path="/search" element={<Search />} />
                            <Route path="/messages" element={<Messages />} />
                        </Routes>
                    </div>
                </div>
            </Router>
        </>
    )
}

export default App
