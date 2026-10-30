using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы «Элементы - ТЗА» (<c>kLiraTable_Elements_PRTypes</c>),
  /// где ТЗА — Тип Задания Арматуры.
  /// Имена свойств соответствуют заголовкам столбцов таблицы: КЭ, Список номеров ТЗА.
  /// Первый столбец — <see cref="int"/>. «Список номеров ТЗА» приходит строкой
  /// (например "3" или "3 5-7") и раскладывается в <see cref="List{Int32}"/>.
  /// </summary>
  [DebuggerDisplay("Element: {ElementNumber}, PRTypes: {PRTypeNumbers.Count}")]
  public class kLiraTable_Elements_PRTypes_TableRow : ILiraTableRow
  {
    /// <summary>КЭ (номер конечного элемента)</summary>
    public int ElementNumber { get; set; }

    /// <summary>
    /// Список номеров ТЗА (Тип Задания Арматуры).
    /// Диапазон "A-B" раскрывается во все номера от A до B включительно.
    /// </summary>
    public List<int> PRTypeNumbers { get; set; } = new List<int>();

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_Elements_PRTypes_TableRow() { }

    /// <summary>
    /// Полный конструктор. Список номеров ТЗА (Тип Задания Арматуры) задаётся строкой
    /// (например "3" или "3 5-7") и разбирается в <see cref="PRTypeNumbers"/>.
    /// </summary>
    public kLiraTable_Elements_PRTypes_TableRow(int elementNumber, string prTypeNumbers)
    {
      ElementNumber = elementNumber;
      PRTypeNumbers = ParseNumbers(prTypeNumbers);
    }

    /// <summary>
    /// Полный конструктор с уже готовым списком номеров ТЗА.
    /// </summary>
    public kLiraTable_Elements_PRTypes_TableRow(int elementNumber, List<int> prTypeNumbers)
    {
      ElementNumber = elementNumber;
      PRTypeNumbers = prTypeNumbers ?? new List<int>();
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — ElementNumber, 1 — PRTypeNumbers.
    /// </summary>
    public static kLiraTable_Elements_PRTypes_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_Elements_PRTypes_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      ElementNumber = ToInt(array[rowIndex, 0]);
      PRTypeNumbers = ParseNumbers(ToText(array[rowIndex, 1]));
    }

    /// <summary>
    /// Разбирает строку с номерами в список. Каждый фрагмент "A-B" даёт все номера
    /// от A до B включительно; одиночное число добавляется как есть.
    /// Некорректные фрагменты пропускаются.
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
        => obj is kLiraTable_Elements_PRTypes_TableRow o
           && ElementNumber == o.ElementNumber
           && PRTypeNumbers.SequenceEqual(o.PRTypeNumbers);

    public override int GetHashCode()
    {
      unchecked
      {
        int hash = ElementNumber;
        foreach (var number in PRTypeNumbers) hash = hash * 31 + number;
        return hash;
      }
    }

    public override string ToString()
        => $"КЭ: {ElementNumber}, ТЗА: {string.Join(", ", PRTypeNumbers)}";
  }
}