using System;
using System.Diagnostics;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы жёстких вставок пластин (<c>kLiraTable_Plates_Offsets</c>).
  /// Столбцы: КЭ, A (м).
  /// В исходных данных значение приходит как <see cref="float"/> (System.Single),
  /// здесь хранится как <see cref="double"/> — приведение без потери точности.
  /// </summary>
  [DebuggerDisplay("ElementNumber: {ElementNumber}, A: {A}")]
  public class kLiraTable_Plates_Offsets_TableRow : ILiraTableRow
  {
    /// <summary>КЭ — номер конечного элемента (пластины)</summary>
    public int ElementNumber { get; set; }

    /// <summary>A — величина жёсткой вставки, м</summary>
    public double A { get; set; }

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_Plates_Offsets_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_Plates_Offsets_TableRow(int elementNumber, double a)
    {
      ElementNumber = elementNumber;
      A = a;
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — ElementNumber (КЭ), 1 — A (м).
    /// </summary>
    public static kLiraTable_Plates_Offsets_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_Plates_Offsets_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      ElementNumber = ToInt(array[rowIndex, 0]);
      A = ToDouble(array[rowIndex, 1]);
    }

    private static int ToInt(object value)
    {
      if (value == null || value is DBNull) return 0;
      return value is int i ? i : (int.TryParse(value.ToString(), out int parsed) ? parsed : 0);
    }

    private static double ToDouble(object value)
    {
      if (value == null || value is DBNull) return 0d;
      switch (value)
      {
        case double d: return d;
        case float f: return f;
        case decimal m: return (double)m;
        default:
          return double.TryParse(value.ToString(), out double parsed) ? parsed : 0d;
      }
    }

    public override bool Equals(object? obj)
        => obj is kLiraTable_Plates_Offsets_TableRow o
           && ElementNumber == o.ElementNumber
           && A.Equals(o.A);

    public override int GetHashCode()
        => HashCode.Combine(ElementNumber, A);

    public override string ToString()
        => $"КЭ: {ElementNumber}, A: {A}";
  }
}
