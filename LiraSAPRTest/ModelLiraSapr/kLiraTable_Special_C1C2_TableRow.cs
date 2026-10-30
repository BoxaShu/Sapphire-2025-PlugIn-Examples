using System;
using System.Diagnostics;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы параметров C1/C2 специальных элементов (<c>kLiraTable_Special_C1C2</c>).
  /// Имена свойств соответствуют заголовкам столбцов таблицы:
  /// КЭ, Тип КЭ, C1z (т/м3), C2z (т/м), Fi (rad).
  /// Замечание: в тестовой модели таблица была пуста, поэтому типы столбцов определены
  /// по аналогии с другими таблицами (<c>Elements_TypeAndNodes</c>) и по смыслу заголовков.
  /// </summary>
  [DebuggerDisplay("ElementNumber: {ElementNumber}, ElementType: {ElementType}, C1z: {C1z}, C2z: {C2z}, Fi: {Fi}")]
  public class kLiraTable_Special_C1C2_TableRow : ILiraTableRow
  {
    /// <summary>КЭ — номер конечного элемента</summary>
    public int ElementNumber { get; set; }

    /// <summary>Тип КЭ — тип специального конечного элемента</summary>
    public int ElementType { get; set; }

    /// <summary>C1z — коэффициент постели по Z, т/м3</summary>
    public double C1z { get; set; }

    /// <summary>C2z — коэффициент постели по Z (сдвиг), т/м</summary>
    public double C2z { get; set; }

    /// <summary>Fi — угол, rad</summary>
    public double Fi { get; set; }

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_Special_C1C2_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_Special_C1C2_TableRow(
        int elementNumber,
        int elementType,
        double c1z,
        double c2z,
        double fi)
    {
      ElementNumber = elementNumber;
      ElementType = elementType;
      C1z = c1z;
      C2z = c2z;
      Fi = fi;
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — ElementNumber (КЭ), 1 — ElementType (Тип КЭ),
    /// 2 — C1z, 3 — C2z, 4 — Fi.
    /// </summary>
    public static kLiraTable_Special_C1C2_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_Special_C1C2_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      ElementNumber = ToInt(array[rowIndex, 0]);
      ElementType = ToInt(array[rowIndex, 1]);
      C1z = ToDouble(array[rowIndex, 2]);
      C2z = ToDouble(array[rowIndex, 3]);
      Fi = ToDouble(array[rowIndex, 4]);
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
        => obj is kLiraTable_Special_C1C2_TableRow o
           && ElementNumber == o.ElementNumber
           && ElementType == o.ElementType
           && C1z.Equals(o.C1z)
           && C2z.Equals(o.C2z)
           && Fi.Equals(o.Fi);

    public override int GetHashCode()
        => HashCode.Combine(ElementNumber, ElementType, C1z, C2z, Fi);

    public override string ToString()
        => $"КЭ: {ElementNumber}, Тип КЭ: {ElementType}, C1z: {C1z}, C2z: {C2z}, Fi: {Fi}";
  }
}
