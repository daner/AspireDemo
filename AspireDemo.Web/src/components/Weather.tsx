import {useState, useEffect} from 'react'
import axios from 'axios'
import {WeatherForecast} from "../models/Weather.ts";

const Weather = () => {

    const [weather, setWeather] = useState<WeatherForecast[]>([])

    useEffect(() => {
        axios.get<WeatherForecast[]>("/api/weatherforecast").then(result => {
            setWeather(result.data)
        })
    }, [])

    return (
        <>
            <header>
                <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
                    <h1 className="text-3xl font-bold leading-tight tracking-tight text-gray-900">Weather</h1>
                </div>
            </header>
            <main>
                <div className="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
                    <div>
                        <table>
                            <thead>
                            <tr>
                                <th>Date</th>
                                <th>Summary</th>
                                <th>Temp C</th>
                                <th>Temp F</th>
                            </tr>
                            </thead>
                            <tbody>
                            {weather.map((item, i) => (
                                <tr key={i}>
                                    <td>{item.date}</td>
                                    <td>{item.summary}</td>
                                    <td>{item.temperatureC}</td>
                                    <td>{item.temperatureF}</td>
                                </tr>
                            ))}
                            </tbody>
                        </table>
                    </div>
                </div>
            </main>
        </>)
}

export default Weather