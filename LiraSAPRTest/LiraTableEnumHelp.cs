using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace LiraSAPRTest; //LiraSapphire;

//[Guid("6426790C-4E48-459A-B5FA-4D06745F9760")]
// Внимание: описания ниже восстановлены по названиям элементов enum.
// Они не являются официальной документацией API Lira-SAPR и в критичных местах
// требуют проверки по документации/заголовкам API.

public enum LiraTableEnumHelp
{
  /// <summary>
  /// Служебное значение: «все таблицы» / выбор всех таблиц.
  /// Реальной таблицей может не быть.
  /// </summary>
  kLiraTable_AllTables,

  /// <summary>
  /// Группы: объединение узлов, элементов или фрагментов для выбора,
  /// фильтрации, назначения нагрузок и т.п.
  /// </summary>
  kLiraTable_Group,

  /// <summary>
  /// Узлы: номера и координаты X, Y, Z.
  /// </summary>
  kLiraTable_Nodes_Coordinates,

  /// <summary>
  /// Элементы: тип конечного элемента и список образующих его узлов.
  /// </summary>
  kLiraTable_Elements_TypeAndNodes,

  /// <summary>
  /// Узлы: закрепления/связи — запреты или задание степеней свободы.
  /// </summary>
  kLiraTable_Nodes_Restraints,

  /// <summary>
  /// Пластины: параметры C1/C2, обычно коэффициенты упругого основания
  /// или аналогичные коэффициенты.
  /// </summary>
  kLiraTable_Plates_C1C2,

  /// <summary>
  /// Стержни: параметры C1/C2, вероятно, коэффициенты упругого основания
  /// или специальных связей стержня.
  /// </summary>
  kLiraTable_Bars_C1C2,

  /// <summary>
  /// Специальные элементы: параметры C1/C2 для специальных связей/элементов.
  /// </summary>
  kLiraTable_Special_C1C2,

  /// <summary>
  /// Нелинейные диаграммы: зависимости σ–ε, законы нелинейного
  /// деформирования материалов/связей.
  /// </summary>
  kLiraTable_NonlinearDiagrams,

  /// <summary>
  /// Жёсткости: библиотека/список типов жёсткостей, сечений и их параметров.
  /// </summary>
  kLiraTable_Stiffnesses,

  /// <summary>
  /// Привязка элементов к жёсткостям.
  /// </summary>
  kLiraTable_Elements_Stiffnesses,

  /// <summary>
  /// PRB/ПРБ: вероятно, расчётные блоки — пользовательские, промежуточные
  /// или пространственные расчётные блоки. Точную трактовку лучше уточнить.
  /// </summary>
  kLiraTable_PRBs,

  /// <summary>
  /// Пластины: смещения/эксцентриситеты относительно срединной поверхности
  /// или узлов.
  /// </summary>
  kLiraTable_Plates_Offsets,

  /// <summary>
  /// Объединение степеней свободы: жёсткие/шарнирные связи между узлами.
  /// </summary>
  kLiraTable_CoupledDOFs,

  /// <summary>
  /// Стержни: смещения/эксцентриситеты стержней.
  /// </summary>
  kLiraTable_Bars_Offsets,

  /// <summary>
  /// Стержни: шарниры, в том числе по концам или в промежуточных точках.
  /// </summary>
  kLiraTable_Bars_Hinges,

  /// <summary>
  /// Узлы: местные оси.
  /// </summary>
  kLiraTable_Nodes_LocalAxes,

  /// <summary>
  /// Стержни: местные оси.
  /// </summary>
  kLiraTable_Bars_LocalAxes,

  /// <summary>
  /// Пластины: местные оси.
  /// </summary>
  kLiraTable_Plates_LocalAxes,

  /// <summary>
  /// Пластины: оси ортотропии.
  /// </summary>
  kLiraTable_Plates_AxesOfOrthotropy,

  /// <summary>
  /// Объёмные элементы: местные оси.
  /// </summary>
  kLiraTable_Solids_LocalAxes,

  /// <summary>
  /// Объёмные элементы: оси ортотропии.
  /// </summary>
  kLiraTable_Solids_AxesOfOrthotropy,

  /// <summary>
  /// Элементы: количество расчётных сечений — для армирования, проверок,
  /// РСУ/РСН и т.п.
  /// </summary>
  kLiraTable_Elements_DesignSectionsCount,

  /// <summary>
  /// Монтажные стадии / стадии возведения.
  /// </summary>
  kLiraTable_AssemblageStages,

  /// <summary>
  /// DCL: вероятно, динамические степени свободы или динамические
  /// связи/характеристики. Лучше уточнить по API.
  /// </summary>
  kLiraTable_DCL,

  /// <summary>
  /// Загружения: список и параметры статических загружений.
  /// </summary>
  kLiraTable_LoadCases,

  /// <summary>
  /// Стержни: внутренние усилия — возможно, начальные/предварительные
  /// усилия или специальные данные.
  /// </summary>
  kLiraTable_Bars_InternalForces,

  /// <summary>
  /// Сбор масс: преобразование нагрузок/элементов в массы для динамики.
  /// </summary>
  kLiraTable_CollectionOfMasses,

  /// <summary>
  /// Коэффициенты/модификаторы жёсткостей.
  /// </summary>
  kLiraTable_Stiffnesses_Factors,

  /// <summary>
  /// Строительные оси: основные/прямоугольные оси сооружения.
  /// </summary>
  kLiraTable_ConstructionAxes,

  /// <summary>
  /// Отметки высот/уровни.
  /// </summary>
  kLiraTable_ElevationMarks,

  /// <summary>
  /// Структурные блоки: разбиение схемы на блоки.
  /// </summary>
  kLiraTable_StructuralBlocks,

  /// <summary>
  /// Элементы: материалы.
  /// </summary>
  kLiraTable_Elements_Materials,

  /// <summary>
  /// Элементы: типы PR — вероятно, типы предельных состояний,
  /// проверок или расчётных параметров.
  /// </summary>
  kLiraTable_Elements_PRTypes,

  /// <summary>
  /// Динамические загружения.
  /// </summary>
  kLiraTable_DynLoadCases,

  /// <summary>
  /// Строительные оси: радиальные/полярные.
  /// </summary>
  kLiraTable_ConstructionAxesRad,

  /// <summary>
  /// Нагрузки на элементы — элементные нагрузки.
  /// </summary>
  kLiraTable_LoadElement,

  /// <summary>
  /// Нагрузки: узловые, стержневые, пластинчатые и др.
  /// </summary>
  kLiraTable_Loads,

  /// <summary>
  /// Штампы нагрузок: шаблоны/отпечатки нагрузочных областей.
  /// </summary>
  kLiraTable_LoadStamps,

  /// <summary>
  /// Аналоги стержней: замена/приведение стержневых элементов.
  /// </summary>
  kLiraTable_BarAnalogs,

  /// <summary>
  /// Эксцентриситеты: смещения приложения нагрузок/элементов.
  /// </summary>
  kLiraTable_Eccentricity,

  /// <summary>
  /// Аналоги пластин.
  /// </summary>
  kLiraTable_PlateAnalogs,

  /// <summary>
  /// Фрагменты нагрузок.
  /// </summary>
  kLiraTable_LoadsFragment,

  /// <summary>
  /// Коэффициенты диссипации.
  /// </summary>
  kLiraTable_DissipationCoefficients,

  /// <summary>
  /// Коэффициенты Рэлея — демпфирование.
  /// </summary>
  kLiraTable_RayleighCoefficients,

  /// <summary>
  /// Коэффициенты ФВК — вероятно, формы вынужденных колебаний;
  /// динамика/сейсмика.
  /// </summary>
  kLiraTable_FVKCoefficients,

  /// <summary>
  /// Пользовательские данные мозаики/визуализации.
  /// </summary>
  kLiraTable_UserMosaicData,

  /// <summary>
  /// Служебный счётчик количества элементов enum.
  /// Не является реальной таблицей.
  /// </summary>
  kLiraTable_Cnt
}

