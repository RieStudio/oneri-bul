# Oneri Bul

<p align="center">
  <img src=".github/assets/oneribullogo.png" width="400">
</p>

## Overview

Öneri Bul is an AI-powered recommendation application that helps users discover movies, books, and games based on their current mood. Instead of browsing through endless lists, users simply describe how they feel, and the application analyzes their input to generate personalized recommendations within seconds.
## Screenshots

- Framework: .NET 9.0 (MAUI)
- Architecture: MVVM
- AI Engine: Google Gemini API
- Movie Data: TMDB API
- Game Data: RAWG API
- Language: C# / XAML

## Features

- AI-powered recommendations
- Mood-based recommendation engine
- Movie, book, and game suggestions
- Smart category selection
- Clean Material Design interface
- Cross-platform support with .NET MAUI
- Fast and responsive user experience
- Lightweight and modern architecture
- Persistent user preferences

## Architecture

The project follows the **MVVM (Model-View-ViewModel)** architecture to provide a clear separation of concerns and improve maintainability.

- **Views** – User interface built with .NET MAUI XAML.
- **ViewModels** – Manage UI state, user interactions, and application logic.
- **Models** – Define the application's data structures.
- **Services** – Handle AI communication, external APIs, local storage, and business logic.
- **Converters** – Transform data for UI presentation.
- **Resources** – Store application assets, styles, fonts, and icons.

This architecture keeps the codebase modular, scalable, and easy to maintain while supporting future feature expansion.

## Tech Stack

- **Framework:** .NET MAUI
- **Language:** C#
- **Architecture:** MVVM
- **AI:** Google Gemini API
- **Movie Data:** TMDb API
- **Book Data:** Google Books API
- **Game Data:** RAWG API

## Getting Started

Clone the repository.

```bash
git clone https://github.com/RieStudio/OneriBul.git
```

Open the project in Android Studio and run the Android target.

## License

This project is licensed under the [MIT License](LICENSE).



