# Лабораторна робота №12. Обробка IO/мережевих помилок та патерн Retry

**Виконала:** Мирончук Ірина, група ІПЗ-3/1
**Варіант 11:** «Отримання списку продуктів»

Запуск:
```bash
dotnet run --project lab12v11
```

| Файл | Призначення |
|---|---|
| `FileProcessor.cs` | `List<string> LoadProductNames(string path)` — перші 2 виклики кидають `FileNotFoundException`, далі читає файл |
| `NetworkClient.cs` | `List<string> GetProductsFromApi(string url)` — перші 3 виклики кидають `HttpRequestException`, далі повертає список |
| `RetryHelper.cs` | узагальнений `ExecuteWithRetry<T>` з експоненційною затримкою, логуванням і `shouldRetry` |
| `Program.cs` | 5 сценаріїв з `try-catch-finally` |

## 1. Реалізація

### RetryHelper

```csharp
public static T ExecuteWithRetry<T>(
    Func<T> operation,
    int retryCount = 3,
    TimeSpan initialDelay = default,
    Func<Exception, bool>? shouldRetry = null)
```

Алгоритм:
1. Виконати `operation()`. Якщо успішно — повернути результат.
2. Якщо виник виняток і `shouldRetry` повертає `false` — прокинути виняток далі (`throw;`) без повторів.
3. Якщо спроби вичерпано (`retryAttempt >= retryCount`) — прокинути останній виняток.
4. Інакше зачекати `initialDelay × 2^retryAttempt` і повторити.

Кожна спроба, причина невдачі та затримка логуються з часом від початку виклику. Загальна кількість спроб = `retryCount + 1`. Якщо `initialDelay` не передано (`default`), використовується 500 мс. Якщо `shouldRetry` не передано, повторюється будь-який виняток.

### FileProcessor і NetworkClient

- `LoadProductNames` імітує файл, який ще не синхронізувався з мережевим диском: перші 2 виклики кидають `FileNotFoundException`, після цього справді читає файл `products.txt` (`File.ReadAllLines`). Тому реальні помилки файлової системи (немає папки, немає файлу) теж можливі.
- `GetProductsFromApi` спершу перевіряє адресу (некоректна → `ArgumentException`), потім імітує затримку мережі 50 мс і перші 3 виклики кидає `HttpRequestException` (503 Service Unavailable).

### shouldRetry для варіанта 11

```csharp
static bool ShouldRetry(Exception ex) => ex is FileNotFoundException or HttpRequestException;
```

## 2. Сценарії (`initialDelay` = 200 мс, `retryCount` = 3)

| № | Сценарій | Що сталося | Спроб | Час |
|---|---|---|---:|---:|
| 1 | `LoadProductNames`: 2 × `FileNotFoundException` | затримки 200 → 400 мс, **успіх на 3-й спробі**, прочитано 5 продуктів з файлу | 3 | 0,66 с |
| 2 | `GetProductsFromApi`: 3 × `HttpRequestException` | затримки 200 → 400 → 800 мс, **успіх на 4-й (останній) спробі**, отримано 5 продуктів | 4 | 1,83 с |
| 3 | файл у неіснуючій папці | `DirectoryNotFoundException` (реальна помилка `File.ReadAllLines`) — `shouldRetry = false`, **без повторів** | 1 | 0,02 с |
| 4 | некоректна адреса API | `ArgumentException` — `shouldRetry = false`, **без повторів**, до мережі запит не дійшов | 1 | 0,00 с |
| 5 | файлу `missing.txt` справді немає | реальний `FileNotFoundException`, затримки 200 → 400 → 800 мс, **усі 4 спроби невдалі**, виняток перехоплено в `Main` | 4 | 1,43 с |

Кожен сценарій обгорнуто в `try-catch-finally`, і блок `finally` виконувався завжди: і після успіху, і після помилки.

Сценарій 3 показує вибіркову обробку: `DirectoryNotFoundException` теж належить до `IOException`, але `shouldRetry` повторює лише `FileNotFoundException`, тому відсутня папка, яка не з'явиться сама, не повторюється. Сценарій 5 показує межу патерну: `FileNotFoundException` дозволено повторювати, але коли файлу справді немає, повтори лише додають 1,4 с очікування, і після вичерпання спроб помилка все одно передається далі.

## 3. Висновок про експоненційну затримку

Затримка перед кожним повтором обчислюється за формулою `initialDelay × 2^retryAttempt`, тобто подвоюється: у варіанті 11 це 200 мс, 400 мс і 800 мс, а сумарне очікування при трьох повторах — 1,4 с. Якщо збій короткочасний (як файл, що синхронізується), перший повтор відбувається швидко і операція завершується за частку секунди. Якщо збій затягується (як API, недоступний перші 3 запити), паузи зростають, і сервер отримує все менше повторних запитів, доки не відновиться.

## 4. Контрольні запитання

**1. Які типи винятків найчастіше виникають при роботі з файлами та мережею?**
Файли: `FileNotFoundException` (файл не існує), `DirectoryNotFoundException` (немає папки), `IOException` (файл заблокований іншим процесом, диск заповнений), `UnauthorizedAccessException` (немає прав доступу), `PathTooLongException`. Мережа: `HttpRequestException` (сервер недоступний або повернув помилку), `SocketException` (не вдалося встановити з'єднання), `TaskCanceledException` / `TimeoutException` (вичерпано час очікування), `WebException`.

**2. Принцип роботи патерну Retry. Коли його доцільно використовувати?**
Retry повторює операцію, що завершилась помилкою, кілька разів із паузою між спробами, сподіваючись, що помилка тимчасова. Доцільний для **тимчасових (transient)** збоїв: короткочасна недоступність сервера (503), обрив мережі, файл, тимчасово заблокований іншим процесом. Недоцільний для постійних помилок (некоректні дані, відсутні права, неіснуюча папка) і небезпечний для неідемпотентних операцій (оплата, надсилання листа), бо повтор може виконати дію двічі.

**3. Як реалізувати експоненційну затримку між повторними спробами?**
Затримку обчислюють як `initialDelay × 2^retryAttempt`, де `retryAttempt` — номер повтору від 0: `TimeSpan.FromMilliseconds(initialDelay.TotalMilliseconds * Math.Pow(2, retryAttempt))`, після чого чекають `Thread.Sleep(delay)` (в асинхронному коді — `await Task.Delay(delay)`). На практиці додають jitter (випадкове відхилення), щоб повтори різних клієнтів не збігалися в часі, і верхню межу затримки.

**4. Для чого потрібен делегат shouldRetry?**
`shouldRetry` визначає, які винятки варто повторювати, а які слід одразу передати далі. Це відділяє тимчасові помилки від постійних: у варіанті 11 повторюються лише `FileNotFoundException` і `HttpRequestException`, а `DirectoryNotFoundException` і `ArgumentException` прокидаються одразу (сценарії 3–4). Без нього `RetryHelper` даремно чекав би й повторював операції, які гарантовано завершаться тією самою помилкою. Оскільки це делегат, правило задається ззовні, і той самий `RetryHelper` можна використовувати з різними політиками для різних операцій.

## 5. Вивід програми

```
===== 1. FileProcessor.LoadProductNames: 2 збої, потім успіх =====
Файл: C:\Users\User\AppData\Local\Temp\lab12v11\products.txt
   [ 0,00 с] Спроба 1/4...
   [ 0,03 с] Спроба 1: НЕВДАЧА — FileNotFoundException: Файл ще не синхронізовано з мережевим диском (виклик #1)
   [ 0,03 с] Очікування 200 мс (200 × 2^0) перед повтором #1
   [ 0,25 с] Спроба 2/4...
   [ 0,25 с] Спроба 2: НЕВДАЧА — FileNotFoundException: Файл ще не синхронізовано з мережевим диском (виклик #2)
   [ 0,25 с] Очікування 400 мс (200 × 2^1) перед повтором #2
   [ 0,65 с] Спроба 3/4...
   [ 0,66 с] Спроба 3: УСПІХ
Результат (5 продуктів, викликів: 3):
   - Смартфон Samsung A55
   - Планшет Lenovo Tab
   - Повербанк Xiaomi 20000
   - Смарт-годинник Amazfit
   - Колонка JBL Go 4
finally: роботу з файлом завершено

===== 2. NetworkClient.GetProductsFromApi: 3 збої, потім успіх =====
   [ 0,00 с] Спроба 1/4...
   [ 0,11 с] Спроба 1: НЕВДАЧА — HttpRequestException: 503 Service Unavailable від api.shop.example (виклик #1)
   [ 0,13 с] Очікування 200 мс (200 × 2^0) перед повтором #1
   [ 0,35 с] Спроба 2/4...
   [ 0,45 с] Спроба 2: НЕВДАЧА — HttpRequestException: 503 Service Unavailable від api.shop.example (виклик #2)
   [ 0,46 с] Очікування 400 мс (200 × 2^1) перед повтором #2
   [ 0,88 с] Спроба 3/4...
   [ 0,94 с] Спроба 3: НЕВДАЧА — HttpRequestException: 503 Service Unavailable від api.shop.example (виклик #3)
   [ 0,94 с] Очікування 800 мс (200 × 2^2) перед повтором #3
   [ 1,75 с] Спроба 4/4...
   [ 1,83 с] Спроба 4: УСПІХ
Результат (5 продуктів, викликів: 4):
   - Ноутбук Lenovo IdeaPad
   - Мишка Logitech M185
   - Монітор Dell 24"
   - Клавіатура A4Tech
   - Навушники JBL Tune 510
finally: з'єднання з API закрито

===== 3. shouldRetry: DirectoryNotFoundException не повторюється =====
   [ 0,00 с] Спроба 1/4...
   [ 0,01 с] Спроба 1: НЕВДАЧА — DirectoryNotFoundException: Could not find a part of the path 'C:\Users\User\AppData\Local\Temp\lab12v11\no_such_folder\products.txt'.
   [ 0,02 с] DirectoryNotFoundException не підлягає повтору (shouldRetry = false)
Main перехопив DirectoryNotFoundException без повторів (викликів: 1)
   Could not find a part of the path 'C:\Users\User\AppData\Local\Temp\lab12v11\no_such_folder\products.txt'.
finally: роботу з файлом завершено

===== 4. shouldRetry: ArgumentException не повторюється =====
   [ 0,00 с] Спроба 1/4...
   [ 0,00 с] Спроба 1: НЕВДАЧА — ArgumentException: Некоректна адреса API: «api.shop/products» (Parameter 'url')
   [ 0,00 с] ArgumentException не підлягає повтору (shouldRetry = false)
Main перехопив ArgumentException без повторів (мережевих викликів: 0): Некоректна адреса API: «api.shop/products» (Parameter 'url')
finally: з'єднання з API закрито

===== 5. Файлу немає: FileNotFoundException після всіх спроб =====
   [ 0,00 с] Спроба 1/4...
   [ 0,00 с] Спроба 1: НЕВДАЧА — FileNotFoundException: Could not find file 'C:\Users\User\AppData\Local\Temp\lab12v11\missing.txt'.
   [ 0,00 с] Очікування 200 мс (200 × 2^0) перед повтором #1
   [ 0,20 с] Спроба 2/4...
   [ 0,21 с] Спроба 2: НЕВДАЧА — FileNotFoundException: Could not find file 'C:\Users\User\AppData\Local\Temp\lab12v11\missing.txt'.
   [ 0,21 с] Очікування 400 мс (200 × 2^1) перед повтором #2
   [ 0,62 с] Спроба 3/4...
   [ 0,62 с] Спроба 3: НЕВДАЧА — FileNotFoundException: Could not find file 'C:\Users\User\AppData\Local\Temp\lab12v11\missing.txt'.
   [ 0,62 с] Очікування 800 мс (200 × 2^2) перед повтором #3
   [ 1,43 с] Спроба 4/4...
   [ 1,43 с] Спроба 4: НЕВДАЧА — FileNotFoundException: Could not find file 'C:\Users\User\AppData\Local\Temp\lab12v11\missing.txt'.
   [ 1,43 с] Вичерпано всі 4 спроби
Main перехопив FileNotFoundException після 4 спроб: Could not find file 'C:\Users\User\AppData\Local\Temp\lab12v11\missing.txt'.
finally: роботу з файлом завершено
```