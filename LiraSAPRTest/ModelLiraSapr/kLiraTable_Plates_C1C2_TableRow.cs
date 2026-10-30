using System;
using System.Diagnostics;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы параметров C1/C2 пластин (<c>kLiraTable_Plates_C1C2</c>) —
  /// коэффициенты упругого основания и связанные с ними параметры.
  /// Порядок столбцов: 0 — Number, 1 — C1, 2 — C2, 3 — C3,
  /// 4 — Mode, 5 — Value, 6 — Diagram.
  /// Точный смысл коэффициентов C3 / Mode / Value требует проверки по документации API.
  /// </summary>
  [DebuggerDisplay("Number: {Number}, C1: {C1}, C2: {C2}, C3: {C3}, Mode: {Mode}, Value: {Value}, Diagram: {Diagram}")]
  public class kLiraTable_Plates_C1C2_TableRow : ILiraTableRow
  {
    /// <summary>Номер пластины (элемента)</summary>
    public int Number { get; set; }

    /// <summary>Коэффициент C1z (обычно коэффициент постели при сжатии/растяжении) м/т3</summary>
    public double C1z { get; set; }

    /// <summary>Коэффициент C2z (обычно коэффициент постели при сдвиге) т/м</summary>
    public double C2z { get; set; }

    /// <summary>Коэффициент Pz т/м2</summary>
    public double Pz { get; set; }

    /// <summary>№ Gr</summary>
    public int Gr { get; set; }

    /// <summary>Rz т/м2</summary>
    public double Value { get; set; }

    /// <summary>Тип работы постели (например "0 - Линейная")</summary>
    public string Diagram { get; set; } = string.Empty;

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_Plates_C1C2_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_Plates_C1C2_TableRow(
        int number,
        double c1,
        double c2,
        double c3,
        int mode,
        double value,
        string diagram)
    {
      Number = number;
      C1z = c1;
      C2z = c2;
      Pz = c3;
      Gr = mode;
      Value = value;
      Diagram = diagram;
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — Number, 1 — C1, 2 — C2, 3 — C3,
    /// 4 — Mode, 5 — Value, 6 — Diagram.
    /// </summary>
    public static kLiraTable_Plates_C1C2_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_Plates_C1C2_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      Number = ToInt(array[rowIndex, 0]);
      C1z = ToDouble(array[rowIndex, 1]);
      C2z = ToDouble(array[rowIndex, 2]);
      Pz = ToDouble(array[rowIndex, 3]);
      Gr = ToInt(array[rowIndex, 4]);
      Value = ToDouble(array[rowIndex, 5]);
      Diagram = array[rowIndex, 6]?.ToString() ?? string.Empty;
    }

    private static int ToInt(object value)
    {
      if (value == null || value is DBNull) return 0;
      return value is int i ? i : (int.TryParse(value.ToString(), out int parsed) ? parsed : 0);
    }

    private static double ToDouble(object value)
    {
      if (value == null || value is DBNull) return 0d;
      return value is double d ? d : (double.TryParse(value.ToString(), out double parsed) ? parsed : 0d);
    }

    public override bool Equals(object? obj)
        => obj is kLiraTable_Plates_C1C2_TableRow o
           && Number == o.Number
           && C1z.Equals(o.C1z)
           && C2z.Equals(o.C2z)
           && Pz.Equals(o.Pz)
           && Gr == o.Gr
           && Value.Equals(o.Value)
           && Diagram == o.Diagram;

    public override int GetHashCode()
        => HashCode.Combine(
               HashCode.Combine(Number, C1z, C2z, Pz),
               HashCode.Combine(Gr, Value, Diagram)
           );

    public override string ToString()
        => $"Number: {Number}, C1: {C1z}, C2: {C2z}, C3: {Pz}, Mode: {Gr}, Value: {Value}, Diagram: {Diagram}";
  }
}
