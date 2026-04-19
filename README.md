# Oneri Bul

A cross-platform mood-based recommendation engine for books, movies, and games. Developed by RieStudio using .NET MAUI.

## Overview

Oneri Bul analyzes user input via the Gemini API to suggest relevant content. It integrates movie data from TMDB and game metadata/covers from the RAWG API, while handling local storage for user history.

## Technical Specifications

- Framework: .NET 9.0 (MAUI)
- Architecture: MVVM
- AI Engine: Google Gemini API
- Movie Data: TMDB API
- Game Data: RAWG API
- Language: C# / XAML

## Key Components

- GeminiService: Handles mood analysis and prompt engineering for content discovery.
- TmdbService: Manages movie metadata and poster retrieval.
- RawgService: Fetches game titles, descriptions, and high-quality cover art.
- StorageService: Local persistence for user data and history.

## Screenshots

| Interface | Saved Content | History |
| :--- | :--- | :--- |
| [Image Path] | [Image Path] | [Image Path] |

## Setup and Development

### Configuration

Open Config.cs and insert your respective API keys:
- Gemini API Key
- TMDB API Key
- RAWG API Key

### Build Process

1. Clone the repository: git clone https://github.com/RieStudio/oneri-bul.git
2. Open OneriBul.sln in Visual Studio 2022.
3. Select target platform (Android, Windows, or iOS).
4. Build and Run the solution.

## Contact

RieStudio - contact@riestudio.com.tr
Official Site: riestudio.com.tr

---
Copyright 2026 RieStudio.
