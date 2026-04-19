# Oneri Bul

A cross-platform mood-based recommendation engine for books, movies, and games. Developed by RieStudio using .NET MAUI.

## Overview

Oneri Bul analyzes user input via the Gemini API to suggest relevant content. It integrates movie data from TMDB and handles local storage for user history and saved items.

## Technical Specifications

- Framework: .NET 9.0 (MAUI)
- Architecture: MVVM
- AI Integration: Google Gemini API
- External Data: TMDB API (Movies & TV)
- Language: C# / XAML

## Key Components

- GeminiService: Handles mood analysis and prompt engineering.
- TmdbService: Manages movie metadata and poster retrieval.
- StorageService: Local persistence for user data.
- DiscoverViewModel: Core logic for filtering and recommendation flow.

## Screenshots

| Interface | Saved Content | History |
| :--- | :--- | :--- |
| [Image Path] | [Image Path] | [Image Path] |

## Setup and Development

### Configuration

Open Config.cs and insert your API keys:
- Gemini API Key
- TMDB API Key

### Build

1. Clone: git clone https://github.com/RieStudio/oneri-bul.git
2. Open OneriBul.sln in Visual Studio 2022.
3. Select target (Android, Windows, or iOS).
4. Build and Run.

## Contact

Arif Sevban KIRIT - arif@riestudio.com.tr
Official Site: riestudio.com.tr

---
Copyright 2026 RieStudio.
