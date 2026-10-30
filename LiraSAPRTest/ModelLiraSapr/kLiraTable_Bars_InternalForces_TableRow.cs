using System;
using System.Diagnostics;
using System.Globalization;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы «Усилия в стержнях» (<c>kLiraTable_Bars_InternalForces</c>).
  /// Имена свойств соответствуют заголовкам столбцов таблицы:
  /// Загружение, СЭ, Тип КЭ, Элемент, Сечение,
  /// N (т), MX (т*м), MY (т*м), QZ (т), MZ (т*м), QY (т),
  /// RY (т), RZ (т), BW (т·мм), TW (т*м).
  /// Первые пять столбцов — целые, усилия — числа (System.Single), здесь хранятся как <see cref="double"/>.
  /// </summary>
  [DebuggerDisplay("LoadCase: {LoadCase}, SE: {SE}, ElementType: {ElementType}, ElementNumber: {ElementNumber}, SectionNumber: {SectionNumber}, N: {N}, MX: {MX}, MY: {MY}, QZ: {QZ}, MZ: {MZ}, QY: {QY}")]
  public class kLiraTable_Bars_InternalForces_TableRow : ILiraTableRow
  {
    /// <summary>Загружение — номер загружения</summary>
    public int LoadCase { get; set; }

    /// <summary>СЭ — номер составного элемента (суперэлемента)</summary>
    public int SE { get; set; }

    /// <summary>Тип КЭ — тип конечного элемента</summary>
    public int ElementType { get; set; }

    /// <summary>Элемент — номер конечного элемента</summary>
    public int ElementNumber { get; set; }

    /// <summary>Сечение — номер расчётного сечения</summary>
    public int SectionNumber { get; set; }

    /// <summary>N — продольное усилие, т</summary>
    public double N { get; set; }

    /// <summary>MX — изгибающий момент относительно X, т*м</summary>
    public double MX { get; set; }

    /// <summary>MY — изгибающий момент относительно Y, т*м</summary>
    public double MY { get; set; }

    /// <summary>QZ — поперечная сила по Z, т</summary>
    public double QZ { get; set; }

    /// <summary>MZ — крутящий момент относительно Z, т*м</summary>
    public double MZ { get; set; }

    /// <summary>QY — поперечная сила по Y, т</summary>
    public double QY { get; set; }

    /// <summary>RY — реакция по Y, т</summary>
    public double RY { get; set; }

    /// <summary>RZ — реакция по Z, т</summary>
    public double RZ { get; set; }

    /// <summary>BW — бимомент, т·мм</summary>
    public double BW { get; set; }

    /// <summary>TW — изгибно-крутящий момент, т*м</summary>
    public double TW { get; set; }

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_Bars_InternalForces_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_Bars_InternalForces_TableRow(
        int loadCase,
        int se,
        int elementType,
        int elementNumber,
        int sectionNumber,
        double n,
        double mx,
        double my,
        double qz,
        double mz,
        double qy,
        double ry,
        double rz,
        double bw,
        double tw)
    {
      LoadCase = loadCase;
      SE = se;
      ElementType = elementType;
      ElementNumber = elementNumber;
      SectionNumber = sectionNumber;
      N = n;
      MX = mx;
      MY = my;
      QZ = qz;
      MZ = mz;
      QY = qy;
      RY = ry;
      RZ = rz;
      BW = bw;
      TW = tw;
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — LoadCase, 1 — SE, 2 — ElementType, 3 — ElementNumber,
    /// 4 — SectionNumber, 5 — N, 6 — MX, 7 — MY, 8 — QZ, 9 — MZ, 10 — QY,
    /// 11 — RY, 12 — RZ, 13 — BW, 14 — TW.
    /// </summary>
    public static kLiraTable_Bars_InternalForces_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_Bars_InternalForces_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      LoadCase = ToInt(array[rowIndex, 0]);
      SE = ToInt(array[rowIndex, 1]);
      ElementType = ToInt(array[rowIndex, 2]);
      ElementNumber = ToInt(array[rowIndex, 3]);
      SectionNumber = ToInt(array[rowIndex, 4]);
      N = ToDouble(array[rowIndex, 5]);
      MX = ToDouble(array[rowIndex, 6]);
      MY = ToDouble(array[rowIndex, 7]);
      QZ = ToDouble(array[rowIndex, 8]);
      MZ = ToDouble(array[rowIndex, 9]);
      QY = ToDouble(array[rowIndex, 10]);
      RY = ToDouble(array[rowIndex, 11]);
      RZ = ToDouble(array[rowIndex, 12]);
      BW = ToDouble(array[rowIndex, 13]);
      TW = ToDouble(array[rowIndex, 14]);
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
        => obj is kLiraTable_Bars_InternalForces_TableRow o
           && LoadCase == o.LoadCase
           && SE == o.SE
           && ElementType == o.ElementType
           && ElementNumber == o.ElementNumber
           && SectionNumber == o.SectionNumber
           && N.Equals(o.N)
           && MX.Equals(o.MX)
           && MY.Equals(o.MY)
           && QZ.Equals(o.QZ)
           && MZ.Equals(o.MZ)
           && QY.Equals(o.QY)
           && RY.Equals(o.RY)
           && RZ.Equals(o.RZ)
           && BW.Equals(o.BW)
           && TW.Equals(o.TW);

    public override int GetHashCode()
        => HashCode.Combine(
               HashCode.Combine(LoadCase, SE, ElementType, ElementNumber),
               HashCode.Combine(SectionNumber, N, MX, MY),
               HashCode.Combine(QZ, MZ, QY, RY),
               HashCode.Combine(RZ, BW, TW)
           );

    public override string ToString()
        => $"Загружение: {LoadCase}, СЭ: {SE}, Тип КЭ: {ElementType}, Элемент: {ElementNumber}, Сечение: {SectionNumber}, " +
           $"N: {N}, MX: {MX}, MY: {MY}, QZ: {QZ}, MZ: {MZ}, QY: {QY}, RY: {RY}, RZ: {RZ}, BW: {BW}, TW: {TW}";
  }
}
