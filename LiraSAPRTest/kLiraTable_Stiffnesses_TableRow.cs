using System;

namespace LiraSAPRTest
{
  //[DebuggerDisplay("ID: {ID}, X: {X}, Y: {Y}, Z: {Z}")]
  public class kLiraTable_Stiffnesses_TableRow : ILiraTableRow
  {
    /// <summary>Номер по порядку</summary>
    public int Number { get; set; }

    /// <summary>Тип</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Цвет</summary>
    public string Color { get; set; } = string.Empty;

    /// <summary>Комментарий</summary>
    public string Comment { get; set; } = string.Empty;

    /// <summary>Параметры (характеристики материала / м², характеристика сечения — см)</summary>
    public string Parameters { get; set; } = string.Empty;

    /// <summary>Основной материал</summary>
    public string MainMaterial { get; set; } = string.Empty;

    /// <summary>Арматурный материал</summary>
    public string RebarMaterial { get; set; } = string.Empty;

    /// <summary>Ползучесть бетона</summary>
    public string ConcreteCreep { get; set; } = string.Empty;

    /// <summary>Параметры арматуры</summary>
    public string RebarParameters { get; set; } = string.Empty;

    // Удобный конструктор без параметров (для инициализатора объекта)
    public kLiraTable_Stiffnesses_TableRow() { }

    // Полный конструктор
    public kLiraTable_Stiffnesses_TableRow(
        int number,
        string type,
        string color,
        string comment,
        string parameters,
        string mainMaterial,
        string rebarMaterial,
        string concreteCreep,
        string rebarParameters)
    {
      Number = number;
      Type = type;
      Color = color;
      Comment = comment;
      Parameters = parameters;
      MainMaterial = mainMaterial;
      RebarMaterial = rebarMaterial;
      ConcreteCreep = concreteCreep;
      RebarParameters = rebarParameters;
    }

    // Фабрика из строки object[,] — тонкая обёртка над LoadFromArray
    public static kLiraTable_Stiffnesses_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_Stiffnesses_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    /// <summary>
    /// Реализация <see cref="ILiraTableRow"/>: заполняет экземпляр из строки массива.
    /// Порядок столбцов: 0 — Number, 1 — Type, 2 — Color, 3 — Comment, 4 — Parameters,
    /// 5 — MainMaterial, 6 — RebarMaterial, 7 — ConcreteCreep, 8 — RebarParameters.
    /// </summary>
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      Number = int.TryParse(array[rowIndex, 0]?.ToString(), out int n) ? n : 0;
      Type = array[rowIndex, 1]?.ToString() ?? string.Empty;
      Color = array[rowIndex, 2]?.ToString() ?? string.Empty;
      Comment = array[rowIndex, 3]?.ToString() ?? string.Empty;
      Parameters = array[rowIndex, 4]?.ToString() ?? string.Empty;
      MainMaterial = array[rowIndex, 5]?.ToString() ?? string.Empty;
      RebarMaterial = array[rowIndex, 6]?.ToString() ?? string.Empty;
      ConcreteCreep = array[rowIndex, 7]?.ToString() ?? string.Empty;
      RebarParameters = array[rowIndex, 8]?.ToString() ?? string.Empty;
    }

    public override bool Equals(object? obj)
        => obj is kLiraTable_Stiffnesses_TableRow other
           && Number == other.Number
           && Type == other.Type
           && Color == other.Color
           && Comment == other.Comment
           && Parameters == other.Parameters
           && MainMaterial == other.MainMaterial
           && RebarMaterial == other.RebarMaterial
           && ConcreteCreep == other.ConcreteCreep
           && RebarParameters == other.RebarParameters;

    public override int GetHashCode()
        => HashCode.Combine(
               HashCode.Combine(Number, Type, Color, Comment),
               HashCode.Combine(Parameters, MainMaterial, RebarMaterial),
               HashCode.Combine(ConcreteCreep, RebarParameters)
           );
  }
}