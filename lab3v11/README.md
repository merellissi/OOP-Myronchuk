# Лабораторна робота №3

**Тема:** Життєвий цикл об'єкта та керування ресурсами  
**Варіант:** 11

## Опис

Клас `CryptoStream` імітує криптографічний потік і реалізує `IDisposable` за патерном Dispose:

 поля `_algorithm`, `_isStreamOpen`, `_disposed`;
 метод `Encrypt(string data)`;
 `Dispose()`, `Dispose(bool disposing)` та деструктор `~CryptoStream()`.

У `Main` показано три сценарії: `using`, явний виклик `Dispose()` і деструктор через `GC.Collect()`.

## Результат

