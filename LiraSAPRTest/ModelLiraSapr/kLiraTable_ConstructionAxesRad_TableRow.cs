using System;
using System.Diagnostics;
using System.Globalization;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы «Координационные оси дуговые» (<c>kLiraTable_ConstructionAxesRad</c>).
  /// Имена свойств соответствуют заголовкам столбцов таблицы:
  /// Маркировка оси, X, Y, R, Fi1, Fi2.
  /// Первый столбец — <see cref="string"/>, остальные — <see cref="double"/>.
  /// </summary>
  [DebuggerDisplay("Axis: {Label}, X: {X}, Y: {Y}, R: {R}, Fi1: {Fi1}, Fi2: {Fi2}")]
  public class kLiraTable_ConstructionAxesRad_TableRow : ILiraTableRow
  {
    /// <summary>Маркировка оси — имя или номер точки/оси (например «Ось 1», «Фундамент А»)</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>X, м — координата X точки приложения</summary>
    public double X { get; set; }

    /// <summary>Y, м — координата Y точки приложения</summary>
    public double Y { get; set; }

    /// <summary>R, м — радиус (расстояние до точки, радиус фундамента или плечо кручения)</summary>
    public double R { get; set; }

    /// <summary>Fi1, ° — угол (направление волны/поворота локальной системы координат)</summary>
    public double Fi1 { get; set; }

    /// <summary>Fi2, ° — второй угол (дополнительный угол наклона/между локальными осями)</summary>
    public double Fi2 { get; set; }

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_ConstructionAxesRad_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_ConstructionAxesRad_TableRow(
        string label, double x, double y, double r, double fi1, double fi2)
    {
      Label = label;
      X = x;
      Y = y;
      R = r;
      Fi1 = fi1;
      Fi2 = fi2;
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — Label, 1 — X, 2 — Y, 3 — R, 4 — Fi1, 5 — Fi2.
    /// </summary>
    public static kLiraTable_ConstructionAxesRad_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_ConstructionAxesRad_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      Label = ToText(array[rowIndex, 0]);
      X = ToDouble(array[rowIndex, 1]);
      Y = ToDouble(array[rowIndex, 2]);
      R = ToDouble(array[rowIndex, 3]);
      Fi1 = ToDouble(array[rowIndex, 4]);
      Fi2 = ToDouble(array[rowIndex, 5]);
    }

    private static string ToText(object value)
        => value == null || value is DBNull ? string.Empty : value.ToString() ?? string.Empty;

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
        => obj is kLiraTable_ConstructionAxesRad_TableRow o
           && Label == o.Label
           && X.Equals(o.X)
           && Y.Equals(o.Y)
           && R.Equals(o.R)
           && Fi1.Equals(o.Fi1)
           && Fi2.Equals(o.Fi2);

    public override int GetHashCode()
        => HashCode.Combine(Label, X, Y, R, Fi1, Fi2);

    public override string ToString()
        => $"Ось: {Label}, X: {X}, Y: {Y}, R: {R}, Fi1: {Fi1}, Fi2: {Fi2}";
  }
}