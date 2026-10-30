using System;
using System.Diagnostics;
using System.Globalization;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы «Высотные отметки» (<c>kLiraTable_ElevationMarks</c>).
  /// Имена свойств соответствуют заголовкам столбцов таблицы:
  /// Высотная отметка (м), Слева/Справа.
  /// Первый столбец — <see cref="double"/> (в данных приходит как System.Single), второй — строка.
  /// </summary>
  [DebuggerDisplay("Elevation: {Elevation}, Side: {Side}")]
  public class kLiraTable_ElevationMarks_TableRow : ILiraTableRow
  {
    /// <summary>Высотная отметка, м</summary>
    public double Elevation { get; set; }

    /// <summary>Слева/Справа (например "Left" / "Right")</summary>
    public string Side { get; set; } = string.Empty;

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_ElevationMarks_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_ElevationMarks_TableRow(double elevation, string side)
    {
      Elevation = elevation;
      Side = side;
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — Elevation (Высотная отметка), 1 — Side (Слева/Справа).
    /// </summary>
    public static kLiraTable_ElevationMarks_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_ElevationMarks_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      Elevation = ToDouble(array[rowIndex, 0]);
      Side = array[rowIndex, 1]?.ToString() ?? string.Empty;
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
        => obj is kLiraTable_ElevationMarks_TableRow o
           && Elevation.Equals(o.Elevation)
           && Side == o.Side;

    public override int GetHashCode()
        => HashCode.Combine(Elevation, Side);

    public override string ToString()
        => $"Высотная отметка: {Elevation} м, Слева/Справа: {Side}";
  }
}
