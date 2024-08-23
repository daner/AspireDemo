import {useState} from 'react'
import SearchForm from "./SearchForm.tsx";
import axios from 'axios'

const Search = () => {

    const searchHandler = async (code: string, city: string) => {
        const response= await axios.get(`api/weatherforecast/${code}/${city}`)
        console.log(response.data);
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
                    <SearchForm searchCallback={searchHandler} />
                </div>
            </main>
        </>
    )
}

export default Search