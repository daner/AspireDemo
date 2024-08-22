import { useState, useEffect } from 'react'
import axios from 'axios'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'

function App() {
    const [count, setCount] = useState(0)
    const [weather, setWeather] = useState([])        

    useEffect(() => {
        axios.get("/api/weatherforecast").then(result => {
            setWeather(result.data)
        })
    }, [])

    return (
        <>
            <div>
                <a href="https://vitejs.dev" target="_blank">
                    <img src={viteLogo} className="logo" alt="Vite logo" />
                </a>
                <a href="https://react.dev" target="_blank">
                    <img src={reactLogo} className="logo react" alt="React logo" />
                </a>
            </div>
            <h1>Vite + React</h1>
            <div className="card">
                <button onClick={() => setCount((count) => count + 1)}>
                    count is {count}
                </button>
                <p>
                    Edit <code>src/App.tsx</code> and save to test HMR
                </p>
            </div>
            <div>
                <table>
                    <thead>
                    <tr><th>Date</th><th>Summary</th><th>Temp</th></tr>
                </thead>
                    <tbody>
                        {weather.map((item :any, i) => (
                            <tr key={i}><td>{item.date}</td><td>{item.summary}</td><td>{ item.temperatureC }</td></tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </>
    )
}

export default App
