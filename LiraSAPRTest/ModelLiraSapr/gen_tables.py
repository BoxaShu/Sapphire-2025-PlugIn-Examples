# -*- coding: utf-8 -*-
# One-off generator for *_TableRow classes based on the schema dump (column count + type
# observed in the "data" variable of GetTableRows). Safe to delete after generation.
import os

OUT = r"V:\_SAPR\Source\SapphireTest\LiraSAPRTest\ModelLiraSapr"

# code: i=int, u=uint, d=double, s=float(Single), t=string, o=object (type unknown / mixed / nullable)
CS = {
	'i': ('int',   'Convert.ToInt32(array[rowIndex, {i}])'),
	'u': ('uint',  'Convert.ToUInt32(array[rowIndex, {i}])'),
	'd': ('double','Convert.ToDouble(array[rowIndex, {i}])'),
	's': ('float', 'Convert.ToSingle(array[rowIndex, {i}])'),
	't': ('string','array[rowIndex, {i}]?.ToString() ?? string.Empty'),
	'o': ('object?','array[rowIndex, {i}]'),
}

def decl(name, code):
	t = CS[code][0]
	if code == 't':
		return f"    public string {name} {{ get; set; }} = string.Empty;"
	if code == 'o':
		return f"    public object? {name} {{ get; set; }}"
	return f"    public {t} {name} {{ get; set; }}"

def load(name, code, idx):
	return f"      {name} = " + CS[code][1].format(i=idx) + ";"

def hash_expr(props):
	if len(props) == 0:
		return "0"
	if len(props) == 1:
		return props[0]
	if len(props) <= 8:
		return "HashCode.Combine(" + ", ".join(props) + ")"
	mid = len(props) // 2
	left = hash_expr(props[:mid])
	right = hash_expr(props[mid:])
	return f"HashCode.Combine({left}, {right})"

def interp(props):
	return ", ".join(f"{p}: {{{p}}}" for p in props)

def gen_class(cls, summary, fields, extra_note=None):
	props = [f[0] for f in fields]
	lines = []
	lines.append("using System;")
	lines.append("using System.Diagnostics;")
	lines.append("")
	lines.append("namespace LiraSAPRTest.ModelLiraSapr")
	lines.append("{")
	lines.append("  /// <summary>")
	lines.append(f"  /// {summary}")
	if extra_note:
		lines.append(f"  /// {extra_note}")
	lines.append("  /// </summary>")
	dd = interp(props[:4])
	lines.append(f'  [DebuggerDisplay("{dd}")]')
	lines.append(f"  public class {cls} : ILiraTableRow")
	lines.append("  {")
	for p, c in fields:
		lines.append(decl(p, c))
		lines.append("")
	# paramless ctor
	lines.append(f"    public {cls}() {{ }}")
	lines.append("")
	# full ctor
	params = ", ".join(f"{CS[c][0]} {p}" for p, c in fields)
	lines.append(f"    public {cls}({params})")
	lines.append("    {")
	for p, c in fields:
		lines.append(f"      {p} = {p};")
	lines.append("    }")
	lines.append("")
	# FromArray
	lines.append(f"    public static {cls} FromArray(object[,] array, int rowIndex)")
	lines.append("    {")
	lines.append(f"      var row = new {cls}();")
	lines.append("      row.LoadFromArray(array, rowIndex);")
	lines.append("      return row;")
	lines.append("    }")
	lines.append("")
	# LoadFromArray
	lines.append("    public void LoadFromArray(object[,] array, int rowIndex)")
	lines.append("    {")
	for idx, (p, c) in enumerate(fields):
		lines.append(load(p, c, idx))
	lines.append("    }")
	lines.append("")
	# Equals
	cond = " && ".join(f"object.Equals({p}, o.{p})" for p in props)
	lines.append("    public override bool Equals(object? obj)")
	lines.append(f"        => obj is {cls} o")
	lines.append(f"           && {cond};")
	lines.append("")
	# GetHashCode
	lines.append("    public override int GetHashCode()")
	lines.append(f"        => {hash_expr(props)};")
	lines.append("")
	# ToString
	lines.append("    public override string ToString()")
	lines.append(f'        => $"{interp(props)}";')
	lines.append("  }")
	lines.append("}")
	lines.append("")
	return "\n".join(lines)

def gen_raw(cls, summary, extra_note=None):
	lines = []
	lines.append("using System;")
	lines.append("using System.Diagnostics;")
	lines.append("")
	lines.append("namespace LiraSAPRTest.ModelLiraSapr")
	lines.append("{")
	lines.append("  /// <summary>")
	lines.append(f"  /// {summary}")
	if extra_note:
		lines.append(f"  /// {extra_note}")
	lines.append("  /// </summary>")
	lines.append('  [DebuggerDisplay("Values: {Values.Length}")]')
	lines.append(f"  public class {cls} : ILiraTableRow")
	lines.append("  {")
	lines.append("    /// <summary>Сырые значения строки (особый «широкий»/служебный формат таблицы).</summary>")
	lines.append("    public object?[] Values { get; set; } = Array.Empty<object?>();")
	lines.append("")
	lines.append(f"    public {cls}() {{ }}")
	lines.append("")
	lines.append(f"    public static {cls} FromArray(object[,] array, int rowIndex)")
	lines.append("    {")
	lines.append(f"      var row = new {cls}();")
	lines.append("      row.LoadFromArray(array, rowIndex);")
	lines.append("      return row;")
	lines.append("    }")
	lines.append("")
	lines.append("    public void LoadFromArray(object[,] array, int rowIndex)")
	lines.append("    {")
	lines.append("      int cols = array.GetLength(1);")
	lines.append("      var values = new object?[cols];")
	lines.append("      for (int c = 0; c < cols; c++)")
	lines.append("        values[c] = array[rowIndex, c];")
	lines.append("      Values = values;")
	lines.append("    }")
	lines.append("")
	lines.append('    public override string ToString() => $"Values[{Values.Length}]";')
	lines.append("  }")
	lines.append("}")
	lines.append("")
	return "\n".join(lines)

def objcols(n):
	return [(f"Column{i}", 'o') for i in range(n)]

# (className, summary, fields, extra_note)
STANDARD = [
 ("kLiraTable_Nodes_Restraints_TableRow",
  "Строка таблицы закреплений (связей) в узлах. Столбцы 1-7 — строковые признаки связей.",
  [("NodeId",'i'),("X",'t'),("Y",'t'),("Z",'t'),("RotX",'t'),("RotY",'t'),("RotZ",'t'),("Other",'t')], None),

 ("kLiraTable_Plates_C1C2_TableRow",
  "Строка таблицы параметров C1/C2 пластин. Порядок/смысл коэффициентов требует проверки.",
  [("Number",'i'),("C1",'d'),("C2",'d'),("C3",'d'),("Mode",'i'),("Value",'d'),("Diagram",'t')], None),

 ("kLiraTable_Bars_C1C2_TableRow",
  "Строка таблицы параметров C1/C2 стержней. Порядок/смысл коэффициентов требует проверки.",
  [("Number",'i'),("C1",'d'),("C2",'d'),("Extra",'t'),("C3",'d'),("C4",'d'),("C5",'d'),("C6",'d'),
   ("C7",'d'),("Mode",'i'),("C8",'d'),("C9",'d'),("Diagram",'t')], None),

 ("kLiraTable_Special_C1C2_TableRow",
  "Строка таблицы параметров C1/C2 специальных элементов.",
  objcols(5), "Типы столбцов не определены: таблица была пуста в тестовой модели."),

 ("kLiraTable_NonlinearDiagrams_TableRow",
  "Строка таблицы нелинейных диаграмм (σ–ε и т.п.).",
  objcols(3), "Типы столбцов не определены: таблица была пуста в тестовой модели."),

 ("kLiraTable_PRBs_TableRow",
  "Строка таблицы расчётных блоков (PRB).",
  [("Number",'i'),("Name",'t'),("Node",'i'),("Elements",'t')], None),

 ("kLiraTable_Plates_Offsets_TableRow",
  "Строка таблицы смещений (эксцентриситетов) пластин.",
  [("Number",'i'),("Offset",'s')], None),

 ("kLiraTable_CoupledDOFs_TableRow",
  "Строка таблицы объединения степеней свободы. Строковые столбцы — DOF-признаки; последний — список узлов.",
  [("Number",'i'),("Kind",'i'),("X",'t'),("Y",'t'),("Z",'t'),("RotX",'t'),("RotY",'t'),("RotZ",'t'),
   ("Extra",'t'),("Nodes",'t')], None),

 ("kLiraTable_Bars_Offsets_TableRow",
  "Строка таблицы смещений (эксцентриситетов) стержней.",
  [("Number",'i'),("X",'s'),("Y",'s'),("Z",'s'),("RotX",'s'),("RotY",'s'),("RotZ",'s'),("Note",'t')], None),

 ("kLiraTable_Bars_Hinges_TableRow",
  "Строка таблицы шарниров стержней. Столбцы 11-14 могут содержать и число, и строку.",
  [("Number",'i'),("Hinge1",'t'),("Hinge2",'t'),("Hinge3",'t'),("Hinge4",'t'),("Hinge5",'t'),("Hinge6",'t'),
   ("Hinge7",'t'),("Hinge8",'t'),("Hinge9",'t'),("Hinge10",'t'),("Extra1",'o'),("Extra2",'o'),("Extra3",'o'),("Extra4",'o')], None),

 ("kLiraTable_Nodes_LocalAxes_TableRow",
  "Строка таблицы местных осей узлов.",
  [("NodeId",'i'),("X",'t'),("Y",'t'),("Z",'t'),("Extra",'t')], None),

 ("kLiraTable_Bars_LocalAxes_TableRow",
  "Строка таблицы местных осей стержней (угол согласования).",
  [("Number",'i'),("Angle",'s')], None),

 ("kLiraTable_Plates_AxesOfOrthotropy_TableRow",
  "Строка таблицы осей ортотропии пластин.",
  [("Number",'i'),("Angle",'s')], None),

 ("kLiraTable_Solids_LocalAxes_TableRow",
  "Строка таблицы местных осей объёмных элементов.",
  objcols(7), "Типы столбцов не определены: таблица была пуста в тестовой модели."),

 ("kLiraTable_Solids_AxesOfOrthotropy_TableRow",
  "Строка таблицы осей ортотропии объёмных элементов.",
  objcols(7), "Типы столбцов не определены: таблица была пуста в тестовой модели."),

 ("kLiraTable_Elements_DesignSectionsCount_TableRow",
  "Строка таблицы количества расчётных сечений элементов.",
  [("Number",'i'),("SectionsCount",'i')], None),

 ("kLiraTable_AssemblageStages_TableRow",
  "Строка таблицы монтажных стадий / стадий возведения.",
  objcols(3), "Типы столбцов не определены: таблица была пуста в тестовой модели."),

 ("kLiraTable_DCL_TableRow",
  "Строка таблицы DCL (динамические степени свободы / характеристики). Все столбцы строковые.",
  [("Number",'t'),("Name",'t'),("Kind",'t'),("Extra1",'t'),("Sign",'t'),("Value1",'t'),("Value2",'t'),
   ("Value3",'t'),("Value4",'t')], None),

 ("kLiraTable_LoadCases_TableRow",
  "Строка таблицы статических загружений.",
  [("Number",'i'),("Name",'t'),("Description",'t'),("Type",'t'),("Note",'t')], None),

 ("kLiraTable_Bars_InternalForces_TableRow",
  "Строка таблицы внутренних усилий стержней. Порядок усилий/моментов требует проверки.",
  [("Number",'t'),("LoadCase",'t'),("ElementId",'i'),("Section",'i'),("Kind",'i'),
   ("N",'s'),("Qy",'s'),("Qz",'s'),("Mx",'s'),("My",'s'),("Mz",'s'),("F7",'s'),("F8",'s'),("F9",'s'),("F10",'s')], None),

 ("kLiraTable_Stiffnesses_Factors_TableRow",
  "Строка таблицы коэффициентов/модификаторов жёсткостей.",
  [("Number",'i'),("Factor",'s')], None),

 ("kLiraTable_ConstructionAxes_TableRow",
  "Строка таблицы строительных (прямоугольных) осей.",
  objcols(6), "Типы столбцов не определены: таблица была пуста в тестовой модели."),

 ("kLiraTable_ElevationMarks_TableRow",
  "Строка таблицы отметок высот/уровней.",
  [("Elevation",'s'),("Mark",'t')], None),

 ("kLiraTable_StructuralBlocks_TableRow",
  "Строка таблицы структурных блоков.",
  [("Elements",'t'),("Type",'t'),("Number",'i'),("Elevation",'t'),("Extra",'t'),("Name",'t'),("Color",'t')], None),

 ("kLiraTable_Elements_Materials_TableRow",
  "Строка таблицы материалов элементов.",
  [("ElementId",'i'),("Material",'t'),("Code",'t')], None),

 ("kLiraTable_Elements_PRTypes_TableRow",
  "Строка таблицы типов PR элементов.",
  [("ElementId",'i'),("PRType",'t')], None),

 ("kLiraTable_DynLoadCases_TableRow",
  "Строка таблицы динамических загружений.",
  objcols(16), "Типы столбцов не определены: все значения в тестовой модели были null."),

 ("kLiraTable_ConstructionAxesRad_TableRow",
  "Строка таблицы радиальных/полярных строительных осей.",
  objcols(6), "Типы столбцов не определены: таблица была пуста в тестовой модели."),

 ("kLiraTable_LoadElement_TableRow",
  "Строка таблицы нагрузок на элементы (элементные нагрузки).",
  [("ElementId",'i'),("Load",'t')], None),

 ("kLiraTable_Loads_TableRow",
  "Строка таблицы нагрузок. Столбцы 4-5 не наблюдались (null).",
  [("Element",'t'),("LoadCase",'u'),("Extra",'t'),("Params",'t'),("Value1",'o'),("Value2",'o')], None),

 ("kLiraTable_LoadStamps_TableRow",
  "Строка таблицы штампов нагрузок.",
  objcols(7), "Типы столбцов не определены: таблица была пуста в тестовой модели."),

 ("kLiraTable_BarAnalogs_TableRow",
  "Строка таблицы аналогов стержней.",
  objcols(6), "Типы столбцов не определены: таблица была пуста в тестовой модели."),

 ("kLiraTable_Eccentricity_TableRow",
  "Строка таблицы эксцентриситетов. Столбцы 1-7 не наблюдались (null).",
  [("ElementId",'u'),("Ecc1",'o'),("Ecc2",'o'),("Ecc3",'o'),("Ecc4",'o'),("Ecc5",'o'),("Ecc6",'o'),("Ecc7",'o')], None),

 ("kLiraTable_PlateAnalogs_TableRow",
  "Строка таблицы аналогов пластин.",
  objcols(4), "Типы столбцов не определены: таблица была пуста в тестовой модели."),

 ("kLiraTable_LoadsFragment_TableRow",
  "Строка таблицы фрагментов нагрузок.",
  objcols(6), "Типы столбцов не определены: все значения в тестовой модели были null."),

 ("kLiraTable_DissipationCoefficients_TableRow",
  "Строка таблицы коэффициентов диссипации. Столбец 2 не наблюдался (null).",
  [("Id",'u'),("Number",'i'),("Value",'o')], None),

 ("kLiraTable_RayleighCoefficients_TableRow",
  "Строка таблицы коэффициентов Рэлея (демпфирование). Столбцы 1-2 не наблюдались (null).",
  [("Id",'u'),("Alpha",'o'),("Beta",'o')], None),

 ("kLiraTable_FVKCoefficients_TableRow",
  "Строка таблицы коэффициентов ФВК. Столбец 2 не наблюдался (null).",
  [("Id",'u'),("Number",'i'),("Value",'o')], None),

 ("kLiraTable_UserMosaicData_TableRow",
  "Строка таблицы пользовательских данных мозаики/визуализации.",
  [("Key",'t'),("Number",'i'),("S1",'t'),("S2",'t'),("S3",'t')], None),
]

RAW = [
 ("kLiraTable_CollectionOfMasses_TableRow",
  "Строка таблицы сбора масс. Таблица имеет особый «широкий» формат (>1000 столбцов).",
  "Хранится как сырой массив Values; структура столбцов специальная."),
 ("kLiraTable_Group_TableRow",
  "Строка таблицы групп (объединений узлов/элементов).",
  "Таблицу не удалось создать в тестовой модели (GetOrCreateTable вернул null); схема столбцов неизвестна, хранится как сырой массив."),
]

def main():
	written = []
	for cls, summary, fields, note in STANDARD:
		text = gen_class(cls, summary, fields, note)
		path = os.path.join(OUT, cls + ".cs")
		with open(path, "w", encoding="utf-8-sig") as f:
			f.write(text)
		written.append(path)
	for cls, summary, note in RAW:
		text = gen_raw(cls, summary, note)
		path = os.path.join(OUT, cls + ".cs")
		with open(path, "w", encoding="utf-8-sig") as f:
			f.write(text)
		written.append(path)
	print("Written files:", len(written))
	for p in written:
		print("  ", os.path.basename(p))

main()
