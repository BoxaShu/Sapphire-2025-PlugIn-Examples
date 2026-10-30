using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы «Избирательный учёт масс» (<c>kLiraTable_LoadElement</c>).
  /// Имена свойств соответствуют заголовкам столбцов таблицы: №, Список номеров КЭ.
  /// «Список номеров КЭ» приходит строкой и поддерживает перечисления через запятую (1, 5, 12),
  /// диапазоны через дефис (10-25) и комбинации (1, 3, 10-15, 40); разбирается в <see cref="List{Int32}"/>.
  /// Первый столбец (№) — <see cref="int"/>.
  /// </summary>
  [DebuggerDisplay("No: {Number}, Elements: {ElementNumbers.Count}")]
  public class kLiraTable_LoadElement_TableRow : ILiraTableRow
  {
    /// <summary>№ — порядковый номер строки в списке</summary>
    public int Number { get; set; }

    /// <summary>
    /// Список номеров КЭ. Диапазоны "A-B" и перечисления через запятую/пробел
    /// раскрываются во все номера (например "1, 3, 10-15, 40").
    /// </summary>
    public List<int> ElementNumbers { get; set; } = new List<int>();

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_LoadElement_TableRow() { }

    /// <summary>
    /// Полный конструктор. Список номеров КЭ задаётся строкой
    /// (например "1, 3, 10-15, 40") и разбирается в <see cref="ElementNumbers"/>.
    /// </summary>
    public kLiraTable_LoadElement_TableRow(int number, string elementNumbers)
    {
      Number = number;
      ElementNumbers = ParseNumbers(elementNumbers);
    }

    /// <summary>
    /// Полный конструктор с уже готовым списком номеров КЭ.
    /// </summary>
    public kLiraTable_LoadElement_TableRow(int number, List<int> elementNumbers)
    {
      Number = number;
      ElementNumbers = elementNumbers ?? new List<int>();
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — Number, 1 — ElementNumbers.
    /// </summary>
    public static kLiraTable_LoadElement_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_LoadElement_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      Number = ToInt(array[rowIndex, 0]);
      ElementNumbers = ParseNumbers(ToText(array[rowIndex, 1]));
    }

    /// <summary>
    /// Разбирает строку с номерами КЭ в список. Каждый фрагмент "A-B" даёт все номера
    /// от A до B включительно; фрагменты разделяются запятой, пробелом, точкой с запятой
    /// или табуляцией. Одиночное число добавляется как есть. Некорректные фрагменты пропускаются.
    /// </summary>
    public static List<int> ParseNumbers(string? value)
    {
      var result = new List<int>();
      if (string.IsNullOrWhiteSpace(value)) return result;

      foreach (var token in value.Split(
                   new[] { ' ', ',', ';', '\t', '\r', '\n' },
                   StringSplitOptions.RemoveEmptyEntries))
      {
        var dash = token.IndexOf('-');
        if (dash > 0)
        {
          if (int.TryParse(token.Substring(0, dash), NumberStyles.Integer, CultureInfo.InvariantCulture, out int from)
              && int.TryParse(token.Substring(dash + 1), NumberStyles.Integer, CultureInfo.InvariantCulture, out int to))
          {
            if (to < from) { int t = from; from = to; to = t; }
            for (int i = from; i <= to; i++) result.Add(i);
          }
        }
        else if (int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out int single))
        {
          result.Add(single);
        }
      }

      return result;
    }

    private static string ToText(object value)
        => value == null || value is DBNull ? string.Empty : value.ToString() ?? string.Empty;

    /// <summary>
    /// Приводит значение столбца к <see cref="int"/>.
    /// Пустые строки/null/DBNull дают 0.
    /// </summary>
    private static int ToInt(object value)
    {
      if (value == null || value is DBNull) return 0;
      switch (value)
      {
        case int n: return n;
        case long l: return (int)l;
        case double d: return (int)d;
        case float f: return (int)f;
        default:
          var text = value.ToString();
          if (string.IsNullOrWhiteSpace(text)) return 0;
          return int.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out int current)
              ? current
              : (int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int invariant) ? invariant : 0);
      }
    }

    public override bool Equals(object? obj)
        => obj is kLiraTable_LoadElement_TableRow o
           && Number == o.Number
           && ElementNumbers.SequenceEqual(o.ElementNumbers);

    public override int GetHashCode()
    {
      unchecked
      {
        int hash = Number;
        foreach (var number in ElementNumbers) hash = hash * 31 + number;
        return hash;
      }
    }

    public override string ToString()
        => $"№: {Number}, КЭ: {string.Join(", ", ElementNumbers)}";
  }
}