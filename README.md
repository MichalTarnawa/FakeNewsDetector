# 🕵️‍♂️ OSINT Fake News Detector & Wieloagentowy System Weryfikacji

Nowoczesna aplikacja desktopowa (WPF/C#) zintegrowana z chmurowym systemem sztucznej inteligencji opartym na agentach (LangChain + Google Gemini). Służy do automatycznego fact-checkingu, analizy propagandy oraz oceny wiarygodności źródeł za pomocą technik OSINT i systemu RAG.

![UI Preview](link_do_twojego_screena_z_aplikacji.png) ## ✨ Główne funkcje

* **🤖 Architektura Wieloagentowa (ReAct):** Zapytania są analizowane przez Koordynatora AI, który rozdziela zadania do wyspecjalizowanych agentów:
  * **Agent RAG:** Przeszukuje zaufaną, wbudowaną bazę wiedzy przy użyciu wektorów (`SentenceTransformers` + podobieństwo kosinusowe w `numpy`).
  * **Agent Fact-Checker:** Wyszukuje dowody w internecie na żywo (DuckDuckGo Search).
  * **Agent ds. Kontekstu:** Wyjaśnia zmanipulowane zdarzenia i dostarcza szerszego tła.
  * **Agent OSINT:** Analizuje wiarygodność domeny, portalu lub autora.
  * **Agent Image Scanner:** Przetwarza obrazy (multimodalnie) z podanego adresu URL.
  * **Agent Analizy Manipulacji:** Wykrywa clickbaity, błędy logiczne i język nacechowany emocjonalnie.
* **🧠 Lekki System RAG z tablicy:** Autorska implementacja bazy wektorowej niewymagająca ciężkich baz grafowych (np. FAISS/Chroma).
* **🖥️ Nowoczesny Interfejs (Copilot Style):** Tryb Dark Mode, płynne animacje ładowania, responsywność oraz pełne renderowanie języka Markdown w wiadomościach.
* **🔌 Architektura "Sidecar":** Aplikacja C# automatycznie i w tle uruchamia lokalny most w Pythonie (Flask), który w bezpieczny sposób komunikuje się z serwerem Gradio w chmurze.

## 🏗️ Architektura Systemu

1. **Frontend (C# WPF):** Odpowiada za UI, animacje i zbieranie wejścia od użytkownika. Wzorzec projektowy MVVM.
2. **Local Bridge (Python/Flask):** Uruchamiany dyskretnie w tle przez aplikację C#. Obsługuje bibliotekę `gradio_client`, wysyłając żądania do chmury.
3. **Backend / AI Engine (Google Colab):** Skrypt uruchamiany w notatniku chmurowym. Utrzymuje agentów LangChain i wystawia API przy pomocy Gradio na zewnątrz.

## 🚀 Wymagania wstępne

Aby uruchomić projekt, upewnij się, że masz zainstalowane:
* **Visual Studio 2022** (z obsługą aplikacji desktopowych .NET).
* **Python 3.10+** (dodany do zmiennej środowiskowej PATH).

Paczki Pythonowe (dla lokalnego mostu):
```bash
pip install flask gradio_client
