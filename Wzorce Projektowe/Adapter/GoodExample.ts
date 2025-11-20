// ✅ GOOD EXAMPLE: Adapter mapujący na domenowy interfejs

// Domenowy interfejs - czytelne nazwy używane w aplikacji:
interface Weather {
    temperature: number;
    humidity: number;
    windSpeed: number;
    windDirection: string;
    pressure: number;
    city: string;
    country: string;
    timestamp: Date;
}

// Zewnętrzne API (pozostaje bez zmian):
interface WeatherApiResponse {
    tmp: number;
    hmd: number;
    wnd_spd: number;
    wnd_dir: string;
    prs: number;
    cty: string;
    cntry: string;
    ts: number;
}

// Adapter tłumaczący API na domenowy model:
class WeatherApiAdapter {
    adapt(apiResponse: WeatherApiResponse): Weather {
        return {
            temperature: apiResponse.tmp,
            humidity: apiResponse.hmd,
            windSpeed: apiResponse.wnd_spd,
            windDirection: apiResponse.wnd_dir,
            pressure: apiResponse.prs,
            city: apiResponse.cty,
            country: apiResponse.cntry,
            timestamp: new Date(apiResponse.ts * 1000)
        };
    }
}

// Frontend używa domenowego interfejsu:
class WeatherDisplay {
    showWeather(weather: Weather): void {
        console.log(`Weather in ${weather.city}, ${weather.country}`);
        console.log(`Temperature: ${weather.temperature}°C`);
        console.log(`Humidity: ${weather.humidity}%`);
        console.log(`Wind: ${weather.windSpeed} m/s ${weather.windDirection}`);
        console.log(`Pressure: ${weather.pressure} hPa`);
        console.log(`Updated: ${weather.timestamp.toLocaleString()}`);
    }

    getTemperatureStatus(weather: Weather): string {
        if (weather.temperature < 0) return "Freezing";
        if (weather.temperature < 15) return "Cold";
        if (weather.temperature < 25) return "Comfortable";
        return "Hot";
    }

    isWindy(weather: Weather): boolean {
        return weather.windSpeed > 10;
    }
}

class WeatherChart {
    renderChart(weather: Weather): void {
        // Czytelny, samodokumentujący się kod!
        const chartData = {
            temperature: weather.temperature,
            wind: weather.windSpeed,
            humidity: weather.humidity
        };
        console.log("Rendering chart with data:", chartData);
    }
}

class WeatherAlert {
    checkAlerts(weather: Weather): string[] {
        const alerts: string[] = [];
        
        if (weather.temperature > 35) {
            alerts.push("High temperature warning");
        }
        
        if (weather.windSpeed > 20) {
            alerts.push("Strong wind warning");
        }
        
        return alerts;
    }
}

// Użycie:
async function fetchWeather() {
    const response = await fetch("https://api.weather-service.com/current?city=Warsaw");
    const apiData: WeatherApiResponse = await response.json();
    
    // Adapter tłumaczy API na domenowy model:
    const adapter = new WeatherApiAdapter();
    const weather = adapter.adapt(apiData);
    
    // Komponenty używają czytelnego interfejsu:
    const display = new WeatherDisplay();
    display.showWeather(weather);
    
    const chart = new WeatherChart();
    chart.renderChart(weather);
    
    const alerts = new WeatherAlert();
    console.log("Alerts:", alerts.checkAlerts(weather));
}

// Bonus: Łatwa zmiana API providera!
interface AlternativeWeatherApi {
    t: number;
    h: number;
    ws: number;
    location: { city: string; country: string };
}

class AlternativeWeatherAdapter {
    adapt(apiResponse: AlternativeWeatherApi): Weather {
        return {
            temperature: apiResponse.t,
            humidity: apiResponse.h,
            windSpeed: apiResponse.ws,
            windDirection: "N", // Default jeśli brak w API
            pressure: 1013,     // Default jeśli brak w API
            city: apiResponse.location.city,
            country: apiResponse.location.country,
            timestamp: new Date()
        };
    }
}

// Komponenty NIE WYMAGAJĄ ZMIAN! Używają tego samego interfejsu Weather
// Wystarczy zamienić adapter przy pobieraniu danych

// Korzyści:
// 1. CZYTELNY KOD - domenowe nazwy zamiast skrótów
// 2. JEDNA ODPOWIEDZIALNOŚĆ - adapter zajmuje się tylko translacją
// 3. ŁATWA ZMIANA API - modyfikacja tylko w adapterze
// 4. TESTOWANIE - łatwe mockowanie domenowego interfejsu
// 5. ROZSZERZALNOŚĆ - nowe API = nowy adapter, bez zmian w komponentach
// 6. SEPARACJA WARSTW - frontend odizolowany od szczegółów API
