# Error Boundaries i Monitoring

## Problem w BadExample
* **Brak error boundaries:** Błędy crashują całą aplikację
* **Unhandled rejections:** Async errors nie są obsługiwane
* **Brak logging:** Błędy nie są raportowane

## Rozwiązanie in GoodExample
* **Error Boundary class:** Łapie błędy w drzewie komponentów
* **Fallback UI:** Wyświetla przyjazny komunikat błędu
* **Error logging:** Integracja z monitoring services (Sentry, LogRocket)
* **Retry mechanism:** Możliwość ponowienia operacji

## Korzyści
* **Graceful degradation:** Aplikacja nie crashuje całkowicie
* **Better UX:** Użytkownik widzi co się stało
* **Monitoring:** Błędy są ś ledzone i raportowane
* **Recovery:** Możliwość naprawienia błędu bez reload

## Potencjalne pułapki
* **Event handlers:** Error boundaries nie łapią błędów z event handlers
* **Async code:** Nie łapią błędów z async/await
* **SSR:** Wymagają specjalnej obsługi w SSR
