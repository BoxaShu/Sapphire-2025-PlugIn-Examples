using System;
using System.Diagnostics;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы параметров C1/C2 стержней (<c>kLiraTable_Bars_C1C2</c>) —
  /// параметры упругого основания / специальных связей стержня.
  /// Имена свойств соответствуют заголовкам столбцов таблицы:
  /// КЭ, C1z (т/м3), C2z (т/м), Bc=B, Bc (м), Cy1 (т/м3), Cy2 (т/м),
  /// Hc (м), Pz (т/м2), Gr, Rz (т/м2), Ry (т/м2), Тип работы постели.
  /// </summary>
  [DebuggerDisplay("ElementNumber: {ElementNumber}, C1z: {C1z}, C2z: {C2z}, Bc: {Bc}, Cy1: {Cy1}, Cy2: {Cy2}, Hc: {Hc}, Pz: {Pz}, FoundationType: {FoundationType}")]
  public class kLiraTable_Bars_C1C2_TableRow : ILiraTableRow
  {
    /// <summary>КЭ — номер конечного элемента (стержня)</summary>
    public int ElementNumber { get; set; }

    /// <summary>C1z — коэффициент постели по Z, т/м3</summary>
    public double C1z { get; set; }

    /// <summary>C2z — коэффициент постели по Z (сдвиг), т/м</summary>
    public double C2z { get; set; }

    /// <summary>Bc=B — признак «ширина Bc равна B» (текстовый флаг)</summary>
    public string BcEqualsB { get; set; } = string.Empty;

    /// <summary>Bc — ширина основания, м</summary>
    public double Bc { get; set; }

    /// <summary>Cy1 — коэффициент постели по Y, т/м3</summary>
    public double Cy1 { get; set; }

    /// <summary>Cy2 — коэффициент постели по Y (сдвиг), т/м</summary>
    public double Cy2 { get; set; }

    /// <summary>Hc — высота/толщина основания, м</summary>
    public double Hc { get; set; }

    /// <summary>Pz — давление/нагрузка по Z, т/м2</summary>
    public double Pz { get; set; }

    /// <summary>Gr — целочисленный параметр из таблицы</summary>
    public int Gr { get; set; }

    /// <summary>Rz — сопротивление по Z, т/м2</summary>
    public double Rz { get; set; }

    /// <summary>Ry — сопротивление по Y, т/м2</summary>
    public double Ry { get; set; }

    /// <summary>Тип работы постели (например "0 - Линейная")</summary>
    public string FoundationType { get; set; } = string.Empty;

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_Bars_C1C2_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_Bars_C1C2_TableRow(
        int elementNumber,
        double c1z,
        double c2z,
        string bcEqualsB,
        double bc,
        double cy1,
        double cy2,
        double hc,
        double pz,
        int gr,
        double rz,
        double ry,
        string foundationType)
    {
      ElementNumber = elementNumber;
      C1z = c1z;
      C2z = c2z;
      BcEqualsB = bcEqualsB;
      Bc = bc;
      Cy1 = cy1;
      Cy2 = cy2;
      Hc = hc;
      Pz = pz;
      Gr = gr;
      Rz = rz;
      Ry = ry;
      FoundationType = foundationType;
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — ElementNumber (КЭ), 1 — C1z, 2 — C2z,
    /// 3 — BcEqualsB, 4 — Bc, 5 — Cy1, 6 — Cy2, 7 — Hc, 8 — Pz,
    /// 9 — Gr, 10 — Rz, 11 — Ry, 12 — FoundationType.
    /// </summary>
    public static kLiraTable_Bars_C1C2_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_Bars_C1C2_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      ElementNumber = ToInt(array[rowIndex, 0]);
      C1z = ToDouble(array[rowIndex, 1]);
      C2z = ToDouble(array[rowIndex, 2]);
      BcEqualsB = array[rowIndex, 3]?.ToString() ?? string.Empty;
      Bc = ToDouble(array[rowIndex, 4]);
      Cy1 = ToDouble(array[rowIndex, 5]);
      Cy2 = ToDouble(array[rowIndex, 6]);
      Hc = ToDouble(array[rowIndex, 7]);
      Pz = ToDouble(array[rowIndex, 8]);
      Gr = ToInt(array[rowIndex, 9]);
      Rz = ToDouble(array[rowIndex, 10]);
      Ry = ToDouble(array[rowIndex, 11]);
      FoundationType = array[rowIndex, 12]?.ToString() ?? string.Empty;
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
        => obj is kLiraTable_Bars_C1C2_TableRow o
           && ElementNumber == o.ElementNumber
           && C1z.Equals(o.C1z)
           && C2z.Equals(o.C2z)
           && BcEqualsB == o.BcEqualsB
           && Bc.Equals(o.Bc)
           && Cy1.Equals(o.Cy1)
           && Cy2.Equals(o.Cy2)
           && Hc.Equals(o.Hc)
           && Pz.Equals(o.Pz)
           && Gr == o.Gr
           && Rz.Equals(o.Rz)
           && Ry.Equals(o.Ry)
           && FoundationType == o.FoundationType;

    public override int GetHashCode()
        => HashCode.Combine(
               HashCode.Combine(ElementNumber, C1z, C2z, BcEqualsB),
               HashCode.Combine(Bc, Cy1, Cy2, Hc),
               HashCode.Combine(Pz, Gr, Rz, Ry),
               FoundationType
           );

    public override string ToString()
        => $"КЭ: {ElementNumber}, C1z: {C1z}, C2z: {C2z}, Bc=B: {BcEqualsB}, Bc: {Bc}, " +
           $"Cy1: {Cy1}, Cy2: {Cy2}, Hc: {Hc}, Pz: {Pz}, Gr: {Gr}, " +
           $"Rz: {Rz}, Ry: {Ry}, Тип работы постели: {FoundationType}";
  }
}
