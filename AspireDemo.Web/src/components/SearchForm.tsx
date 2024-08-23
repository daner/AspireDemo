import {useState} from 'react'
import {ExclamationCircleIcon} from '@heroicons/react/20/solid'

interface FormState {
    city: string;
    country: string;
    cityError: boolean;
    countryError: boolean;
}

interface IProps {
    searchCallback: (code: string, city: string) => Promise<void>
}

const SearchForm = ({searchCallback} : IProps) => {

    const [formData, setFormData] = useState<FormState>({city: "", country: "", cityError: false, countryError: false})

    const classNames = (...classes: string[]) => {
        return classes.filter(Boolean).join(' ')
    }
    const handleSubmit = async (event: any) => {
        event.preventDefault();

        let cityError = false;
        let countryError = false;

        if(formData.city.length === 0) {
            cityError = true;
        }

        if(formData.country.length === 0) {
            countryError = true;
        }

        setFormData({...formData, cityError: cityError, countryError: countryError})

        if(!cityError && !countryError)
        {
            await searchCallback(formData.country, formData.city);
        }
    }
    
    const handleCityChanged = (event: any) => {
        setFormData({...formData, city: event.target.value})
    }

    const handleCountryChanged = (event: any) => {
        setFormData({...formData, country: event.target.value})
    }

    const commonClasses = "block rounded-md border-0 py-1.5 sm:text-sm sm:leading-6 focus:ring-2 focus:ring-inset ring-1 ring-inset"
    const errorClasses = "pr-10 text-red-900 ring-red-300 placeholder:text-red-300  focus:ring-red-500 "
    const normalClasses = "shadow-sm text-gray-900 ring-gray-300 placeholder:text-gray-400 focus:ring-indigo-600"
    
    return(
        <>
            <form onSubmit={handleSubmit}>
                <span>Search for city to show weather</span>
                <div className="mt-2 flex gap-4">
                    <div className="relative mt-2">
                        <input
                            id="country"
                            name="country"
                            type="text"
                            placeholder="SE"
                            value={formData.country}
                            onChange={handleCountryChanged}
                            className={classNames("max-w-20", commonClasses, formData.countryError ? errorClasses : normalClasses)}
                        />
                        {
                            formData.countryError && (
                                <div
                                    className="pointer-events-none absolute inset-y-0 right-0 flex items-center pr-3">
                                    <ExclamationCircleIcon aria-hidden="true" className="h-5 w-5 text-red-500"/>
                                </div>
                            )
                        }
                    </div>
                    <div className="relative mt-2">
                        <input
                            id="city"
                            name="city"
                            type="text"
                            value={formData.city}
                            onChange={handleCityChanged}
                            placeholder="Norrköping"
                            className={classNames("max-w-48", commonClasses, formData.cityError ? errorClasses : normalClasses)}
                        />
                        {
                            formData.cityError && (
                                <div
                                    className="pointer-events-none absolute inset-y-0 right-0 flex items-center pr-3">
                                    <ExclamationCircleIcon aria-hidden="true" className="h-5 w-5 text-red-500"/>
                                </div>
                            )
                        }
                    </div>
                    <div className="mt-2">
                        <button
                            className="rounded-md bg-indigo-600 px-3.5 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600">
                            Search
                        </button>
                    </div>
                </div>
                <div>
                    {
                        (formData.cityError || formData.countryError) && (
                            <p id="form-error" className="mt-2 text-sm text-red-600">
                                Must enter country code and city.
                            </p>
                        )
                    }
                </div>
            </form>
        </>)
}

export default SearchForm