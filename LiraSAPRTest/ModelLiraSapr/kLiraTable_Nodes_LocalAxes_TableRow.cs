using System;
using System.Diagnostics;
using System.Globalization;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы «Местные оси узлов» (<c>kLiraTable_Nodes_LocalAxes</c>).
  /// Имена свойств соответствуют заголовкам столбцов таблицы: Узел, X (м), Y (м), Z (м), Fi (°).
  /// Первый столбец — целый, остальные — <see cref="double"/>.
  /// </summary>
  [DebuggerDisplay("NodeNumber: {NodeNumber}, X: {X}, Y: {Y}, Z: {Z}, Fi: {Fi}")]
  public class kLiraTable_Nodes_LocalAxes_TableRow : ILiraTableRow
  {
    /// <summary>Узел — номер узла</summary>
    public int NodeNumber { get; set; }

    /// <summary>X — координата/компонента местной оси по X, м</summary>
    public double X { get; set; }

    /// <summary>Y — координата/компонента местной оси по Y, м</summary>
    public double Y { get; set; }

    /// <summary>Z — координата/компонента местной оси по Z, м</summary>
    public double Z { get; set; }

    /// <summary>Fi — угол поворота местных осей, °</summary>
    public double Fi { get; set; }

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_Nodes_LocalAxes_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_Nodes_LocalAxes_TableRow(int nodeNumber, double x, double y, double z, double fi)
    {
      NodeNumber = nodeNumber;
      X = x;
      Y = y;
      Z = z;
      Fi = fi;
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — NodeNumber (Узел), 1 — X, 2 — Y, 3 — Z, 4 — Fi.
    /// </summary>
    public static kLiraTable_Nodes_LocalAxes_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_Nodes_LocalAxes_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      NodeNumber = ToInt(array[rowIndex, 0]);
      X = ToDouble(array[rowIndex, 1]);
      Y = ToDouble(array[rowIndex, 2]);
      Z = ToDouble(array[rowIndex, 3]);
      Fi = ToDouble(array[rowIndex, 4]);
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
        => obj is kLiraTable_Nodes_LocalAxes_TableRow o
           && NodeNumber == o.NodeNumber
           && X.Equals(o.X)
           && Y.Equals(o.Y)
           && Z.Equals(o.Z)
           && Fi.Equals(o.Fi);

    public override int GetHashCode()
        => HashCode.Combine(NodeNumber, X, Y, Z, Fi);

    public override string ToString()
        => $"Узел: {NodeNumber}, X: {X}, Y: {Y}, Z: {Z}, Fi: {Fi}";
  }
}
