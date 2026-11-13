# Самостійна робота №14. Dictionary<TKey, TValue> та HashSet<T> у бібліотеках .NET

**Виконала:** Мирончук Ірина, група ІПЗ-3/1

## 1. Аналіз використання в open-source проєктах

### Приклад 1. Dictionary<TKey, TValue> — Dapper, `typeMap`

- **Проєкт:** [Dapper](https://github.com/DapperLib/Dapper) — micro-ORM для .NET
- **Файл:** [Dapper/SqlMapper.cs](https://github.com/DapperLib/Dapper/blob/main/Dapper/SqlMapper.cs)

```csharp
static readonly Dictionary<Type, DbType> typeMap;

static SqlMapper()
{
    typeMap = new Dictionary<Type, DbType>();
    typeMap[typeof(int)] = DbType.Int32;
    typeMap[typeof(string)] = DbType.String;
    typeMap[typeof(DateTime)] = DbType.DateTime;
    
}


if (typeMap.TryGetValue(type, out dbType))
    return dbType;
```

- **Контекст (яка задача вирішується):** коли Dapper передає параметр у SQL-запит, йому треба знати, якому типу бази даних (`DbType`) відповідає тип .NET (`int` → `Int32`, `string` → `String` тощо). Словник `typeMap` зберігає цю відповідність.
- **Чому саме Dictionary:** це класична задача «ключ → значення». Метод `LookupDbType` викликається для кожного параметра кожного запиту, тому пошук має бути дуже швидким. У `List` довелося б перебирати всі ~35 пар, а Dictionary знаходить потрібну за хешем ключа.
- **Складність:** `TryGetValue` — O(1) в середньому. Словник заповнюється один раз у статичному конструкторі, далі тільки читається.
- **Особливості:** ключем є об'єкт `Type`, для якого порівняння за посиланням коректне (кожен тип існує в одному екземплярі), тому власний comparer не потрібен. Використовується `TryGetValue` замість індексатора: так немає винятку, якщо тип не знайдено, і виконується лише один пошук замість двох (`ContainsKey` + індексатор).

### Приклад 2. HashSet<T> — .NET Runtime (System.Text.Json), `JsonDocument.PropertyNameSet`

- **Проєкт:** [dotnet/runtime](https://github.com/dotnet/runtime), бібліотека System.Text.Json
- **Файл:** [JsonDocument.PropertyNameSet.cs](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Text.Json/src/System/Text/Json/Document/JsonDocument.PropertyNameSet.cs) ([переглянути на source.dot.net](https://source.dot.net/System.Text.Json/System/Text/Json/Document/JsonDocument.PropertyNameSet.cs.html))

```csharp
_hashSet = new HashSet<ReadOnlyMemory<byte>>(capacity, PropertyNameComparer.Instance);
...
if (!_hashSet.Add(propertyName))
{
    ThrowHelper.ThrowJsonException_DuplicatePropertyNotAllowed(propertyName.Span);
}
```

- **Контекст:** під час розбору JSON треба перевірити, що в одному об'єкті немає двох властивостей з однаковим іменем (наприклад, `{"a":1, "a":2}`). Якщо дубль знайдено, викидається `JsonException`.
- **Чому саме HashSet:** потрібно лише відповісти на питання «чи бачили ми вже це ім'я?», значення не потрібні, тож Dictionary тут зайвий. Метод `Add` повертає `false`, якщо елемент уже є, тобто перевірка й додавання робляться однією операцією.
- **Складність:** `Add` / перевірка наявності — O(1) в середньому. Для маленьких об'єктів (до 16 властивостей, константа `ArraySetThreshold`) розробники використовують простий масив з лінійним пошуком O(n): на малих n він швидший, бо не треба рахувати хеші. Для більших об'єктів код переходить на HashSet.
- **Особливості — custom comparer:** елементи мають тип `ReadOnlyMemory<byte>` (байти імені в UTF-8), який сам по собі не порівнюється за вмістом. Тому написано власний `PropertyNameComparer : IEqualityComparer<ReadOnlyMemory<byte>>`: `Equals` порівнює байти через `SequenceEqual`, а `GetHashCode` рахує хеш алгоритмом Marvin з випадковим для кожного процесу seed. Це захищає від атак, коли зловмисник підбирає ключі з однаковим хешем, щоб пошук деградував до O(n) (hash flooding).

### Висновок з аналізу

В обох бібліотеках колекцію обрано під конкретну задачу: Dictionary — коли треба швидко знайти **значення за ключем**, HashSet — коли треба лише знати **чи є елемент** і гарантувати унікальність. Приклад із System.Text.Json також показує, що для дуже малих наборів даних простий масив буває швидшим за хеш-таблицю.

## 2. Порівняльна таблиця

| Операція | List\<T\> | Dictionary<TKey, TValue> | HashSet\<T\> | Коли використовувати |
|---|---|---|---|---|
| Додавання | O(1)* в кінець, O(n) у середину | O(1)* | O(1)* | List — потрібен порядок і дублікати; Dictionary — дані «ключ → значення»; HashSet — тільки унікальні елементи |
| Пошук за значенням | O(n) | N/A (`ContainsValue` — O(n)) | O(1) | HashSet, якщо часто питаємо «чи є такий елемент?» |
| Пошук за ключем | N/A (за індексом — O(1)) | O(1) | N/A | Dictionary — коли елемент шукають за ідентифікатором (id, ім'я файлу) |
| Видалення | O(n) | O(1) | O(1) | Dictionary / HashSet — часті видалення довільних елементів |
| Перевірка наявності | O(n) (`Contains`) | O(1) (`ContainsKey`) | O(1) (`Contains`) | Dictionary / HashSet — великі колекції з частими перевірками |

\* амортизовано: зазвичай O(1), але при заповненні внутрішнього масиву колекція збільшується, і ця операція займає O(n). Для Dictionary і HashSet O(1) — це середній випадок; при поганій хеш-функції (багато колізій) операції деградують до O(n).

## 3. Висновки щодо вибору колекції

**Коли Dictionary краще за List:**
- коли елементи шукають за ключем (id, ім'я, код): Dictionary знаходить їх за O(1), а List перебирає всі елементи (O(n));
- коли колекція велика й пошуків багато. Для 10 елементів різниця непомітна, для 100 000 — величезна;
- List кращий, коли важливий порядок елементів, потрібен доступ за індексом або дозволені дублікати.

**Коли HashSet — оптимальний вибір:**
- потрібно зберігати лише унікальні значення (теги, відвідані вершини графа, імена, які вже зустрічались);
- часто треба перевіряти, чи є елемент у наборі;
- потрібні операції над множинами: об'єднання (`UnionWith`), перетин (`IntersectWith`), різниця (`ExceptWith`).

**Підводні камені:**
- **Порядок не гарантується.** Dictionary і HashSet не зберігають порядок додавання, тому для стабільного виводу результат треба сортувати.
- **Індексатор Dictionary викидає `KeyNotFoundException`,** якщо ключа немає. Безпечніше використовувати `TryGetValue`, до того ж це один пошук замість двох (`ContainsKey` + `[]`).
- **Власні класи як ключі.** За замовчуванням об'єкти порівнюються за посиланням, тому два «однакові» `Person` вважатимуться різними. Треба перевизначити `Equals` і `GetHashCode` або передати `IEqualityComparer<T>`.
- **Не можна змінювати ключ після додавання.** Якщо поле, з якого рахується хеш, змінилося, елемент «загубиться» в колекції.
- **Регістр рядків.** `"CSharp"` і `"csharp"` — різні ключі, якщо не передати `StringComparer.OrdinalIgnoreCase`.
- **Потокобезпечність.** Звичайні Dictionary і HashSet не можна одночасно змінювати з кількох потоків, для цього є `ConcurrentDictionary`.
- **Пам'ять.** Хеш-таблиці займають більше пам'яті, ніж List, через службові масиви.

## 4. Приклади з власного коду

У проєкті реалізовано три сценарії (код — у файлах `.cs`, повний вивід програми — у `README.md`).

**Сценарій 1. `FileMetadataCache`** — `Dictionary<string, FileMetadata>` з `StringComparer.OrdinalIgnoreCase`: ім'я файлу → метадані (розмір, дата зміни, хеш). Методи `Add`, `Get`, `Remove`, `Contains` працюють за O(1). Завдяки comparer `"REPORT.PDF"` і `"report.pdf"` вважаються одним файлом, як у Windows.

```
Contains("REPORT.PDF") (інший регістр): True
Remove("photo.jpg"): True
Get("missing.doc") -> KeyNotFoundException: Файл 'missing.doc' відсутній у кеші.
```

**Сценарій 2. `TagManager`** — `Dictionary<string, HashSet<string>>`: id статті → множина тегів. HashSet не дає додати один тег двічі (повторний `"CSharp"` проігноровано). Для пошуку використано операції множин:

```
Усі унікальні теги (UnionWith): { collections, csharp, dotnet, performance }
Спільні теги (IntersectWith): { collections, dotnet }
Є в article-1, немає в article-2 (ExceptWith): { csharp }
```

**Сценарій 3 (додатково). `SearchIndex`** — інвертований індекс `Dictionary<string, HashSet<int>>`: слово → номери документів. Пошук одного слова — O(1), пошук документів з кількома словами — перетин множин (`IntersectWith`).

```
Search("dictionary"): { 1, 3 }
SearchMultiple("dictionary", "hashset"): { 3 }
```