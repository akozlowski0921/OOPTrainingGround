// ❌ BAD EXAMPLE: Frontend bezpośrednio korzysta z API o dziwnych polach

// Zewnętrzne API zwraca dziwne nazwy pól:
interface WeatherApiResponse {
    tmp: number;           // temperatura
    hmd: number;           // wilgotność
    wnd_spd: number;       // prędkość wiatru
    wnd_dir: string;       // kierunek wiatru
    prs: number;           // ciśnienie
    cty: string;           // miasto
    cntry: string;         // kraj
    ts: number;            // timestamp
}

// Frontend bezpośrednio używa tych dziwnych nazw:
class WeatherDisplay {
    showWeather(data: WeatherApiResponse): void {
        console.log(`Weather in ${data.cty}, ${data.cntry}`);
        console.log(`Temperature: ${data.tmp}°C`);
        console.log(`Humidity: ${data.hmd}%`);
        console.log(`Wind: ${data.wnd_spd} m/s ${data.wnd_dir}`);
        console.log(`Pressure: ${data.prs} hPa`);
        console.log(`Updated: ${new Date(data.ts * 1000).toLocaleString()}`);
    }

    getTemperatureStatus(data: WeatherApiResponse): string {
        if (data.tmp < 0) return "Freezing";
        if (data.tmp < 15) return "Cold";
        if (data.tmp < 25) return "Comfortable";
        return "Hot";
    }

    isWindy(data: WeatherApiResponse): boolean {
        return data.wnd_spd > 10;
    }
}

// Użycie:
async function fetchWeather() {
    const response = await fetch("https://api.weather-service.com/current?city=Warsaw");
    const data: WeatherApiResponse = await response.json();
    
    const display = new WeatherDisplay();
    display.showWeather(data);
    
    console.log(`Status: ${display.getTemperatureStatus(data)}`);
    console.log(`Windy: ${display.isWindy(data)}`);
}

// Inny komponent też używa tych dziwnych nazw:
class WeatherChart {
    renderChart(data: WeatherApiResponse): void {
        // Co to jest tmp? Co to wnd_spd?
        const chartData = {
            temperature: data.tmp,  // Trzeba pamiętać co to znaczy
            wind: data.wnd_spd,
            humidity: data.hmd
        };
        console.log("Rendering chart with data:", chartData);
    }
}

// Jeszcze jeden komponent:
class WeatherAlert {
    checkAlerts(data: WeatherApiResponse): string[] {
        const alerts: string[] = [];
        
        if (data.tmp > 35) {
            alerts.push("High temperature warning");
        }
        
        if (data.wnd_spd > 20) {
            alerts.push("Strong wind warning");
        }
        
        return alerts;
    }
}

// Problemy:
// 1. NIECZYTELNY KOD - trzeba pamiętać co oznacza tmp, hmd, wnd_spd
// 2. ROZRZUCONA WIEDZA - wiele komponentów musi znać strukturę API
// 3. TRUDNA ZMIANA API - zmiana API wymaga zmian w CAŁYM kodzie frontendu
// 4. BRAK ABSTRAKCJI - frontend ściśle związany z konkretnym API
// 5. TRUDNE TESTOWANIE - trzeba mockować dziwną strukturę API
// 6. NIEMOŻNOŚĆ ZAMIANY API - zmiana providera = przepisanie całego kodu
