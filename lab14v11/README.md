# Лабораторна робота №14. Аналіз SRP/OCP у відкритих репозиторіях

**Виконала:** Мирончук Ірина, група ІПЗ-3/1

## Обрані репозиторії

| Репозиторій | Домен | Що аналізувалось |
|---|---|---|
| [dotnet/runtime](https://github.com/dotnet/runtime) | бібліотека серіалізації JSON (System.Text.Json) | конвертери, налаштування серіалізатора, розбір JSON-документа |
| [dotnet/aspnetcore](https://github.com/dotnet/aspnetcore) | вебфреймворк (MVC, автентифікація) | обробники автентифікації, базовий клас контролера |

Номери рядків вказано за станом на 14.11.2026 (гілка `main`, перевірено через [source.dot.net](https://source.dot.net), де показано той самий код із нумерацією рядків). У нових комітах рядки можуть зміститися, тому поруч із кожним посиланням на GitHub наведено назву класу або методу.

## Зведена таблиця

| № | Принцип | Оцінка | Репозиторій | Клас / файл |
|---|---|---|---|---|
| 1 | SRP | вдале застосування | runtime | `JsonDocument.PropertyNameSet` |
| 2 | SRP | вдале застосування | aspnetcore | `AuthenticationHandlerProvider` |
| 3 | SRP | порушення | aspnetcore | `ControllerBase` |
| 4 | SRP | порушення | runtime | `JsonSerializerOptions` |
| 5 | OCP | розширення через абстракцію | runtime | `JsonConverter<T>` / `JsonConverterFactory` |
| 6 | OCP | розширення через шаблонний метод | aspnetcore | `AuthenticationHandler<TOptions>` |

---

## 1. SRP — вдале застосування

### 1.1. `JsonDocument.PropertyNameSet` (dotnet/runtime)

- **Файл:** [`src/libraries/System.Text.Json/src/System/Text/Json/Document/JsonDocument.PropertyNameSet.cs`](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Text.Json/src/System/Text/Json/Document/JsonDocument.PropertyNameSet.cs#L15) ([source.dot.net](https://source.dot.net/System.Text.Json/System/Text/Json/Document/JsonDocument.PropertyNameSet.cs.html))
- **Де видно принцип:** `private ref struct PropertyNameSet` (рядок 15) має одну відповідальність — перевірити, що в JSON-об'єкті немає двох властивостей з однаковим іменем. Увесь файл займає 161 рядок: поріг `ArraySetThreshold` (рядок 24), `AddPropertyName` (рядок 70), `SwitchToHashSet` (рядок 111) і вкладений `PropertyNameComparer` (рядок 144). Для малих об'єктів використовується масив, для великих — `HashSet` з власним comparer, але ця деталь схована всередині структури.
- **Переваги для підтримки:** `JsonDocument` не знає, як саме шукаються дублікати. Логіку можна оптимізувати (змінити поріг, алгоритм хешування) і протестувати окремо, не чіпаючи розбір JSON.
- **Ризики:** мінімальні. Структура `private`, тож її не можна перевикористати поза `JsonDocument`, але для такої вузької задачі це свідоме обмеження.
- **При додаванні нової функції** (наприклад, режиму «дозволити дублікати, брати останнє значення») зміни стосуватимуться лише цього файлу та місця його виклику.

### 1.2. `AuthenticationHandlerProvider` (dotnet/aspnetcore)

- **Файл:** [`src/Http/Authentication.Core/src/AuthenticationHandlerProvider.cs`](https://github.com/dotnet/aspnetcore/blob/main/src/Http/Authentication.Core/src/AuthenticationHandlerProvider.cs#L12) ([source.dot.net](https://source.dot.net/Microsoft.AspNetCore.Authentication.Core/AuthenticationHandlerProvider.cs.html))
- **Де видно принцип:** клас (рядок 12, весь файл — 59 рядків) робить лише одне: за назвою схеми повертає екземпляр обробника автентифікації (`GetHandlerAsync`, рядок 37). Пошук схеми делеговано `IAuthenticationSchemeProvider` (рядок 44), створення обробника — DI-контейнеру (рядки 49–51), а сама автентифікація — обробнику.
- **Переваги:** клас легко зрозуміти й протестувати, бо всі залежності приходять через інтерфейси. Його можна замінити власною реалізацією `IAuthenticationHandlerProvider`, не змінюючи решти системи.
- **Ризики:** кеш `_handlerMap` (рядок 29) розрахований на один запит, тому клас має бути зареєстрований із часом життя scoped. Це неочевидне правило, яке видно лише з коментаря.
- **При додаванні нової схеми автентифікації** цей клас не змінюється взагалі: нова схема реєструється в `IAuthenticationSchemeProvider`, а провайдер просто створить її обробник.

---

## 2. SRP — порушення

### 2.1. `ControllerBase` (dotnet/aspnetcore)

- **Файл:** [`src/Mvc/Mvc.Core/src/ControllerBase.cs`](https://github.com/dotnet/aspnetcore/blob/main/src/Mvc/Mvc.Core/src/ControllerBase.cs) ([source.dot.net](https://source.dot.net/Microsoft.AspNetCore.Mvc.Core/ControllerBase.cs.html))
- **Де видно порушення:** файл має понад 2000 рядків і понад 110 публічних перевантажень методів (близько 25 різних назв). Клас одночасно відповідає за:
  1. **доступ до контексту запиту:** `Request` (рядок 42), `Response` (рядок 47), `User` (рядок 206), `RouteData`, `ModelState`;
  2. **створення HTTP-відповідей:** `Ok`, `BadRequest`, `NotFound`, `Content`, `StatusCode`, `Problem`, `ValidationProblem`, `CreatedAtAction`;
  3. **перенаправлення:** десятки перевантажень `Redirect*`, `RedirectToAction*`, `RedirectToRoute*`;
  4. **повернення файлів:** перевантаження `File(...)`;
  5. **автентифікацію:** `Challenge`, `Forbid`, `SignIn`, `SignOut`;
  6. **зв'язування й валідацію моделей:** `TryUpdateModelAsync`, `TryValidateModel` (кінець файлу, звертання до `HttpContext` на рядках 1882 і 2004).

  Це шість різних причин для змін: нова форма відповіді, нова схема автентифікації, зміни в model binding — усе це змінює один клас.
- **Ризики для підтримки:** будь-яка зміна зачіпає базовий клас усіх контролерів у всіх застосунках. Додати метод у `ControllerBase` можна лише з великою обережністю, щоб не зламати контролери, де вже є метод з такою самою назвою. Клас важко тестувати ізольовано, а IntelliSense у контролері показує понад сотню методів, більшість з яких конкретному контролеру не потрібна.
- **Переваги (чому так зроблено):** зручність для розробника — `return Ok(x);` без додаткових залежностей — і зворотна сумісність з ASP.NET MVC 5. Це свідомий компроміс, а не помилка.
- **При додаванні нової функції** (наприклад, нового типу відповіді) Microsoft доводиться додавати ще одне перевантаження в цей клас. Показово, що в Minimal API ці ж відповіді винесено в окремі статичні класи `Results` / `TypedResults`, тобто сама команда ASP.NET Core в новому API відокремила цю відповідальність.

### 2.2. `JsonSerializerOptions` (dotnet/runtime)

- **Файл:** [`src/libraries/System.Text.Json/src/System/Text/Json/Serialization/JsonSerializerOptions.cs`](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Text.Json/src/System/Text/Json/Serialization/JsonSerializerOptions.cs#L22) ([source.dot.net](https://source.dot.net/System.Text.Json/System/Text/Json/Serialization/JsonSerializerOptions.cs.html)) та partial-файл [`JsonSerializerOptions.Caching.cs`](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Text.Json/src/System/Text/Json/Serialization/JsonSerializerOptions.Caching.cs) ([source.dot.net](https://source.dot.net/System.Text.Json/System/Text/Json/Serialization/JsonSerializerOptions.Caching.cs.html))
- **Де видно порушення:** клас `public sealed partial class JsonSerializerOptions` (рядок 22, близько 1350 рядків в основному файлі, 33 публічні властивості) за назвою — «налаштування», але фактично виконує щонайменше 10 різних ролей: попередньо визначені набори (`Default`, `Web`, `Strict`), власне налаштування, копіювання, розв'язання метаданих типів (`TypeInfoResolverChain`, близько рядка 265), списки конвертерів, незмінність (`MakeReadOnly`, близько рядка 1039), кешування метаданих (вкладений клас `CachingContext` у `JsonSerializerOptions.Caching.cs`), створення налаштувань для reader/writer, сумісність із reflection-режимом і діагностику.
- **Ризики:** об'єкт налаштувань має складний життєвий цикл. Після першої серіалізації він стає незмінним, і спроба змінити властивість кидає виняток. Через вбудований кеш новий об'єкт налаштувань, створений на кожен виклик, суттєво сповільнює роботу — типова помилка користувачів. Partial-файли лише ділять код фізично, але відповідальності лишаються в одному класі.
- **Переваги (чому так зроблено):** продуктивність — кеш метаданих прив'язаний саме до екземпляра налаштувань, тож вони гарантовано узгоджені — і простий публічний API (один об'єкт, який передають у `Serialize`).
- **При додаванні нової функції** (нової опції серіалізації) треба змінювати і властивості, і копіювальний конструктор, і перевірку незмінності, і порівняння для кешу, тобто кілька місць в одному великому класі.

---

## 3. OCP — розширення без зміни коду

### 3.1. `JsonConverter<T>` і `JsonConverterFactory` (dotnet/runtime)

- **Файли:** [`JsonConverterOfT.cs`](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Text.Json/src/System/Text/Json/Serialization/JsonConverterOfT.cs#L15) ([source.dot.net](https://source.dot.net/System.Text.Json/System/Text/Json/Serialization/JsonConverterOfT.cs.html)), [`JsonConverterFactory.cs`](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Text.Json/src/System/Text/Json/Serialization/JsonConverterFactory.cs#L15) ([source.dot.net](https://source.dot.net/System.Text.Json/System/Text/Json/Serialization/JsonConverterFactory.cs.html))
- **Де видно принцип:** `public abstract partial class JsonConverter<T> : JsonConverter` (рядок 15) задає точку розширення з двома абстрактними методами: `Read(ref Utf8JsonReader, Type, JsonSerializerOptions)` (рядок 147) і `Write(Utf8JsonWriter, T, JsonSerializerOptions)` (рядок 593). `JsonConverterFactory` (рядок 15) дозволяє створювати конвертери для цілих сімейств типів (наприклад, усіх `List<T>`). Користувач додає свій конвертер через `options.Converters.Add(new MyConverter())` або атрибут `[JsonConverter]`, а серіалізатор використовує його, **не змінюючись**. Самі вбудовані конвертери бібліотеки (enum, колекції, F#-типи) побудовані на тій самій абстракції.
- **Переваги:** підтримку нового типу (наприклад, грошової суми з валютою чи дати в нестандартному форматі) додають окремим класом. Ядро серіалізатора закрите для змін, але відкрите для розширення.
- **Ризики:** базовий клас має 703 рядки й багато внутрішньої логіки (поліморфізм, продовження читання з потоку). Власний конвертер мусить правильно працювати з `Utf8JsonReader`, і помилка в ньому ламає всю серіалізацію цього типу.
- **При додаванні нової функції:** новий тип → новий клас `JsonConverter<T>` + одна реєстрація; код System.Text.Json не змінюється.

### 3.2. `AuthenticationHandler<TOptions>` (dotnet/aspnetcore)

- **Файл:** [`src/Security/Authentication/Core/src/AuthenticationHandler.cs`](https://github.com/dotnet/aspnetcore/blob/main/src/Security/Authentication/Core/src/AuthenticationHandler.cs#L16) ([source.dot.net](https://source.dot.net/Microsoft.AspNetCore.Authentication/AuthenticationHandler.cs.html))
- **Де видно принцип:** абстрактний клас (рядок 16, 333 рядки) реалізує патерн «шаблонний метод». Публічний `AuthenticateAsync()` (рядок 215) містить спільну логіку (одноразовий виклик, логування, обробка помилок) і викликає `HandleAuthenticateAsync()` (рядок 252), оголошений як `protected abstract` (рядок 279). Поведінку відмови доступу можна змінити через `virtual HandleForbiddenAsync` (рядок 286) і `HandleChallengeAsync` (рядок 298). Кожна схема — окремий нащадок: `CookieAuthenticationHandler` перевизначає `HandleAuthenticateAsync` у [`CookieAuthenticationHandler.cs`](https://source.dot.net/Microsoft.AspNetCore.Authentication.Cookies/CookieAuthenticationHandler.cs.html) (рядок 200), `JwtBearerHandler` — у [`JwtBearerHandler.cs`](https://source.dot.net/Microsoft.AspNetCore.Authentication.JwtBearer/JwtBearerHandler.cs.html) (рядок 56). Нова схема підключається через `AuthenticationBuilder.AddScheme<TOptions, THandler>()`, де `THandler : AuthenticationHandler<TOptions>`.
- **Переваги:** щоб додати вхід через API-ключ чи сертифікат, достатньо написати один клас із методом `HandleAuthenticateAsync` і зареєструвати його. Ні `AuthenticationHandler`, ні middleware, ні інші схеми не змінюються. Спільна логіка написана один раз.
- **Ризики:** наслідування жорстко прив'язує нащадка до базового класу: зміна протоколу базового класу (порядок викликів, обов'язковий конструктор із `IOptionsMonitor`, `ILoggerFactory`, `UrlEncoder`) впливає на всі схеми, включно зі сторонніми.
- **При додаванні нової функції** (нової схеми автентифікації) змінюється лише реєстрація в `Program.cs`, а існуючий код — ні.

---

## 4. Висновки

1. В обох репозиторіях OCP послідовно реалізовано через абстрактні базові класи з чітко визначеними точками розширення (`JsonConverter<T>.Read/Write`, `AuthenticationHandler.HandleAuthenticateAsync`), тому нові типи й нові схеми додаються новими класами без зміни ядра.
2. Вдале SRP найчастіше трапляється в невеликих внутрішніх або інфраструктурних класах (`PropertyNameSet` — 161 рядок, `AuthenticationHandlerProvider` — 59 рядків), які роблять одну річ і делегують решту через інтерфейси.
3. Порушення SRP зосереджені в «фасадних» класах публічного API (`ControllerBase`, `JsonSerializerOptions`), які мають бути зручними для мільйонів користувачів і зберігати зворотну сумісність.
4. Такі порушення — свідомі компроміси: зручність (`return Ok()`) і продуктивність (кеш у налаштуваннях) переважили чистоту дизайну, а розділення зламало б існуючий код.
5. Ознака порушення SRP — не лише розмір класу, а кількість незалежних причин для змін: `ControllerBase` змінюється і через нові типи відповідей, і через автентифікацію, і через model binding.
6. Partial-класи (`JsonSerializerOptions.Caching.cs`) організовують код фізично, але не розділяють відповідальності: клас залишається один.
7. Сама команда ASP.NET Core у новішому API (Minimal API, `Results`/`TypedResults`) винесла створення відповідей в окремий клас. Це підтверджує, що розділення відповідальностей дає перевагу, коли немає обмежень сумісності.
8. Під час аналізу важливо дивитися на точки розширення (`AddScheme`, `options.Converters`), а не лише на наявність інтерфейсів: саме вони показують, чи можна додати поведінку без редагування бібліотеки.

## 5. Пропозиція рефакторингу для найгіршого прикладу: `ControllerBase`

**Проблема:** один базовий клас понад 2000 рядків поєднує 6 відповідальностей, і кожна нова можливість MVC додає в нього нові методи.

**Пропозиція:** залишити в `ControllerBase` лише доступ до контексту (`HttpContext`, `Request`, `Response`, `User`, `ModelState`), а решту винести в окремі сервіси з однією відповідальністю, доступні через властивості або DI:

```csharp
public abstract class ControllerBase
{
    public ControllerContext ControllerContext { get; set; }
    public HttpContext HttpContext => ControllerContext.HttpContext;
    public ModelStateDictionary ModelState => ControllerContext.ModelState;

    // фабрики відповідей — окремий клас з однією відповідальністю
    protected ActionResults Results { get; } = new();
}

public sealed class ActionResults                 // створення HTTP-відповідей
{
    public OkObjectResult Ok(object? value) => new(value);
    public NotFoundResult NotFound() => new();
    public RedirectToActionResult RedirectToAction(string action) => new(action, null, null);
}

public interface IModelUpdater                    // зв'язування й валідація моделей
{
    Task<bool> TryUpdateModelAsync<T>(T model, ControllerContext context) where T : class;
    bool TryValidateModel(object model, ControllerContext context);
}
```

Автентифікаційні методи (`Challenge`, `SignIn`, `SignOut`) вже мають еквіваленти в `HttpContext.ChallengeAsync()` / `SignInAsync()` (сервіс `IAuthenticationService`), тому їх у базовому класі можна позначити застарілими.

**Обґрунтування:**
- кожен новий клас має одну причину для змін: новий тип відповіді змінює лише `ActionResults`, зміни в model binding — лише реалізацію `IModelUpdater`;
- класи можна тестувати окремо, а в контролері видно лише те, що йому потрібно;
- `IModelUpdater` можна замінити своєю реалізацією (OCP), не наслідуючи `ControllerBase`;
- підхід уже перевірений у самому ASP.NET Core: `Results` / `TypedResults` у Minimal API.

**Ціна:** зміна торкнулася б усіх існуючих контролерів, тому реалістичний шлях — додати нові класи паралельно, залишити старі методи як обгортки й поступово позначати їх `[Obsolete]`. Без такої поступовості рефакторинг «під патерн» зламав би сумісність, і його ціна перевищила б користь.

## 6. Контрольні запитання

**1. Як відрізнити окрему відповідальність класу від просто великого обсягу коду?**
Відповідальність — це причина для змін (актор, вимога), а не кількість рядків. Великий клас може мати одну відповідальність, якщо всі його методи змінюються з однієї причини: так, `JsonConverterOfT.cs` має 703 рядки, але весь цей код про одне — конвертацію значення типу `T`. Ознаки кількох відповідальностей: групи методів, які не використовують спільних полів; різні «словники» в назвах (Redirect…, SignIn…, TryUpdateModel…); зміни з різних причин в одному файлі в історії комітів.

**2. Які ознаки в коді найкраще підтверджують дотримання OCP?**
Абстрактні класи або інтерфейси з точками розширення (`abstract`/`virtual` методи), механізм реєстрації реалізацій (`options.Converters.Add`, `AddScheme<THandler>`), відсутність `switch`/`if` за типом, який довелося б доповнювати для кожного нового варіанта, і наявність кількох незалежних реалізацій однієї абстракції (Cookie, JwtBearer, Certificate для `AuthenticationHandler`).

**3. Чому для аналізу SRP/OCP важливо дивитися не лише на інтерфейси, а й на місця розширення системи?**
Інтерфейс сам по собі не гарантує розширюваності: якщо систему не можна налаштувати на нову реалізацію без редагування коду (немає реєстрації, DI, фабрики), OCP фактично не виконується. Точки розширення (`AddScheme`, `Converters`) показують, як нову поведінку реально підключають. Крім того, місця розширення показують, де накопичується зайва логіка: базовий клас із надто великою кількістю `virtual`-методів може порушувати SRP.

**4. Які ризики виникають, якщо застосовувати рефакторинг «під патерн» без реальної потреби?**
Зайві абстракції ускладнюють код: замість одного зрозумілого класу з'являються інтерфейси, фабрики й стратегії з однією реалізацією (over-engineering). Код стає важче читати й налагоджувати, зростає кількість файлів і непрямих викликів. У публічних бібліотеках рефакторинг може зламати зворотну сумісність для користувачів. Рефакторинг виправданий, коли є реальна причина: кілька варіантів поведінки, часті зміни в одному місці, проблеми з тестуванням.