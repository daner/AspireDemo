import {useState} from 'react'
import SearchForm from "../components/SearchForm";
import axios from 'axios'

const Search = () => {

    const [weather, setWeather] = useState<any>(null)

    const searchHandler = async (code: string, city: string) => {
        try {
            const response = await axios.get(`api/weatherforecast/${code}/${city}`)
            setWeather(response.data);
        } catch (ex: any) {
            setWeather({message: ex.message})
        }
    }

    return (
        <>
            <header>
                <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
                    <h1 className="text-3xl font-bold leading-tight tracking-tight text-gray-900">Search</h1>
                </div>
            </header>
            <main>
                <div className="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
                    <SearchForm searchCallback={searchHandler}/>
                    <div className="mt-8">
                        {
                            weather !== null &&
                            <code className="block whitespace-pre overflow-x-scroll">
                                {JSON.stringify(weather, null, 2)}
                            </code>
                        }
                    </div>
                </div>
            </main>
        </>
    )
}

export default Search