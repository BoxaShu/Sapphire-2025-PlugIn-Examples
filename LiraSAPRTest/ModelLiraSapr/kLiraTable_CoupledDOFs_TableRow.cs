using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы «Объединение перемещений» (<c>kLiraTable_CoupledDOFs</c>) —
  /// объединение (жёсткая связь) степеней свободы группы узлов.
  /// Имена свойств соответствуют заголовкам столбцов таблицы:
  /// №, Номер блока, X, Y, Z, UX, UY, UZ, W, Номера узлов.
  /// Признаки степеней свободы (столбцы 2–8) — числовые (<see cref="double"/>).
  /// </summary>
  [DebuggerDisplay("Number: {Number}, BlockNumber: {BlockNumber}, X: {X}, Y: {Y}, Z: {Z}, UX: {UX}, UY: {UY}, UZ: {UZ}, W: {W}, NodeIds: {NodeIds}")]
  public class kLiraTable_CoupledDOFs_TableRow : ILiraTableRow
  {
    /// <summary>№ — номер записи</summary>
    public int Number { get; set; }

    /// <summary>Номер блока (группы объединения)</summary>
    public int BlockNumber { get; set; }

    /// <summary>Признак объединения по X</summary>
    public double X { get; set; }

    /// <summary>Признак объединения по Y</summary>
    public double Y { get; set; }

    /// <summary>Признак объединения по Z</summary>
    public double Z { get; set; }

    /// <summary>Признак объединения по повороту вокруг X (UX)</summary>
    public double UX { get; set; }

    /// <summary>Признак объединения по повороту вокруг Y (UY)</summary>
    public double UY { get; set; }

    /// <summary>Признак объединения по повороту вокруг Z (UZ)</summary>
    public double UZ { get; set; }

    /// <summary>Признак объединения по W</summary>
    public double W { get; set; }

    /// <summary>Номера объединяемых узлов</summary>
    public List<int> NodeIds { get; set; } = new List<int>();

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_CoupledDOFs_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_CoupledDOFs_TableRow(
        int number,
        int blockNumber,
        double x,
        double y,
        double z,
        double ux,
        double uy,
        double uz,
        double w,
        List<int> nodeIds)
    {
      Number = number;
      BlockNumber = blockNumber;
      X = x;
      Y = y;
      Z = z;
      UX = ux;
      UY = uy;
      UZ = uz;
      W = w;
      NodeIds = nodeIds ?? new List<int>();
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — Number, 1 — BlockNumber,
    /// 2 — X, 3 — Y, 4 — Z, 5 — UX, 6 — UY, 7 — UZ, 8 — W,
    /// 9 — NodeIds (строка вида "139414 158897").
    /// </summary>
    public static kLiraTable_CoupledDOFs_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_CoupledDOFs_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      Number = ToInt(array[rowIndex, 0]);
      BlockNumber = ToInt(array[rowIndex, 1]);
      X = ToDouble(array[rowIndex, 2]);
      Y = ToDouble(array[rowIndex, 3]);
      Z = ToDouble(array[rowIndex, 4]);
      UX = ToDouble(array[rowIndex, 5]);
      UY = ToDouble(array[rowIndex, 6]);
      UZ = ToDouble(array[rowIndex, 7]);
      W = ToDouble(array[rowIndex, 8]);
      NodeIds = ParseNodeIds(array[rowIndex, 9]?.ToString());
    }

    /// <summary>
    /// Разбирает строку вида "139414 158897" (разделители — пробелы, запятые,
    /// точка с запятой) в список целых чисел. Некорректные элементы пропускаются.
    /// </summary>
    public static List<int> ParseNodeIds(string? input)
    {
      var result = new List<int>();
      if (string.IsNullOrWhiteSpace(input))
        return result;

      var parts = input.Split(new[] { ' ', ',', ';', '\t', '\r', '\n' },
                              StringSplitOptions.RemoveEmptyEntries);
      foreach (var part in parts)
      {
        if (int.TryParse(part, out int value))
          result.Add(value);
      }

      return result;
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
        => obj is kLiraTable_CoupledDOFs_TableRow o
           && Number == o.Number
           && BlockNumber == o.BlockNumber
           && X.Equals(o.X)
           && Y.Equals(o.Y)
           && Z.Equals(o.Z)
           && UX.Equals(o.UX)
           && UY.Equals(o.UY)
           && UZ.Equals(o.UZ)
           && W.Equals(o.W)
           && NodeIds.SequenceEqual(o.NodeIds);

    public override int GetHashCode()
    {
      var hash = new HashCode();
      hash.Add(Number);
      hash.Add(BlockNumber);
      hash.Add(X);
      hash.Add(Y);
      hash.Add(Z);
      hash.Add(UX);
      hash.Add(UY);
      hash.Add(UZ);
      hash.Add(W);
      foreach (var nodeId in NodeIds)
        hash.Add(nodeId);
      return hash.ToHashCode();
    }

    public override string ToString()
        => $"№: {Number}, Блок: {BlockNumber}, X: {X}, Y: {Y}, Z: {Z}, " +
           $"UX: {UX}, UY: {UY}, UZ: {UZ}, W: {W}, Узлы: [{string.Join(", ", NodeIds)}]";
  }
}
