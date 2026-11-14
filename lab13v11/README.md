# Лабораторна робота №13. Власні винятки та патерн Result

**Виконала:** Мирончук Ірина, група ІПЗ-3/1
**Варіант 11:** «Обробка платежів»

Запуск:
```bash
dotnet run --project lab13v11
```

| Файл | Призначення |
|---|---|
| `PaymentGatewayException.cs` | власний виняток з властивостями `GatewayName`, `ErrorCode` |
| `Result.cs` | узагальнений клас `Result<T>` |
| `PaymentService.cs` | сервіс платежів і клас `Transaction` |
| `Program.cs` | демонстрація обох підходів |

## 1. Власний виняток `PaymentGatewayException`

```csharp
[Serializable]
public class PaymentGatewayException : Exception
{
    public string GatewayName { get; }
    public string ErrorCode { get; }

    public PaymentGatewayException() : base("Платіжний шлюз повернув помилку.") { }
    public PaymentGatewayException(string message) : base(message) { }
    public PaymentGatewayException(string message, Exception innerException) : base(message, innerException) { }

    public PaymentGatewayException(string message, string gatewayName, string errorCode) : base(message) { ... }
    public PaymentGatewayException(string message, string gatewayName, string errorCode, Exception innerException) { ... }

    [Obsolete(...)] protected PaymentGatewayException(SerializationInfo info, StreamingContext context) { ... }
    [Obsolete(...)] public override void GetObjectData(SerializationInfo info, StreamingContext context) { ... }
}
```

- успадкований від `Exception`, назва закінчується на `Exception`;
- **три стандартні конструктори**: без параметрів, з `message`, з `message` + `innerException`;
- **додаткові властивості** `GatewayName` (назва шлюзу) і `ErrorCode` (код помилки: `LIMIT_EXCEEDED`, `INSUFFICIENT_FUNDS`, `GATEWAY_UNAVAILABLE`) та конструктори, що їх заповнюють;
- **конструктор для серіалізації** і `GetObjectData` зберігають додаткові властивості. У .NET 8+ серіалізація через `BinaryFormatter` застаріла, тому ці члени позначено `[Obsolete]` з кодом `SYSLIB0051`, як рекомендує Microsoft.

## 2. Узагальнений клас `Result<T>`

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }

    private Result(T? value, bool isSuccess, string? error) { ... }

    public static Result<T> Success(T value)      => new(value, true, null);
    public static Result<T> Failure(string error) => new(default, false, error);
    public static Result<T> Failure(Exception ex) => Failure($"{ex.GetType().Name}: {ex.Message}");
}
```

Конструктор приватний, тому об'єкт можна створити лише через `Success` або `Failure`. Це гарантує узгоджений стан: успіх завжди має значення, а невдача — опис помилки (порожній опис не допускається).

## 3. `PaymentService`

| Метод | Підхід | Коли невдача |
|---|---|---|
| `void ProcessPayment(decimal amount)` | **виняток** `PaymentGatewayException` | шлюз недоступний (`GATEWAY_UNAVAILABLE`), сума більша за ліміт 50 000 грн (`LIMIT_EXCEEDED`), банк відхилив через нестачу коштів (`INSUFFICIENT_FUNDS`); від'ємна сума → `ArgumentOutOfRangeException` |
| `Result<bool> RefundPayment(string transactionId)` | **Result** | порожній ідентифікатор, транзакцію не знайдено, транзакцію вже повернено |
| `Result<string> TryProcessPayment(decimal amount)` | виняток → **Result** | перехоплює `PaymentGatewayException` і повертає `Result<string>.Failure(ex)` |

## 4. Демонстрація

**ProcessPayment + try-catch** (баланс картки 10 000 грн). Блоки `catch` йдуть від специфічного до загального, а фільтр `when` окремо обробляє недоступний шлюз:

```csharp
catch (PaymentGatewayException ex) when (ex.ErrorCode == "GATEWAY_UNAVAILABLE") { ... }
catch (PaymentGatewayException ex) { ... }
catch (ArgumentOutOfRangeException ex) { ... }
finally { ... }
```

| Виклик | Результат |
|---|---|
| `ProcessPayment(1 500)` | успіх, транзакція TX-0001, баланс 8 500 грн |
| `ProcessPayment(60 000)` | `PaymentGatewayException` [LiqPay, LIMIT_EXCEEDED] |
| `ProcessPayment(12 000)` | `PaymentGatewayException` [LiqPay, INSUFFICIENT_FUNDS] |
| `ProcessPayment(-5)` | `ArgumentOutOfRangeException` |
| `ProcessPayment(100)`, шлюз недоступний | `PaymentGatewayException` [LiqPay, GATEWAY_UNAVAILABLE], причина з `InnerException`: «Немає відповіді за 30 с» |

`finally` виконався в усіх п'яти випадках.

**RefundPayment + Result<bool>** — обробка через `if (result.IsSuccess) … else result.Error`, без `try-catch`:

| Виклик | Результат |
|---|---|
| `RefundPayment("TX-0001")` | УСПІХ, баланс знову 10 000 грн |
| `RefundPayment("TX-0001")` повторно | ВІДМОВА: транзакцію вже повернено раніше |
| `RefundPayment("TX-9999")` | ВІДМОВА: транзакцію не знайдено |
| `RefundPayment("")` | ВІДМОВА: не вказано ідентифікатор |

**Result<T>.Failure(Exception)**: `TryProcessPayment(2 000)` → `Success(TX-0002)`, `TryProcessPayment(75 000)` → `Failure(PaymentGatewayException: Сума 75 000,00 грн перевищує ліміт шлюзу 50 000 грн.)`.

## 5. Висновок: виняток чи Result для методів варіанта

Для `ProcessPayment` обрано **виняток**, бо помилка платіжного шлюзу — виняткова ситуація, що виникає поза нашим кодом (зовнішній сервіс не відповів або відхилив операцію). Метод не може виконати свій контракт «гроші списано», а пропустити таку помилку неприпустимо: виняток неможливо проігнорувати, він несе код помилки й причину (`InnerException`), і його можна обробити на рівні, який знає, що робити (повторити, запропонувати інший спосіб оплати). Для `RefundPayment` обрано **Result**, бо неіснуюча чи вже повернена транзакція — **очікуваний** сценарій (користувач ввів неправильний номер, натиснув кнопку двічі). Це не аварія, а звичайна відповідь, яку видно в сигнатурі методу й обробляють простим `if` без дорогого розкручування стека. Метод `TryProcessPayment` показує, що підходи поєднуються: на межі шарів виняток можна перетворити на `Result` через `Failure(Exception)`.

## 6. Контрольні запитання

**1. Навіщо створювати власні типи винятків? Які переваги це дає?**
Власний тип дозволяє перехоплювати саме цю помилку (`catch (PaymentGatewayException)`), не чіпаючи інших, і відрізняти помилки бізнес-логіки від системних. Додаткові властивості (`GatewayName`, `ErrorCode`) передають контекст помилки в структурованому вигляді, тож обробнику не треба розбирати текст повідомлення, а можна приймати рішення за кодом, як у фільтрі `when (ex.ErrorCode == "GATEWAY_UNAVAILABLE")`. Назва типу також документує, що саме може піти не так.

**2. Які стандартні конструктори рекомендується реалізовувати?**
Три: без параметрів, з `string message` і з `string message, Exception innerException` (останній зберігає першопричину). Додатково — конструктори з власними властивостями. Для сумісності зі старим кодом — захищений конструктор серіалізації `(SerializationInfo, StreamingContext)` разом із `GetObjectData`; у .NET 8+ вони позначаються `[Obsolete]`, бо formatter-серіалізацію визнано застарілою.

**3. Поясніть патерн Result. Коли він доцільніший за винятки?**
Метод повертає об'єкт `Result<T>`, який містить або значення (`IsSuccess = true`, `Value`), або опис помилки (`IsSuccess = false`, `Error`). Можливість невдачі видно прямо в сигнатурі, а обробка йде звичайним `if`. Доцільний для очікуваних неуспіхів, що є частиною нормальної роботи: валідація введення, пошук елемента, якого може не бути, перевірка бізнес-правила. Він також швидший за винятки при частих невдачах, бо не розкручує стек.

**4. Які сценарії краще обробляти через винятки, а які через Result?**
**Винятки** — для справді виняткових ситуацій, коли метод не може виконати контракт: збій зовнішнього сервісу чи мережі, втрачене з'єднання з БД, пошкоджені дані, помилки програміста (неправильні аргументи). **Result** — для передбачуваних «негативних» відповідей: неправильні дані користувача, відсутній запис, порушене бізнес-правило (повернення вже поверненого платежу). Правило: якщо невдача — звичайна ситуація, яку викликач має обробити, повертаємо `Result`; якщо це аварія, яку не можна пропустити, кидаємо виняток.

## 7. Вивід програми

```
===== 1. ProcessPayment: обробка винятків через try-catch =====
Баланс картки: 10 000,00 грн
ProcessPayment(1 500,00)
   УСПІХ: транзакція TX-0001, баланс 8 500,00 грн
   finally: спробу платежу записано в журнал
ProcessPayment(60 000,00)
   PaymentGatewayException [LiqPay, LIMIT_EXCEEDED]: Сума 60 000,00 грн перевищує ліміт шлюзу 50 000 грн.
   finally: спробу платежу записано в журнал
ProcessPayment(12 000,00)
   PaymentGatewayException [LiqPay, INSUFFICIENT_FUNDS]: Банк відхилив платіж: на картці 8 500,00 грн, потрібно 12 000,00 грн.
   finally: спробу платежу записано в журнал
ProcessPayment(-5,00)
   ArgumentOutOfRangeException: Сума платежу має бути додатною. (Parameter 'amount')
   finally: спробу платежу записано в журнал

--- Шлюз недоступний ---
ProcessPayment(100,00)
   PaymentGatewayException [LiqPay, GATEWAY_UNAVAILABLE]: Шлюз LiqPay не відповідає.
   Причина: Немає відповіді за 30 с. Пропонуємо спробувати пізніше або інший спосіб оплати.
   finally: спробу платежу записано в журнал

===== 2. RefundPayment: обробка Result<bool> через IsSuccess / Error =====
RefundPayment(TX-0001): УСПІХ — кошти повернено, баланс 10 000,00 грн
RefundPayment(TX-0001): ВІДМОВА — Транзакцію TX-0001 вже повернено раніше.
RefundPayment(TX-9999): ВІДМОВА — Транзакцію TX-9999 не знайдено.
RefundPayment(""): ВІДМОВА — Не вказано ідентифікатор транзакції.

===== 3. TryProcessPayment: Result<string>.Failure(Exception) =====
TryProcessPayment(2 000): Success(TX-0002)
TryProcessPayment(75 000): Failure(PaymentGatewayException: Сума 75 000,00 грн перевищує ліміт шлюзу 50 000 грн.)
Підсумковий баланс картки: 8 000,00 грн

===== 4. Конструктори PaymentGatewayException =====
Message: Платіжний шлюз повернув помилку. | Gateway: — | Code: — | Inner: —
Message: Шлюз відхилив запит. | Gateway: — | Code: — | Inner: —
Message: Помилка під час платежу. | Gateway: — | Code: — | Inner: 502 Bad Gateway
Message: Картку заблоковано. | Gateway: Monobank Acquiring | Code: CARD_BLOCKED | Inner: —
```