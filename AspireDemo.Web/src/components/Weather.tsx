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
                    <div className="mt-8 flow-root">
                        <div className="-mx-4 -my-2 overflow-x-auto sm:-mx-6 lg:-mx-8">
                            <div className="inline-block min-w-full py-2 align-middle sm:px-6 lg:px-8">
                                <div className="overflow-hidden shadow ring-1 ring-black ring-opacity-5 sm:rounded-lg">
                                    <table className="min-w-full divide-y divide-gray-300">
                                        <thead className="bg-gray-50">
                                        <tr>
                                            <th scope="col"
                                                className="py-3.5 pl-4 pr-3 text-left text-sm font-semibold text-gray-900 sm:pl-6">
                                                Date
                                            </th>
                                            <th scope="col"
                                                className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">
                                                Summary
                                            </th>
                                            <th scope="col"
                                                className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">
                                                Temperature C
                                            </th>
                                            <th scope="col"
                                                className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">
                                                Temperature F
                                            </th>
                                        </tr>
                                        </thead>
                                        <tbody className="divide-y divide-gray-200 bg-white">
                                        {weather.map((item, i) => (
                                            <tr key={i}>
                                                <td className="whitespace-nowrap py-4 pl-4 pr-3 text-sm font-medium text-gray-900 sm:pl-6">
                                                    {item.date}
                                                </td>
                                                <td className="whitespace-nowrap px-3 py-4 text-sm text-gray-500">{item.summary}</td>
                                                <td className="whitespace-nowrap px-3 py-4 text-sm text-gray-500">{item.temperatureC}</td>
                                                <td className="whitespace-nowrap px-3 py-4 text-sm text-gray-500">{item.temperatureF}</td>
                                            </tr>
                                        ))}
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </main>
        </>)
}

export default Weather