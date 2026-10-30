using System;
using System.Diagnostics;
using System.Globalization;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы «Координационные оси» (<c>kLiraTable_ConstructionAxes</c>).
  /// Имена свойств соответствуют заголовкам столбцов таблицы:
  /// Маркировка оси, X1 (м), Y1 (м), X2 (м), Y2 (м), Угол поворота (°).
  /// Первый столбец — строка, остальные — <see cref="double"/>.
  /// Замечание: в тестовой модели таблица была пуста, типы определены по заголовкам.
  /// </summary>
  [DebuggerDisplay("Label: {Label}, X1: {X1}, Y1: {Y1}, X2: {X2}, Y2: {Y2}, RotationAngle: {RotationAngle}")]
  public class kLiraTable_ConstructionAxes_TableRow : ILiraTableRow
  {
    /// <summary>Маркировка оси (например "1", "А", "1-1")</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>X1 — координата начала оси по X, м</summary>
    public double X1 { get; set; }

    /// <summary>Y1 — координата начала оси по Y, м</summary>
    public double Y1 { get; set; }

    /// <summary>X2 — координата конца оси по X, м</summary>
    public double X2 { get; set; }

    /// <summary>Y2 — координата конца оси по Y, м</summary>
    public double Y2 { get; set; }

    /// <summary>Угол поворота оси, °</summary>
    public double RotationAngle { get; set; }

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_ConstructionAxes_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_ConstructionAxes_TableRow(
        string label,
        double x1,
        double y1,
        double x2,
        double y2,
        double rotationAngle)
    {
      Label = label;
      X1 = x1;
      Y1 = y1;
      X2 = x2;
      Y2 = y2;
      RotationAngle = rotationAngle;
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — Label, 1 — X1, 2 — Y1, 3 — X2, 4 — Y2, 5 — RotationAngle.
    /// </summary>
    public static kLiraTable_ConstructionAxes_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_ConstructionAxes_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      Label = array[rowIndex, 0]?.ToString() ?? string.Empty;
      X1 = ToDouble(array[rowIndex, 1]);
      Y1 = ToDouble(array[rowIndex, 2]);
      X2 = ToDouble(array[rowIndex, 3]);
      Y2 = ToDouble(array[rowIndex, 4]);
      RotationAngle = ToDouble(array[rowIndex, 5]);
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
        => obj is kLiraTable_ConstructionAxes_TableRow o
           && Label == o.Label
           && X1.Equals(o.X1)
           && Y1.Equals(o.Y1)
           && X2.Equals(o.X2)
           && Y2.Equals(o.Y2)
           && RotationAngle.Equals(o.RotationAngle);

    public override int GetHashCode()
        => HashCode.Combine(
               HashCode.Combine(Label, X1, Y1),
               HashCode.Combine(X2, Y2, RotationAngle)
           );

    public override string ToString()
        => $"Маркировка оси: {Label}, X1: {X1}, Y1: {Y1}, X2: {X2}, Y2: {Y2}, Угол поворота: {RotationAngle}";
  }
}
