using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы «Конструктивные блоки» (<c>kLiraTable_StructuralBlocks</c>).
  /// Имена свойств соответствуют заголовкам столбцов таблицы:
  /// Список номеров КЭ, Вид КоБ, ID, Этаж, Марка, Комментарий, Цвет.
  /// «Список номеров КЭ» приходит строкой вида "28-724 798381-799136": каждая пара "A-B"
  /// означает все номера от A до B включительно, фрагменты разделены пробелом.
  /// Строка раскладывается в <see cref="List{Int32}"/> со всеми номерами.
  /// Третий столбец (ID) — <see cref="int"/>, «Этаж» — <see cref="double"/> (например -3.450),
  /// остальные — строки. Цвет — формат RGB: символ '@' и три байта в hex, "@rrggbb" (например "@e0e0b4").
  /// </summary>
  [DebuggerDisplay("ID: {Id}, Kind: {Kind}, Floor: {Floor}, Elements: {ElementNumbers.Count}, Comment: {Comment}")]
  public class kLiraTable_StructuralBlocks_TableRow : ILiraTableRow
  {
    /// <summary>
    /// Список номеров КЭ. Диапазон "A-B" раскрывается во все номера от A до B включительно,
    /// например "28-724 798381-799136" даёт 697 + 756 номеров.
    /// </summary>
    public List<int> ElementNumbers { get; set; } = new List<int>();

    /// <summary>Вид КоБ (например "СТЕНА", "ПЛИТА")</summary>
    public string Kind { get; set; } = string.Empty;

    /// <summary>ID конструктивного блока</summary>
    public int Id { get; set; }

    /// <summary>Этаж (отметка этажа, м)</summary>
    public double Floor { get; set; }

    /// <summary>Марка</summary>
    public string Mark { get; set; } = string.Empty;

    /// <summary>Комментарий (например "Стена", "Фунд. плита")</summary>
    public string Comment { get; set; } = string.Empty;

    /// <summary>
    /// Цвет в формате RGB: символ '@' и три байта в hex — "@rrggbb" (например "@e0e0b4" = R 0xE0, G 0xE0, B 0xB4).
    /// </summary>
    public string Color { get; set; } = string.Empty;

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_StructuralBlocks_TableRow() { }

    /// <summary>
    /// Полный конструктор. Список номеров КЭ задаётся строкой в формате Lira
    /// ("28-724 798381-799136") и разбирается в <see cref="ElementNumbers"/>.
    /// </summary>
    public kLiraTable_StructuralBlocks_TableRow(
        string elementNumbers, string kind, int id, double floor,
        string mark, string comment, string color)
    {
      ElementNumbers = ParseElementNumbers(elementNumbers);
      Kind = kind;
      Id = id;
      Floor = floor;
      Mark = mark;
      Comment = comment;
      Color = color;
    }

    /// <summary>
    /// Полный конструктор с уже готовым списком номеров КЭ.
    /// </summary>
    public kLiraTable_StructuralBlocks_TableRow(
        List<int> elementNumbers, string kind, int id, double floor,
        string mark, string comment, string color)
    {
      ElementNumbers = elementNumbers ?? new List<int>();
      Kind = kind;
      Id = id;
      Floor = floor;
      Mark = mark;
      Comment = comment;
      Color = color;
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — ElementNumbers, 1 — Kind, 2 — Id, 3 — Floor,
    /// 4 — Mark, 5 — Comment, 6 — Color.
    /// </summary>
    public static kLiraTable_StructuralBlocks_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_StructuralBlocks_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      ElementNumbers = ParseElementNumbers(ToText(array[rowIndex, 0]));
      Kind = ToText(array[rowIndex, 1]);
      Id = ToInt(array[rowIndex, 2]);
      Floor = ToDouble(array[rowIndex, 3]);
      Mark = ToText(array[rowIndex, 4]);
      Comment = ToText(array[rowIndex, 5]);
      Color = ToText(array[rowIndex, 6]);
    }

    /// <summary>
    /// Разбирает строку с номерами КЭ в список. Каждый фрагмент "A-B" даёт все номера
    /// от A до B включительно ("28-724 798381-799136" → 28..724 и 798381..799136);
    /// одиночное число добавляется как есть. Некорректные фрагменты пропускаются.
    /// </summary>
    public static List<int> ParseElementNumbers(string? value)
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

    /// <summary>
    /// Разбирает <see cref="Color"/> (формат "@rrggbb") в компоненты RGB.
    /// Возвращает false, если строка не соответствует формату.
    /// </summary>
    public bool TryGetRgb(out byte r, out byte g, out byte b)
    {
      r = g = b = 0;
      if (string.IsNullOrWhiteSpace(Color)) return false;

      var hex = Color.Trim();
      if (hex.StartsWith("@")) hex = hex.Substring(1);
      if (hex.Length != 6) return false;

      if (!byte.TryParse(hex.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out r)) return false;
      if (!byte.TryParse(hex.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out g)) return false;
      if (!byte.TryParse(hex.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out b)) return false;
      return true;
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

    /// <summary>
    /// Приводит значение столбца к <see cref="double"/>.
    /// Пустые строки/null/DBNull дают 0; принимаются как текущая, так и инвариантная культура.
    /// </summary>
    private static double ToDouble(object value)
    {
      if (value == null || value is DBNull) return 0d;
      switch (value)
      {
        case double d: return d;
        case float f: return f;
        case decimal m: return (double)m;
        case int n: return n;
        default:
          var text = value.ToString();
          if (string.IsNullOrWhiteSpace(text)) return 0d;
          return double.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out double current)
              ? current
              : (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out double invariant) ? invariant : 0d);
      }
    }

    public override bool Equals(object? obj)
        => obj is kLiraTable_StructuralBlocks_TableRow o
           && Kind == o.Kind
           && Id == o.Id
           && Floor.Equals(o.Floor)
           && Mark == o.Mark
           && Comment == o.Comment
           && Color == o.Color
           && ElementNumbers.SequenceEqual(o.ElementNumbers);

    public override int GetHashCode()
    {
      unchecked
      {
        int hash = HashCode.Combine(Kind, Id, Floor, Mark, Comment, Color);
        foreach (var number in ElementNumbers) hash = hash * 31 + number;
        return hash;
      }
    }

    public override string ToString()
        => $"ID: {Id}, Вид КоБ: {Kind}, Этаж: {Floor}, КЭ: {ElementNumbers.Count} шт., Комментарий: {Comment}";
  }
}