using System;
using System.Diagnostics;

namespace LiraSAPRTest
{
  /// <summary>
  /// Строка таблицы жёсткостей конечных элементов.
  /// </summary>
  [DebuggerDisplay("Number: {Number}, StiffnessNumber: {StiffnessNumber}")]
  public class kLiraTable_ElementsStiffnesses_TableRow : ILiraTableRow
  {
    /// <summary>Номер строки</summary>
    public int Number { get; set; }

    /// <summary>Номер жёсткости</summary>
    public int StiffnessNumber { get; set; }

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_ElementsStiffnesses_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_ElementsStiffnesses_TableRow(int number, int stiffnessNumber)
    {
      Number = number;
      StiffnessNumber = stiffnessNumber;
    }

    public static kLiraTable_ElementsStiffnesses_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_ElementsStiffnesses_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    public void LoadFromArray(object[,] array, int rowIndex)
    {
      Number = int.TryParse(array[rowIndex, 0]?.ToString(), out int n) ? n : 0;
      StiffnessNumber = int.TryParse(array[rowIndex, 1]?.ToString(), out int s) ? s : 0;
    }

    public override bool Equals(object? obj)
        => obj is kLiraTable_ElementsStiffnesses_TableRow o
           && Number == o.Number
           && StiffnessNumber == o.StiffnessNumber;

    public override int GetHashCode()
        => HashCode.Combine(Number, StiffnessNumber);

    public override string ToString()
        => $"Number: {Number}, StiffnessNumber: {StiffnessNumber}";
  }
}