using System;
using System.Diagnostics;

namespace LiraSAPRTest
{
  /// <summary>
  /// Строка таблицы координат узлов.
  /// </summary>
  [DebuggerDisplay("ID: {ID}, X: {X}, Y: {Y}, Z: {Z}")]
  public class kLiraTable_Nodes_Coordinates_TableRow : ILiraTableRow
  {
    /// <summary>Номер узла</summary>
    public int ID { get; set; }

    /// <summary>Координата X</summary>
    public double X { get; set; }

    /// <summary>Координата Y</summary>
    public double Y { get; set; }

    /// <summary>Координата Z</summary>
    public double Z { get; set; }

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_Nodes_Coordinates_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_Nodes_Coordinates_TableRow(int id, double x, double y, double z)
    {
      ID = id;
      X = x;
      Y = y;
      Z = z;
    }

    ///// <summary>
    ///// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    ///// Ожидаемый порядок столбцов: 0 — ID, 1 — X, 2 — Y, 3 — Z.
    ///// </summary>
    public static kLiraTable_Nodes_Coordinates_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_Nodes_Coordinates_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      ID = Convert.ToInt32(array[rowIndex, 0]);
      X = Convert.ToDouble(array[rowIndex, 1]);
      Y = Convert.ToDouble(array[rowIndex, 2]);
      Z = Convert.ToDouble(array[rowIndex, 3]);
    }



    public override bool Equals(object? obj)
        => obj is kLiraTable_Nodes_Coordinates_TableRow o
           && ID == o.ID
           && X.Equals(o.X)
           && Y.Equals(o.Y)
           && Z.Equals(o.Z);

    public override int GetHashCode()
        => HashCode.Combine(ID, X, Y, Z);

    public override string ToString()
        => $"ID: {ID}, X: {X}, Y: {Y}, Z: {Z}";
  }
}