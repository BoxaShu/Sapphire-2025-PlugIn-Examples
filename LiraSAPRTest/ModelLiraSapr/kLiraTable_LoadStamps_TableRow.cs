using System;
using System.Diagnostics;
using System.Globalization;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы «Нагрузки-штамп» (<c>kLiraTable_LoadStamps</c>).
  /// Имена свойств соответствуют заголовкам столбцов таблицы:
  /// Тип, Направление, Линия/Контур, Вектор, Комментарий, Интенсивность, Шаг триангуляции.
    /// «Линия/Контур», «Вектор», «Интенсивность» и «Шаг триангуляции» заданы в метрах и имеют тип
    /// <see cref="double"/>, остальные — строки.
    /// </summary>
  [DebuggerDisplay("Type: {Type}, Intensity: {Intensity}, Step: {TriangulationStep}")]
  public class kLiraTable_LoadStamps_TableRow : ILiraTableRow
  {
    /// <summary>Тип создаваемой нагрузки («Нагрузка на контур», «Нагрузка по линии», «Нагрузка-штамп»)</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Направление действия нагрузки (вектор в глобальной СК, например "0, 0, -1")</summary>
    public string Direction { get; set; } = string.Empty;

    /// <summary>Линия / Контур, м — длина линии/контура</summary>
        public double LineOrContour { get; set; }

        /// <summary>Вектор, м — величина вектора действия нагрузки</summary>
        public double Vector { get; set; }

    /// <summary>Комментарий (например «Нагрузка от пожарной машины»)</summary>
    public string Comment { get; set; } = string.Empty;

    /// <summary>Интенсивность, т, м — значение нагрузки (погонная или распределённая по площади)</summary>
    public double Intensity { get; set; }

    /// <summary>Шаг триангуляции, м — параметр дискретизации нагрузки</summary>
    public double TriangulationStep { get; set; }

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_LoadStamps_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_LoadStamps_TableRow(
            string type, string direction, double lineOrContour, double vector,
        string comment, double intensity, double triangulationStep)
    {
      Type = type;
      Direction = direction;
      LineOrContour = lineOrContour;
      Vector = vector;
      Comment = comment;
      Intensity = intensity;
      TriangulationStep = triangulationStep;
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — Type, 1 — Direction, 2 — LineOrContour, 3 — Vector,
    /// 4 — Comment, 5 — Intensity, 6 — TriangulationStep.
    /// </summary>
    public static kLiraTable_LoadStamps_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_LoadStamps_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      Type = ToText(array[rowIndex, 0]);
      Direction = ToText(array[rowIndex, 1]);
      LineOrContour = ToDouble(array[rowIndex, 2]);
            Vector = ToDouble(array[rowIndex, 3]);
      Comment = ToText(array[rowIndex, 4]);
      Intensity = ToDouble(array[rowIndex, 5]);
      TriangulationStep = ToDouble(array[rowIndex, 6]);
    }

    private static string ToText(object value)
        => value == null || value is DBNull ? string.Empty : value.ToString() ?? string.Empty;

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
        => obj is kLiraTable_LoadStamps_TableRow o
           && Type == o.Type
           && Direction == o.Direction
           && LineOrContour.Equals(o.LineOrContour)
                      && Vector.Equals(o.Vector)
           && Comment == o.Comment
           && Intensity.Equals(o.Intensity)
           && TriangulationStep.Equals(o.TriangulationStep);

    public override int GetHashCode()
        => HashCode.Combine(Type, Direction, LineOrContour, Vector, Comment, Intensity, TriangulationStep);

    public override string ToString()
        => $"Тип: {Type}, Интенсивность: {Intensity}, Шаг триангуляции: {TriangulationStep}";
  }
}