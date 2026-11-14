# InvertedIndexDemo — інвертований індекс і вимірювання продуктивності

**Самостійна робота №16.** Виконала: Мирончук Ірина, група ІПЗ-3/1

Консольний застосунок (.NET 9), що реалізує інвертований індекс для повнотекстового пошуку (`Dictionary<string, HashSet<int>>`) і порівнює його швидкість з лінійним пошуком.

Опис реалізації, таблиця вимірювань і висновки: **[REPORT.md](REPORT.md)**

## Можливості

- `SearchSingleWord` — пошук документів з одним словом;
- `SearchMultipleWords` — пошук документів з усіма словами (AND, `HashSet.IntersectWith`);
- `SearchAnyWord` — пошук документів з будь-яким словом (OR, `HashSet.UnionWith`) з ранжуванням за кількістю збігів;
- вимірювання швидкості для 10, 100, 1000 і 10000 документів;
- інтерактивний пошук у консолі.

## Запуск

```bash
dotnet run --project InvertedIndexDemo -c Release
```

## Приклад виводу програми

```
===== Частина 1. Інвертований індекс =====
Loaded 5 documents
Index contains 31 unique words
Tokenize("Hello, world! C# is great.") -> [hello, world, c#, is, great]

--- SearchSingleWord("python") ---
[3] Python Overview
   Python is a high-level programming language with simple syntax.
[5] Comparing Languages
   C#, Java, and Python are among the most popular programming languages today.
Found 2 documents

--- SearchSingleWord("C#") ---
[1] Introduction to C#
   C# is a modern object-oriented programming language developed by Microsoft.
[4] C# Advanced Features
   C# supports LINQ, async/await, and many modern programming paradigms.
[5] Comparing Languages
   C#, Java, and Python are among the most popular programming languages today.
Found 3 documents

--- SearchMultipleWords("programming", "language")  — AND ---
[1] Introduction to C#
   C# is a modern object-oriented programming language developed by Microsoft.
[2] Java Basics
   Java is a popular programming language known for its portability and performance.
[3] Python Overview
   Python is a high-level programming language with simple syntax.
Found 3 documents

--- SearchMultipleWords("c#", "async")  — AND ---
[4] C# Advanced Features
   C# supports LINQ, async/await, and many modern programming paradigms.
Found 1 documents

--- SearchAnyWord("java", "python", "linq")  — OR ---
[5] Comparing Languages
   C#, Java, and Python are among the most popular programming languages today.
[2] Java Basics
   Java is a popular programming language known for its portability and performance.
[3] Python Overview
   Python is a high-level programming language with simple syntax.
[4] C# Advanced Features
   C# supports LINQ, async/await, and many modern programming paradigms.
Found 4 documents

--- SearchSingleWord("rust") ---
Found 0 documents

===== Частина 3. Продуктивність: лінійний пошук vs інвертований індекс =====
Шукане слово: "python". Час — середній на один пошук.

Документів |  Лінійний, мкс |  Індекс, мкс |  Побудова індексу, мкс | У скільки разів | Знайдено (лін/інд) | Окупність, запитів
-----------------------------------------------------------------------------------------------------------------------------
        10 |         116,21 |        4,598 |                  218,8 |             25x |                4/4 | 2
       100 |         985,20 |        7,091 |                 1364,6 |            139x |              40/40 | 2
      1000 |        5984,33 |      158,693 |                11808,2 |             38x |            400/400 | 3
     10000 |       55693,33 |     1399,890 |                83620,7 |             40x |          4000/4000 | 2

===== Частина 4. Інтерактивний пошук (AND) =====
Enter search query (or 'exit'): programming language

--- Search Results (AND) ---
[1] Introduction to C#
   C# is a modern object-oriented programming language developed by Microsoft.
[2] Java Basics
   Java is a popular programming language known for its portability and performance.
[3] Python Overview
   Python is a high-level programming language with simple syntax.
Found 3 documents
Enter search query (or 'exit'): c# async

--- Search Results (AND) ---
[4] C# Advanced Features
   C# supports LINQ, async/await, and many modern programming paradigms.
Found 1 documents
Enter search query (or 'exit'): exit
```