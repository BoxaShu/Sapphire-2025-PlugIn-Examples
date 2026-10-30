using System;
using System.Diagnostics;

namespace LiraSAPRTest.ModelLiraSapr
{
  /// <summary>
  /// Строка таблицы «Монтажные стадии» (<c>kLiraTable_AssemblageStages</c>).
  /// Имена свойств соответствуют заголовкам столбцов таблицы:
  /// номер монтажной стадии, номер монтируемых элементов, номер демонтируемых элементов.
  /// Все столбцы — целые.
  /// Замечание: в тестовой модели таблица была пуста, типы определены по заголовкам.
  /// </summary>
  [DebuggerDisplay("StageNumber: {StageNumber}, MountedElementNumber: {MountedElementNumber}, DismountedElementNumber: {DismountedElementNumber}")]
  public class kLiraTable_AssemblageStages_TableRow : ILiraTableRow
  {
    /// <summary>Номер монтажной стадии</summary>
    public int StageNumber { get; set; }

    /// <summary>Номер монтируемых элементов</summary>
    public int MountedElementNumber { get; set; }

    /// <summary>Номер демонтируемых элементов</summary>
    public int DismountedElementNumber { get; set; }

    /// <summary>Конструктор без параметров</summary>
    public kLiraTable_AssemblageStages_TableRow() { }

    /// <summary>Полный конструктор</summary>
    public kLiraTable_AssemblageStages_TableRow(
        int stageNumber,
        int mountedElementNumber,
        int dismountedElementNumber)
    {
      StageNumber = stageNumber;
      MountedElementNumber = mountedElementNumber;
      DismountedElementNumber = dismountedElementNumber;
    }

    /// <summary>
    /// Создаёт строку из строки двумерного массива <paramref name="array"/>.
    /// Ожидаемый порядок столбцов: 0 — StageNumber, 1 — MountedElementNumber,
    /// 2 — DismountedElementNumber.
    /// </summary>
    public static kLiraTable_AssemblageStages_TableRow FromArray(object[,] array, int rowIndex)
    {
      var row = new kLiraTable_AssemblageStages_TableRow();
      row.LoadFromArray(array, rowIndex);
      return row;
    }

    // А это — реализация интерфейса
    public void LoadFromArray(object[,] array, int rowIndex)
    {
      StageNumber = ToInt(array[rowIndex, 0]);
      MountedElementNumber = ToInt(array[rowIndex, 1]);
      DismountedElementNumber = ToInt(array[rowIndex, 2]);
    }

    private static int ToInt(object value)
    {
      if (value == null || value is DBNull) return 0;
      return value is int i ? i : (int.TryParse(value.ToString(), out int parsed) ? parsed : 0);
    }

    public override bool Equals(object? obj)
        => obj is kLiraTable_AssemblageStages_TableRow o
           && StageNumber == o.StageNumber
           && MountedElementNumber == o.MountedElementNumber
           && DismountedElementNumber == o.DismountedElementNumber;

    public override int GetHashCode()
        => HashCode.Combine(StageNumber, MountedElementNumber, DismountedElementNumber);

    public override string ToString()
        => $"Номер монтажной стадии: {StageNumber}, " +
           $"Номер монтируемых элементов: {MountedElementNumber}, " +
           $"Номер демонтируемых элементов: {DismountedElementNumber}";
  }
}
