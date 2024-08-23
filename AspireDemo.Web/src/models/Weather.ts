export interface WeatherForecast {
    date: string
    summary: string
    temperatureC: number
    temperatureF: number
}

export interface OpenWeatherResult {
    coord: LatLon
    weather: any[]
    base: string
    main: any
    visibility: number
    wind: Wind
    clouds: any
    dt: number
    sys: any;
    timezone: number;
    id: number;
    name: string;
    cod: number;
}

export interface LatLon {
    lon: number;
    lat: number;
}

export interface Wind {
    speed: number;
    deg: number;
}
