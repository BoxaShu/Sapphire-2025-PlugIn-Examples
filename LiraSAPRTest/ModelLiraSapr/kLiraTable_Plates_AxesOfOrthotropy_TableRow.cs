using System;
using System.Diagnostics;
using System.Globalization;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы «Оси ортотропии для пластин» (<c>kLiraTable_Plates_AxesOfOrthotropy</c>).
  /// Имена свойств соответствуют заголовкам столбцов таблицы: Элемент, Угол орто. (°).
  /// Первый столбец — целый, второй — <see cref="double"/> (в данных приходит как System.Single).
  /// </summary>
  [DebuggerDisplay("ElementNumber: {ElementNumber}, OrthotropyAngle: {OrthotropyAngle}")]
  public class kLiraTable_Plates_AxesOfOrthotropy_TableRow : ILiraTableRow
  {
    /// <summary>Элемент — номер конечного элемента (пластины)</summary>
    public int ElementNumber { get; set; }

    /// <summary>Угол ортотропии, °</summary>
    public double OrthotropyAngle { get; set; }

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_Plates_AxesOfOrthotropy_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_Plates_AxesOfOrthotropy_TableRow(int elementNumber, double orthotropyAngle)
    {
      ElementNumber = elementNumber;
      OrthotropyAngle = orthotropyAngle;
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — ElementNumber (Элемент), 1 — OrthotropyAngle (Угол орто.).
    /// </summary>
    public static kLiraTable_Plates_AxesOfOrthotropy_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_Plates_AxesOfOrthotropy_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      ElementNumber = ToInt(array[rowIndex, 0]);
      OrthotropyAngle = ToDouble(array[rowIndex, 1]);
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
        => obj is kLiraTable_Plates_AxesOfOrthotropy_TableRow o
           && ElementNumber == o.ElementNumber
           && OrthotropyAngle.Equals(o.OrthotropyAngle);

    public override int GetHashCode()
        => HashCode.Combine(ElementNumber, OrthotropyAngle);

    public override string ToString()
        => $"Элемент: {ElementNumber}, Угол орто.: {OrthotropyAngle}";
  }
}
