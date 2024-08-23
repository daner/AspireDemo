import {BrowserRouter as Router, Routes, Route} from 'react-router-dom'
import Navbar from './components/Navbar.tsx'

import {NavigationItem} from "./models/Navigation.ts";
import Weather from "./components/Weather.tsx";
import Dashboard from "./components/Dashboard.tsx";
import Messages from "./components/Messages.tsx";
import Search from "./components/Search.tsx";

const navigation: NavigationItem[] = [
    {name: 'Dashboard', href: '/'},
    {name: 'Weather', href: '/weather'},
    {name: 'Search', href: '/search'},
    {name: 'Messages', href: '/messages'},
]

const App = () => {

    return (
        <>
            <Router>
                <div className="min-h-full">
                    <Navbar navigation={navigation}/>
                    <div className="py-10">
                        <Routes>
                            <Route path="/" element={<Dashboard/>}/>
                            <Route path="/weather" element={<Weather/>}/>
                            <Route path="/search" element={<Search/>}/>
                            <Route path="/messages" element={<Messages/>}/>
                        </Routes>
                    </div>
                </div>
            </Router>
        </>
    )
}

export default App
