using System;
using System.Diagnostics;
using System.Globalization;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы «Динамические загружения» (<c>kLiraTable_DynLoadCases</c>).
  /// Имена свойств соответствуют заголовкам столбцов таблицы:
  /// N, Имя, (пустой), Тип, Полное наименование типа, Формы, %(0/1), %, %% танг., %% верт.,
  /// C(0/1), Метод, М(0/1), Уч(0/1), Nст, Кэi, Параметры.
  /// Флаги (0/1) — <see cref="bool"/>, процентные величины — <see cref="double"/>,
  /// N/Nст — <see cref="int"/>, остальное — строки.
  /// </summary>
  [DebuggerDisplay("N: {Number}, Name: {Name}, Type: {Type}, Method: {Method}")]
  public class kLiraTable_DynLoadCases_TableRow : ILiraTableRow
  {
    /// <summary>N — номер строки (порядковый номер)</summary>
    public int Number { get; set; }

    /// <summary>Имя — название или идентификатор воздействия/нагрузки</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Пустой столбец (предположительно для иконок или статуса)</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Тип воздействия (например, сейсмическое, ветровое, вибрационное)</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Полное наименование типа динамического воздействия</summary>
    public string TypeFullName { get; set; } = string.Empty;

    /// <summary>Формы — собственные формы колебаний</summary>
    public string Forms { get; set; } = string.Empty;

    /// <summary>%(0/1) — флаг учёта процентного соотношения</summary>
    public bool PercentEnabled { get; set; }

    /// <summary>% — числовое значение в процентах</summary>
    public double Percent { get; set; }

    /// <summary>%% танг. — процент тангенциальной (касательной) составляющей</summary>
    public double TangentialPercent { get; set; }

    /// <summary>%% верт. — процент вертикальной составляющей</summary>
    public double VerticalPercent { get; set; }

    /// <summary>C(0/1) — флаг учёта параметра (жёсткости или коэффициента)</summary>
    public bool CEnabled { get; set; }

    /// <summary>Метод — метод расчёта (например, модальный анализ, прямой динамический расчёт)</summary>
    public string Method { get; set; } = string.Empty;

    /// <summary>М(0/1) — флаг учёта массы</summary>
    public bool MassEnabled { get; set; }

    /// <summary>Уч(0/1) — флаг «Учитывать» (включить воздействие в расчёт)</summary>
    public bool ConsiderEnabled { get; set; }

    /// <summary>Nст — номер степени свободы или статического случая</summary>
    public int StaticCaseNumber { get; set; }

    /// <summary>Кэi — коэффициент (динамичности или эквивалентности)</summary>
    public double Kei { get; set; }

    /// <summary>Параметры — дополнительные настройки для конкретного воздействия</summary>
    public string Parameters { get; set; } = string.Empty;

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_DynLoadCases_TableRow() { }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — Number, 1 — Name, 2 — Status, 3 — Type,
    /// 4 — TypeFullName, 5 — Forms, 6 — PercentEnabled, 7 — Percent,
    /// 8 — TangentialPercent, 9 — VerticalPercent, 10 — CEnabled, 11 — Method,
    /// 12 — MassEnabled, 13 — ConsiderEnabled, 14 — StaticCaseNumber, 15 — Kei, 16 — Parameters.
    /// </summary>
    public static kLiraTable_DynLoadCases_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_DynLoadCases_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      Number = ToInt(array[rowIndex, 0]);
      Name = ToText(array[rowIndex, 1]);
      Status = ToText(array[rowIndex, 2]);
      Type = ToText(array[rowIndex, 3]);
      TypeFullName = ToText(array[rowIndex, 4]);
      Forms = ToText(array[rowIndex, 5]);
      PercentEnabled = ToBool(array[rowIndex, 6]);
      Percent = ToDouble(array[rowIndex, 7]);
      TangentialPercent = ToDouble(array[rowIndex, 8]);
      VerticalPercent = ToDouble(array[rowIndex, 9]);
      CEnabled = ToBool(array[rowIndex, 10]);
      Method = ToText(array[rowIndex, 11]);
      MassEnabled = ToBool(array[rowIndex, 12]);
      ConsiderEnabled = ToBool(array[rowIndex, 13]);
      StaticCaseNumber = ToInt(array[rowIndex, 14]);
      Kei = ToDouble(array[rowIndex, 15]);
      Parameters = ToText(array[rowIndex, 16]);
    }

    private static string ToText(object value)
        => value == null || value is DBNull ? string.Empty : value.ToString() ?? string.Empty;

    /// <summary>
    /// Приводит значение столбца к <see cref="bool"/> (флаг 0/1).
    /// Принимаются числа (0/1) и строки "0"/"1"/"true"/"false"; null/DBNull/пусто дают false.
    /// </summary>
    private static bool ToBool(object value)
    {
      if (value == null || value is DBNull) return false;
      switch (value)
      {
        case bool b: return b;
            case int i: return i != 0;
        case long l: return l != 0;
        case double d: return d != 0d;
        case float f: return f != 0f;
        default:
          var text = value.ToString();
          if (string.IsNullOrWhiteSpace(text)) return false;
              if (bool.TryParse(text, out bool parsedBool)) return parsedBool;
              return int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int parsedInt) && parsedInt != 0;
      }
    }

    /// <summary>
    /// Приводит значение столбца к <see cref="int"/>.
    /// Пустые строки/null/DBNull дают 0.
    /// </summary>
    private static int ToInt(object value)
    {
      if (value == null || value is DBNull) return 0;
      switch (value)
      {
        case int n: return n;
        case long l: return (int)l;
        case double d: return (int)d;
        case float f: return (int)f;
        default:
          var text = value.ToString();
          if (string.IsNullOrWhiteSpace(text)) return 0;
          return int.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out int current)
              ? current
              : (int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int invariant) ? invariant : 0);
      }
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
        => obj is kLiraTable_DynLoadCases_TableRow o
           && Number == o.Number
           && Name == o.Name
           && Status == o.Status
           && Type == o.Type
           && TypeFullName == o.TypeFullName
           && Forms == o.Forms
           && PercentEnabled == o.PercentEnabled
           && Percent.Equals(o.Percent)
           && TangentialPercent.Equals(o.TangentialPercent)
           && VerticalPercent.Equals(o.VerticalPercent)
           && CEnabled == o.CEnabled
           && Method == o.Method
           && MassEnabled == o.MassEnabled
           && ConsiderEnabled == o.ConsiderEnabled
           && StaticCaseNumber == o.StaticCaseNumber
           && Kei.Equals(o.Kei)
           && Parameters == o.Parameters;

    public override int GetHashCode()
        {
          var hash = new HashCode();
          hash.Add(Number);
          hash.Add(Name);
          hash.Add(Status);
          hash.Add(Type);
          hash.Add(TypeFullName);
          hash.Add(Forms);
          hash.Add(PercentEnabled);
          hash.Add(Percent);
          hash.Add(TangentialPercent);
          hash.Add(VerticalPercent);
          hash.Add(CEnabled);
          hash.Add(Method);
          hash.Add(MassEnabled);
          hash.Add(ConsiderEnabled);
          hash.Add(StaticCaseNumber);
          hash.Add(Kei);
          hash.Add(Parameters);
          return hash.ToHashCode();
        }

    public override string ToString()
        => $"N: {Number}, Имя: {Name}, Тип: {Type}, Метод: {Method}";
  }
}