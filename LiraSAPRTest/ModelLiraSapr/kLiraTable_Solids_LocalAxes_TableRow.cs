using System;
using System.Diagnostics;
using System.Globalization;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы «Согласованные оси объёмных КЭ» (<c>kLiraTable_Solids_LocalAxes</c>).
  /// Имена свойств соответствуют заголовкам столбцов таблицы:
  /// Элемент, X2_X (м), X2_Y (м), X2_Z (м), Y2_X (м), Y2_Y (м), Y2_Z (м).
  /// Первый столбец — целый, остальные — <see cref="double"/>.
  /// Замечание: в тестовой модели таблица была пуста, типы определены по заголовкам.
  /// </summary>
  [DebuggerDisplay("ElementNumber: {ElementNumber}, X2X: {X2X}, X2Y: {X2Y}, X2Z: {X2Z}, Y2X: {Y2X}, Y2Y: {Y2Y}, Y2Z: {Y2Z}")]
  public class kLiraTable_Solids_LocalAxes_TableRow : ILiraTableRow
  {
    /// <summary>Элемент — номер объёмного конечного элемента</summary>
    public int ElementNumber { get; set; }

    /// <summary>X2_X — компонента X второй оси, м</summary>
    public double X2X { get; set; }

    /// <summary>X2_Y — компонента Y второй оси, м</summary>
    public double X2Y { get; set; }

    /// <summary>X2_Z — компонента Z второй оси, м</summary>
    public double X2Z { get; set; }

    /// <summary>Y2_X — компонента X третьей оси, м</summary>
    public double Y2X { get; set; }

    /// <summary>Y2_Y — компонента Y третьей оси, м</summary>
    public double Y2Y { get; set; }

    /// <summary>Y2_Z — компонента Z третьей оси, м</summary>
    public double Y2Z { get; set; }

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_Solids_LocalAxes_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_Solids_LocalAxes_TableRow(
        int elementNumber,
        double x2x,
        double x2y,
        double x2z,
        double y2x,
        double y2y,
        double y2z)
    {
      ElementNumber = elementNumber;
      X2X = x2x;
      X2Y = x2y;
      X2Z = x2z;
      Y2X = y2x;
      Y2Y = y2y;
      Y2Z = y2z;
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — ElementNumber (Элемент),
    /// 1 — X2X, 2 — X2Y, 3 — X2Z, 4 — Y2X, 5 — Y2Y, 6 — Y2Z.
    /// </summary>
    public static kLiraTable_Solids_LocalAxes_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_Solids_LocalAxes_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      ElementNumber = ToInt(array[rowIndex, 0]);
      X2X = ToDouble(array[rowIndex, 1]);
      X2Y = ToDouble(array[rowIndex, 2]);
      X2Z = ToDouble(array[rowIndex, 3]);
      Y2X = ToDouble(array[rowIndex, 4]);
      Y2Y = ToDouble(array[rowIndex, 5]);
      Y2Z = ToDouble(array[rowIndex, 6]);
    }

    private static int ToInt(object value)
    {
      if (value == null || value is DBNull) return 0;
      return value is int i ? i : (int.TryParse(value.ToString(), out int parsed) ? parsed : 0);
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
        => obj is kLiraTable_Solids_LocalAxes_TableRow o
           && ElementNumber == o.ElementNumber
           && X2X.Equals(o.X2X)
           && X2Y.Equals(o.X2Y)
           && X2Z.Equals(o.X2Z)
           && Y2X.Equals(o.Y2X)
           && Y2Y.Equals(o.Y2Y)
           && Y2Z.Equals(o.Y2Z);

    public override int GetHashCode()
        => HashCode.Combine(
               HashCode.Combine(ElementNumber, X2X, X2Y, X2Z),
               HashCode.Combine(Y2X, Y2Y, Y2Z)
           );

    public override string ToString()
        => $"Элемент: {ElementNumber}, X2_X: {X2X}, X2_Y: {X2Y}, X2_Z: {X2Z}, " +
           $"Y2_X: {Y2X}, Y2_Y: {Y2Y}, Y2_Z: {Y2Z}";
  }
}
