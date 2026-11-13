# Самостійна робота №14. Dictionary<TKey, TValue> та HashSet<T>

Консольний застосунок `CollectionsDemo` (.NET 9), який демонструє використання узагальнених колекцій `Dictionary<TKey, TValue>` і `HashSet<T>`.

Звіт з аналізом open-source прикладів, порівняльною таблицею і висновками: [REPORT.md](REPORT.md)

## Структура

| Файл | Опис |
|---|---|
| `CollectionsDemo/FileMetadata.cs` | Клас метаданих файлу |
| `CollectionsDemo/FileMetadataCache.cs` | **Сценарій 1.** Кеш метаданих на `Dictionary<string, FileMetadata>` (Add, Get, Remove, Contains) |
| `CollectionsDemo/TagManager.cs` | **Сценарій 2.** Теги статей на `Dictionary<string, HashSet<string>>` (UnionWith, IntersectWith, ExceptWith) |
| `CollectionsDemo/SearchIndex.cs` | **Сценарій 3 (додатковий).** Інвертований індекс на `Dictionary<string, HashSet<int>>` |
| `CollectionsDemo/Program.cs` | Демонстрація роботи всіх класів |

## Запуск

```bash
cd SR14
dotnet run --project CollectionsDemo
```

## Приклад виводу програми

```
===== Сценарій 1: FileMetadataCache (Dictionary) =====
Файлів у кеші: 3
Get("report.pdf"): report.pdf | 204800 байт | змінено 10.11.2026 14:30 | hash a1b2c3
Contains("REPORT.PDF") (інший регістр): True
Remove("photo.jpg"): True
Contains("photo.jpg") після видалення: False
Файлів у кеші: 2
Get("missing.doc") -> KeyNotFoundException: Файл 'missing.doc' відсутній у кеші.

===== Сценарій 2: TagManager (HashSet) =====
Теги article-1: { collections, csharp, dotnet }
Теги article-2: { collections, dotnet, performance }
Усі унікальні теги (UnionWith): { collections, csharp, dotnet, performance }
Спільні теги (IntersectWith): { collections, dotnet }
Є в article-1, немає в article-2 (ExceptWith): { csharp }
Після RemoveTag("article-1", "collections"): { csharp, dotnet }

===== Сценарій 3: SearchIndex (Dictionary + HashSet) =====
Search("dictionary"): { 1, 3 }
Search("stores"): { 1, 2 }
SearchMultiple("dictionary", "hashset"): { 3 }
Search("python"): {  }
```