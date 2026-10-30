using System;
using System.Diagnostics;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы закреплений узлов (<c>kLiraTable_Nodes_Restraints</c>).
  /// Порядок столбцов: 0 — ID, 1 — X, 2 — Y, 3 — Z,
  /// 4 — UX, 5 — UY, 6 — UZ, 7 — DOF7.
  /// Значения связей в API Lira-SAPR приходят строками (например "0", "1", "2", "К 1000"),
  /// поэтому все они хранятся как <see cref="string"/>.
  /// </summary>
  [DebuggerDisplay("ID: {ID}, X: {X}, Y: {Y}, Z: {Z}, UX: {UX}, UY: {UY}, UZ: {UZ}, DOF7: {DOF7}")]
  public class kLiraTable_Nodes_Restraints_TableRow : ILiraTableRow
  {
    /// <summary>Номер узла</summary>
    public int ID { get; set; }

    /// <summary>Связь по направлению X</summary>
    public string X { get; set; } = string.Empty;

    /// <summary>Связь по направлению Y</summary>
    public string Y { get; set; } = string.Empty;

    /// <summary>Связь по направлению Z</summary>
    public string Z { get; set; } = string.Empty;

    /// <summary>Связь по повороту вокруг X (UX)</summary>
    public string UX { get; set; } = string.Empty;

    /// <summary>Связь по повороту вокруг Y (UY)</summary>
    public string UY { get; set; } = string.Empty;

    /// <summary>Связь по повороту вокруг Z (UZ)</summary>
    public string UZ { get; set; } = string.Empty;

    /// <summary>Связь по дополнительной (7-й) степени свободы</summary>
    public string W { get; set; } = string.Empty;

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_Nodes_Restraints_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_Nodes_Restraints_TableRow(
        int id,
        string x,
        string y,
        string z,
        string ux,
        string uy,
        string uz,
        string w)
    {
      ID = id;
      X = x;
      Y = y;
      Z = z;
      UX = ux;
      UY = uy;
      UZ = uz;
      W = w;
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — ID, 1 — X, 2 — Y, 3 — Z,
    /// 4 — UX, 5 — UY, 6 — UZ, 7 — DOF7.
    /// </summary>
    public static kLiraTable_Nodes_Restraints_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_Nodes_Restraints_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      ID = int.TryParse(array[rowIndex, 0]?.ToString(), out int id) ? id : 0;
      X = array[rowIndex, 1]?.ToString() ?? string.Empty;
      Y = array[rowIndex, 2]?.ToString() ?? string.Empty;
      Z = array[rowIndex, 3]?.ToString() ?? string.Empty;
      UX = array[rowIndex, 4]?.ToString() ?? string.Empty;
      UY = array[rowIndex, 5]?.ToString() ?? string.Empty;
      UZ = array[rowIndex, 6]?.ToString() ?? string.Empty;
      W = array[rowIndex, 7]?.ToString() ?? string.Empty;
    }

    public override bool Equals(object? obj)
        => obj is kLiraTable_Nodes_Restraints_TableRow o
           && ID == o.ID
           && X == o.X
           && Y == o.Y
           && Z == o.Z
           && UX == o.UX
           && UY == o.UY
           && UZ == o.UZ
           && W == o.W;

    public override int GetHashCode()
        => HashCode.Combine(
               HashCode.Combine(ID, X, Y, Z),
               HashCode.Combine(UX, UY, UZ, W)
           );

    public override string ToString()
        => $"ID: {ID}, X: {X}, Y: {Y}, Z: {Z}, UX: {UX}, UY: {UY}, UZ: {UZ}, DOF7: {W}";
  }
}
