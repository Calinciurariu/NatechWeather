# Natech Weather MAUI Application

This repository contains a cross-platform weather forecasting application for **Android** and **iOS**, built with **.NET MAUI**. The application delivers real-time weather data, hourly forecasts, and includes a custom-built, high-performance charting component for data visualization.

The project is architected to be robust, maintainable, and extensible, demonstrating professional software engineering practices within the .NET MAUI framework.

## Core Features

The application provides a comprehensive and responsive user experience with the following features:

### Weather Data & API Integration
- **OpenWeatherMap One Call API 3.0**: Fetches current weather, 48-hour hourly forecasts, and daily forecasts in a single API call.
- **Geocoding and Reverse Geocoding**:
  - Users can input a city name, which is converted to latitude and longitude using .NET MAUI's built-in geocoding services.
  - When using device GPS, the app performs reverse geocoding to display the relevant city name.
- **Location-Based Weather**: Retrieves weather data for the user's current location using device GPS, with proper permission handling.

### Data Visualization
- **Custom Charting Library (NatechCharts)**: A reusable, high-performance charting library implemented as a separate .NET MAUI Class Library.
- **Hardware-Accelerated Rendering**: Utilizes **SkiaSharp**'s `SKGLView` with OpenGL for smooth, GPU-accelerated animations and interactions.
- **Interactive Forecast Chart**: Displays a 48-hour temperature forecast as a line chart with:
  - Touch-based panning to navigate the forecast.
  - A tooltip on press-and-hold gestures, showing precise temperature and time for selected data points.
  - Dynamic axis labels that adjust format (hourly vs. daily) and rotation based on the time span, ensuring readability.

### User Interface & Experience
- **Custom Input Component**: A reusable `LocationInputView` `ContentView` with a modern text entry experience, featuring a floating label that animates on focus or when text is present.
- **Theming**: Supports **Light** and **Dark** modes for excellent readability and a native feel across platforms.
- **Asynchronous Operations with UI Feedback**: All network and long-running operations are asynchronous, with a full-screen loading indicator bound to an `IsBusy` property to provide clear feedback and prevent interaction during data fetching.
- **Interactive UI Elements**: A "multitasking" label allows users to tap to cycle through secondary weather details, such as "Feels Like" temperature and "Humidity."
- **Platform-Specific Audio Feedback**: Non-blocking audio cues for successful and failed data fetch operations, implemented with platform-native audio players for optimal performance.

## Application Services
- **In-Memory Caching**: Stores API results for one hour to reduce redundant API calls, improve performance, and respect API rate limits.
- **API Key Management**: Reads the OpenWeatherMap API key from a standard `appsettings.json` file, ensuring secrets are not hardcoded.

## Technical Architecture & Design Patterns
The application is built with a focus on clean architecture, testability, and maintainability, leveraging the following design patterns:

### Model-View-ViewModel (MVVM)
- **Views**: XAML-based pages and controls (`MainPage.xaml`, `LocationInputView.xaml`) handle UI structure and presentation.
- **ViewModels**: `MainPageViewModel` manages presentation logic, state, and commands, fully decoupled from the View.
- **Models**: Plain C# objects (`OneCallResult`, `ChartDataPoint`) represent application data.
- **Self-Implemented MVVM Framework**: Includes a custom `BaseViewModel` (implementing `INotifyPropertyChanged`) and `AsyncRelayCommand` (implementing `ICommand`) to demonstrate MVVM mechanics without external libraries.

### Dependency Injection (DI)
- Services (`IWeatherService`, `IAudioHelper`, `ICacheService`, `HttpClient`), ViewModels, and Pages are registered in `MauiProgram.cs` and resolved at runtime for loose coupling and testability.

### Decorator Pattern
- **WeatherService**: Handles direct API communication.
- **CachingWeatherService**: Wraps `WeatherService`, implementing the same `IWeatherService` interface. It checks the cache before delegating to the real service on cache misses, enabling flexible caching via DI registration.

### Adapter Pattern
- The `NatechCharts` library uses reflection to adapt application-specific data (`ChartDataPoint`) into generic X/Y coordinates for plotting, making the charting library reusable.
- `LineSeries` exposes `XValuePath` and `YValuePath` properties, set to model properties (`Date`, `Value`) in the app.

### Service Layer (Repository Pattern Variation)
- Interfaces like `IWeatherService` and `IAudioHelper` abstract implementation details, enabling swapping of implementations (e.g., `MockWeatherService` for testing) via DI.

### Strategy Pattern (Implicit)
- The `ChartRenderer` supports multiple chart types via a `ChartType` enum. Adding new chart types (e.g., bar charts) requires only a new renderer, leaving the core rendering loop unchanged.

## Project Structure
The solution is organized into two projects:
- **NatechWeather**: The main .NET MAUI application, containing Views, ViewModels, API data models, and services.
- **NatechCharts**: A .NET MAUI Class Library with reusable charting components, including the `LineChart` control and SkiaSharp-based `ChartRenderer`.

## Setup and Configuration

### Prerequisites
- **.NET 8 SDK**
- **.NET MAUI Workload**: Install with `dotnet workload install maui`
- **Visual Studio 2022** or a compatible IDE

### API Key
The application requires an API key from [OpenWeatherMap One Call API 3.0](https://openweathermap.org/api/one-call-3).

1. Create a file named `appsettings.json` in the root of the `NatechWeather` project.
2. Set its **Build Action** to `MauiAsset`.
3. Add the following content, replacing `"YOUR_API_KEY"` with your actual key:

```json
{
  "OpenWeatherMap": {
    "ApiKey": "YOUR_API_KEY",
    "GeoBaseUrl": "http://api.openweathermap.org/geo/1.0/direct",
    "OneCallBaseUrl": "https://api.openweathermap.org/data/3.0/onecall"
  }
}
```
###Build and Run
Open NatechWeather.sln in Visual Studio.
Restore NuGet packages for both projects.
Select the target platform (Android or iOS) and run the application.

###License
This project is licensed under the MIT License (LICENSE). See the LICENSE file for details.

### Formatting Details
- **Headers**: Used `#` for the main title, `##` for sections, and `###` for subsections to maintain hierarchy.
- **Lists**: Used `-` for unordered lists and `1.` for ordered lists, with consistent indentation.
- **Code Blocks**: JSON and other code snippets are wrapped in triple backticks (```) with language identifiers for syntax highlighting.
- **Links**: Hyperlinked the OpenWeatherMap API URL for easy access.
- **Emphasis**: Used **bold** for key terms (e.g., Android, iOS, NatechCharts) and *italics* for minor emphasis.
- **Clarity**: Broke up long sections into concise bullet points and ensured consistent spacing.
- **Professional Touch**: Added a license placeholder and a friendly closing note to align with common GitHub READMEs.

### Notes
- The `LICENSE` file is referenced but not included. You can add an MIT License file or replace it with your preferred license.
- If you want to add additional sections (e.g., Contributing, Testing, or Screenshots), let me know, and I can expand the README.
- The code assumes `appsettings.json` is a `MauiAsset`. If you use a different configuration method, I can adjust the instructions.

Let me know if you need further tweaks or additional content!

