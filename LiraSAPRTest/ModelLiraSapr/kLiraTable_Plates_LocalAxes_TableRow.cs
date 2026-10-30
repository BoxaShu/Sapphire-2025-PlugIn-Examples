using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы согласованных осей пластин
  /// </summary>
  [DebuggerDisplay("ID: {ID}, Fi: {Fi}")]
  internal class kLiraTable_Plates_LocalAxes_TableRow : ILiraTableRow
  {
    /// <summary>Номер элемента</summary>
    public int ID { get; set; }

    /// <summary>Угол согласования в градусах</summary>
    public double Fi { get; set; }



    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_Plates_LocalAxes_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_Plates_LocalAxes_TableRow(int id, double fi)
    {
      ID = id;
      Fi = fi;
    }

    ///// <summary>
    ///// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    ///// Ожидаемый порядок столбцов: 0 — ID, 1 — X, 2 — Y, 3 — Z.
    ///// </summary>
    public static kLiraTable_Plates_LocalAxes_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_Plates_LocalAxes_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      ID = Convert.ToInt32(array[rowIndex, 0]);
      Fi = Convert.ToDouble(array[rowIndex, 1]);

    }



    public override bool Equals(object? obj)
        => obj is kLiraTable_Plates_LocalAxes_TableRow o
           && ID == o.ID
           && Fi.Equals(o.Fi);

    public override int GetHashCode()
        => HashCode.Combine(ID, Fi);

    public override string ToString()
        => $"ID: {ID}, Fi: {Fi}";
  }
}
