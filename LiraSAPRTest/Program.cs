// See https://aka.ms/new-console-template for more information

// подключение библиотеки LiraFemRes (LiraSAPR 2026)
//using LiraFemRes;

// подключение библиотеки LiraSapphireRes (LiraSAPR 2025)
using LiraSapphire;
using LiraSapphireRes;
using LiraSAPRTest.ModelLiraSapr;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LiraSAPRTest
{


  internal class Program
  {

    // Импортируем нативные функции Windows для работы с COM-таблицей запущенных объектов (ROT)
    [DllImport("ole32.dll", CharSet = CharSet.Unicode)]
    private static extern int CLSIDFromProgIDEx(string lpszProgID, out Guid lpclsid);

    [DllImport("oleaut32.dll", PreserveSig = false)]
    private static extern void GetActiveObject(ref Guid rclsid, IntPtr pvReserved, [MarshalAs(UnmanagedType.IUnknown)] out object ppunk);



    static void Main(string[] args)
    {
      //GetNodeDisplacements();
      //LoadCombinationForces();
      //GetSelectedElem();
      //GetElementGeom();
      //GetSelectedElemReinforcement();
      //GetSelectedElemLocalAxes();
      DumpAllTableSchemas();
    }


    private static void GetSelectedElemLocalAxes()
    {
      // Запускаем ВИЗОР
      LiraApplication App = new LiraApplication();

      // Создаём в нём новую расчётную схему:
      LiraDocument Doc = App.ActiveDocument;

      object pNumbers = null; //Тут будут лежать номера выбранных в Лире элементов

      Doc.GetSelection(ref pNumbers, LiraSelEnum.kLiraSel_Els);

      // Явное приведение к массиву int
      object[] sNumbers = (object[])pNumbers;


     var elements = GetTableRows<kLiraTable_Plates_LocalAxes_TableRow>(Doc,
                            (LiraTableEnum)LiraTableEnumHelp.kLiraTable_Plates_LocalAxes);

      //Получаю только выбранные элементы
      var selected = elements.Where(q => sNumbers.Contains(q.ID)).ToList();

    }



    private static void GetElementGeom()
    {
      // Запускаем ВИЗОР
      LiraApplication App = new LiraApplication();

      // Создаём в нём новую расчётную схему:
      LiraDocument Doc = App.ActiveDocument;

      var Elements = GetTableRows<kLiraTable_Elements_TypeAndNodes_TableRow>(Doc,
                                  (LiraTableEnum)LiraTableEnumHelp.kLiraTable_Elements_TypeAndNodes);

      var Nodes = GetTableRows<kLiraTable_Nodes_Coordinates_TableRow>(Doc,
                               (LiraTableEnum)LiraTableEnumHelp.kLiraTable_Nodes_Coordinates);

      var Stiffnesses = GetTableRows<kLiraTable_Stiffnesses_TableRow>(Doc,
                                     (LiraTableEnum)LiraTableEnumHelp.kLiraTable_Stiffnesses);

      var ElementStiffnesses = GetTableRows<kLiraTable_ElementsStiffnesses_TableRow>(Doc,
                                       (LiraTableEnum)LiraTableEnumHelp.kLiraTable_Elements_Stiffnesses);





      int i = 0;
      // теперь обращение по именам, а не по индексам
      foreach (var el in Elements)
      {
        Console.WriteLine($"\n\t\tElement number: {el.ID}");
        Console.WriteLine($"\t\tТип:             {el.Type}");
        Console.WriteLine($"\t\tУзлы:             {string.Join(", ", el.NodeIds)}");
        if (i < 10) { i++; } else { i = 0; break; }
      }

      var groups = ElementGrouper.SplitIntoConnectedGroups(Elements);

      for (int g = 0; g < groups.Count; g++)
      {
        Console.WriteLine($"Группа {g}: {groups[g].Count} элементов");
      }

      // теперь обращение по именам, а не по индексам
      foreach (var el in Nodes)
      {
        Console.WriteLine($"\n\t\tNode number: {el.ID}");
        Console.WriteLine($"\t\tКоординаты:    {el.ToString()}");
        if (i < 10) { i++; } else { i = 0; break; }
      }

      // теперь обращение по именам, а не по индексам
      foreach (var el in Stiffnesses)
      {
        Console.WriteLine($"\n\t\tStiffnesse number: {el.Number}");
        Console.WriteLine($"\t\tТип:             {el.Type}");
        Console.WriteLine($"\t\tЦвет:             {el.Color}");
        Console.WriteLine($"\t\tКомментарий:  {el.Comment}");
        Console.WriteLine($"\t\tПараметры:      {el.Parameters}");
        if (i < 10) { i++; } else { i = 0; break; }
      }


      // теперь обращение по именам, а не по индексам
      foreach (var el in ElementStiffnesses)
      {
        Console.WriteLine($"\n\t\tElement number: {el.Number}");
        Console.WriteLine($"\t\tТип жесткости:    {el.StiffnessNumber}");
        if (i < 10) { i++; } else { i = 0; break; }
      }
    }

    public static void GetSelectedElemReinforcement()
    {
      // Запускаем ВИЗОР
      LiraApplication App = new LiraApplication();

      // Создаём в нём новую расчётную схему:
      LiraDocument Doc = App.ActiveDocument;

      object pNumbers = null; //Тут будут лежать номера выбранных в Лире элементов

      Doc.GetSelection(ref pNumbers, LiraSelEnum.kLiraSel_Els);

      // Явное приведение к массиву int
      object[] sNumbers = (object[])pNumbers;

      var Result = new LiraResultsAccess();

      // объявление и инициализация интерфейса Request (запрос)
      var Request = Result.CreateNewRequest(LiraRequestEnum.kLiraRequest_SelectedReinforcement) as LiraSelectedReinforcementRequest;

      // объявление интерфейса Response (ответ)
      LiraSelectedReinforcementResponse Response;

      // инициализация поля DocumentName (имя задачи)
      Request.DocumentName = Doc.Title;

      // инициализация поля, отвечающего за номер варианта подбора арматуры
      Request.DesignOption = 1;

      // инициализация списка элементов
      //Request.Elements.AddFromString("1611,75,96");


      // заполнение массива Elements номерами конечных элементов
      //Request.Elements.AddFromString("1-100");
      foreach (var element in sNumbers)
      {
        Request.Elements.AddFromString(element.ToString());
      }


      // инициализация массива категорий данных армирования
      //LiraReinforcementDataEnum[] RD = { LiraReinforcementDataEnum.kLiraReinforcementData_Total, LiraReinforcementDataEnum.kLiraReinforcementData_Strength, LiraReinforcementDataEnum.kLiraReinforcementData_Torsion };
      LiraReinforcementDataEnum[] RD = { LiraReinforcementDataEnum.kLiraReinforcementData_Total, LiraReinforcementDataEnum.kLiraReinforcementData_Strength };


      // строковое представление элементов RD
      //string[] RDstr = { "Total", "Strn.", "Tors." };
      string[] RDstr = { "Полн", "Проч"};

      Request.ReinforcementData.Count = RD.Length;

      // заполнение массива категорий данных армирования в запросе
      for (int i = 0; i < RD.Length; i++)
        Request.ReinforcementData.Item[i] = (int)RD[i];

      // обработка запроса
      try
      {
        Response = Result.SelectedReinforcement(Request);
      }
      catch (Exception e)
      {
        Console.WriteLine("Error: " + e.HResult);
        return;
      }

      // массив случайных элементов, для которых будут выведены на консоль данные армирования

      //int[] Elements = { 1611, 75, 96 };
      int[] Elements = sNumbers.Cast<int>().ToArray();

        string row = "+-------+------+-------+-------+-------+-------+-------+------+------+-------+-------+-------+------+";


      for (int i = 0; i < Elements.Length; i++)
      {


        Console.WriteLine(row);
        Console.WriteLine("| Elem. | Sec. | Type. |  AS1  |  AS2  |  AS3  |  AS4  |  %X  |  %Y  | ASW1  | ASW2  | Short | Long |");

        Console.WriteLine(row);



        int SecCount = Response.GetSectionCount(Elements[i]);



        for (int CS = 1; CS <= SecCount; CS++)
        {
          //for (int s = 1; s >= 0; s--)
          //{
            for (int k = 0; k < RD.Length; k++)
            {
            //// симметрия / асимметрия
            //string SA = "S";
            //if (s == 0)
            //  SA = "A";
            int s = 0;
              // код ошибок
              int Err = 0;
              // получение данных армирования
              float AS1 = Response.GetPlateAS1(Elements[i], RD[k], s, out Err);
              float AS2 = Response.GetPlateAS2(Elements[i], RD[k], s, out Err);
              float AS3 = Response.GetPlateAS3(Elements[i], RD[k], s, out Err);
              float AS4 = Response.GetPlateAS4(Elements[i], RD[k], s, out Err);
              float PreX = Response.GetPlatePercX(Elements[i],s, out Err);
              float PreY = Response.GetPlatePercY(Elements[i], s, out Err);
              float ASW1 = Response.GetPlateASW1(Elements[i], RD[k], s, out Err);
              float ASW2 = Response.GetPlateASW2(Elements[i], RD[k], s, out Err);
              float CrShrt = Response.GetPlateCrackSHORT(Elements[i], s, out Err);
              float CrLong = Response.GetPlateCrackLONG(Elements[i], s, out Err);



              //// получение данных армирования
              //float AU1 = Response.GetBarAU1(Elements[i], CS, RD[k], s, 0, out Err);
              //float AU2 = Response.GetBarAU2(Elements[i], CS, RD[k], s, 0, out Err);
              //float AU3 = Response.GetBarAU3(Elements[i], CS, RD[k], s, 0, out Err);
              //float AU4 = Response.GetBarAU4(Elements[i], CS, RD[k], s, 0, out Err);
              //float AS1 = Response.GetBarAS1(Elements[i], CS, RD[k], s, 0, out Err);
              //float AS2 = Response.GetBarAS2(Elements[i], CS, RD[k], s, 0, out Err);
              //float AS3 = Response.GetBarAS3(Elements[i], CS, RD[k], s, 0, out Err);
              //float AS4 = Response.GetBarAS4(Elements[i], CS, RD[k], s, 0, out Err);
              //float Pre = Response.GetBarPERC(Elements[i], CS, RD[k], s, 0, out Err);
              //float ASW1 = Response.GetBarASW1(Elements[i], CS, RD[k], s, 0, out Err);
              //float ASW2 = Response.GetBarASW2(Elements[i], CS, RD[k], s, 0, out Err);
              //float CrShrt = Response.GetBarCrackSHORT(Elements[i], CS, s, 0, out Err);
              //float CrLong = Response.GetBarCrackLONG(Elements[i], CS, s, 0, out Err);



              // вывод на консоль данных армирования
              Console.Write("| {0,-5} | {1,-4} | {2,-5} ", Elements[i], CS, RDstr[k]);
              Console.Write("| {0,-5:#.##} | {1,-5:#.##} | {2,-5:#.##} | {3,-5:#.##} ", AS1, AS2, AS3, AS4);
              Console.Write("| {0,-4:#.##} | {1,-4:#.##} | {2,-5:#.##} | {3,-5:#.##} ", PreX, PreY, ASW1, ASW2);
              Console.WriteLine("| {0,-5:#.##} | {1,-4:#.##} |", CrShrt, CrLong);



            }

          //}

        }

      }

      Console.WriteLine(row);

    }

    public static void GetSelectedElem()
    {
      // Запускаем ВИЗОР
      LiraApplication App = new LiraApplication();



      // Создаём в нём новую расчётную схему:
      LiraDocument Doc = App.ActiveDocument;

      object pNumbers = null; //Тут будут лежать номера выбранных в Лире элементов

      Doc.GetSelection(ref pNumbers, LiraSelEnum.kLiraSel_Els);

      // Явное приведение к массиву int
      object[] sNumbers = (object[])pNumbers;

      var elements = GetTableRows<kLiraTable_Elements_TypeAndNodes_TableRow>(Doc, (LiraTableEnum)LiraTableEnumHelp.kLiraTable_Elements_TypeAndNodes);


      var nodes = GetTableRows<kLiraTable_Nodes_Coordinates_TableRow>(Doc, (LiraTableEnum)LiraTableEnumHelp.kLiraTable_Nodes_Coordinates);

      //Получаю только выбранные элементы
      var selected = elements.Where(q => sNumbers.Contains(q.ID)).ToList();


      var groups = ElementGrouper.SplitIntoConnectedGroups(selected);

      for (int g = 0; g < groups.Count; g++)
      {
        Console.WriteLine($"Группа {g}: {groups[g].Count} элементов");
      }

      //GetLoadCombinationForces(Doc, selected);

    }



    /// <summary>
    /// Обобщённый метод: находит или создаёт таблицу нужного типа и заполняет список строк.
    /// </summary>
    private static List<T> GetTableRows<T>(LiraDocument doc, LiraTableEnum tableEnum)
        where T : ILiraTableRow, new()
    {
      LiraTable table = GetOrCreateTable(doc, tableEnum);

      object data = null;
      table.GetContents(ref data);


      var array = (object[,])data;

      int rows = array.GetLength(0);
      var result = new List<T>(rows);

      for (int i = 0; i < rows; i++)
      {
        var row = new T();
        row.LoadFromArray(array, i);
        result.Add(row);
      }

      return result;
    }

    private static LiraTable GetOrCreateTable(LiraDocument doc, LiraTableEnum tableEnum)
    {
      // Ищем уже существующую таблицу нужного типа
      for (int i = 0; i < doc.AllTables.ItemCount; i++)
      {
        if (doc.AllTables.Item[i].Type == tableEnum)
        {
          var tabl = doc.AllTables.Item[i] as LiraTable;
          if (tabl != null)
          {
            tabl.RefillFromModel(); //обновляем модель
            return tabl;
          }
        }
      }
      // Не нашли — создаём
      return doc.AllTables.CreateNewItem(tableEnum) as LiraTable;
    }

    /// <summary>
    /// Одноразовая утилита: печатает для каждой таблицы из <see cref="LiraTableEnumHelp"/>
    /// количество строк/столбцов и для каждого столбца — типы значений и пример.
    /// Нужно запустить один раз внутри Lira, скопировать вывод и по нему сгенерировать классы.
    /// </summary>
    private static void DumpAllTableSchemas()
    {
      LiraApplication App = new LiraApplication();
      LiraDocument Doc = App.ActiveDocument;

      foreach (LiraTableEnumHelp enumValue in Enum.GetValues(typeof(LiraTableEnumHelp)))
      {
        // Служебные значения — не реальные таблицы.
        if (enumValue == LiraTableEnumHelp.kLiraTable_AllTables ||
            enumValue == LiraTableEnumHelp.kLiraTable_Cnt)
          continue;

        DumpTableSchema(Doc, enumValue);
      }
    }

    private static void DumpTableSchema(LiraDocument doc, LiraTableEnumHelp enumValue)
    {
      string name = enumValue.ToString();
      try
      {
        LiraTable table = GetOrCreateTable(doc, (LiraTableEnum)enumValue);
        if (table == null)
        {
          Console.WriteLine($"=== {name} : GetOrCreateTable вернул null ===");
          return;
        }

        object data = null;
        table.GetContents(ref data);

        if (data is not object[,] array)
        {
          Console.WriteLine($"=== {name} : GetContents вернул {data?.GetType().FullName ?? "null"} ===");
          return;
        }

        int rows = array.GetLength(0);
        int cols = array.GetLength(1);
        Console.WriteLine();
        Console.WriteLine($"=== {name} : rows={rows}, cols={cols} ===");

        if (rows > 0)
        {
          var firstRow = new List<string>(cols);
          for (int c = 0; c < cols; c++)
            firstRow.Add(FormatCell(array[0, c]));
          Console.WriteLine($"    row0: [{string.Join(" | ", firstRow)}]");
        }
        else
        {
          Console.WriteLine("    (нет строк — типы столбцов определить нельзя)");
        }

        for (int c = 0; c < cols; c++)
        {
          var types = new List<string>();
          var seen = new HashSet<string>();
          object sample = null;

          for (int r = 0; r < rows; r++)
          {
            object v = array[r, c];
            if (v == null || v is DBNull)
              continue;

            string t = v.GetType().FullName ?? v.GetType().Name;
            if (seen.Add(t))
              types.Add(t);

            if (sample == null)
              sample = v;
          }

          string typeStr = types.Count == 0 ? "<empty>" : string.Join(" | ", types);
          string sampleStr = sample == null ? "<none>" : FormatCell(sample);
          Console.WriteLine($"    [{c}] {typeStr}  sample={sampleStr}");
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"=== {name} : ERROR {ex.GetType().Name} — {ex.Message} ===");
      }
    }

    private static string FormatCell(object value)
    {
      if (value == null)
        return "<null>";
      if (value is DBNull)
        return "<DBNull>";
      if (value is string s)
        return s.Length <= 80 ? $"\"{s}\"" : $"\"{s.Substring(0, 80)}…\"";

      if (value is System.Collections.IEnumerable enumerable)
      {
        var items = new List<string>();
        int count = 0;
        foreach (object item in enumerable)
        {
          if (++count > 5)
            break;
          items.Add(item?.ToString() ?? "null");
        }
        return $"[{string.Join(", ", items)}{(count > 5 ? ", …" : "")}]";
      }

      return value.ToString() ?? "<null>";
    }


    private static void GetLoadCombinationForces(LiraDocument Doc, List<kLiraTable_Elements_TypeAndNodes_TableRow> elements)
    {
      var Result = new LiraResultsAccess();

      // объявление и инициализация интерфейса Request (запрос)
      var Request = Result.CreateNewRequest(LiraRequestEnum.kLiraRequest_LoadCombinationForces) as LiraLoadCombinationForcesRequest;

      // объявление интерфейса Response (ответ)
      LiraLoadCombinationForcesResponse Response;

      // инициализация поля DocumentName (имя задачи)
      Request.DocumentName = Doc.Title;

      // заполнение массива Elements номерами конечных элементов
      //Request.Elements.AddFromString("1-100");
      foreach (kLiraTable_Elements_TypeAndNodes_TableRow element in elements)
      {
        Request.Elements.AddFromString(element.ID.ToString());
      }

      // инициализация номера таблицы РСН
      Request.LoadCombinationTable = 1;


      // инициализация массива, содержащего перечень предельных состояний внутренних усилий (РСН расчетные и РСН нормативные)

      // для получения всех предельных состояний необходимо оставить поле LoadCombinationLimitState пустым или перечислить все виды предельных состояний

      Request.LoadCombinationLimitState.Count = 2;



      // РСН расчетные

      Request.LoadCombinationLimitState.Item[0] = (int)LiraLimitStateForcesEnum.kLiraLimitStateForces_UltimateFull;



      // РСН нормативные
      Request.LoadCombinationLimitState.Item[1] = (int)LiraLimitStateForcesEnum.kLiraLimitStateForces_ServiceabilityFull;


      // обработка запроса
      try
      {
        Response = Result.LoadCombinationForces(Request);
      }
      catch (Exception e)
      {
        Console.WriteLine("Error: " + e.HResult);
        return;
      }

      // получение данных о всех загружениях
      //LiraLoadCaseInfos LoadCases = Response.LoadCases;

      // получение данных о всех РСН
      LiraLoadCombinationInfos LoadCombinations = Response.LoadCombinations;


      // инициализация перечня внутренних усилий (LiraForceEnum)
      string[] StringElementForces = { "Undefined", "BarN", "BarMx", "BarMy", "BarQz", "BarMz", "BarQy", "BarRy", "BarRz", "BarBw", "BarTw", "PlateNx", "PlateNy", "PlateNz", "PlateTxy", "PlateTxz", "PlateMx", "PlateMy", "PlateMxy", "PlateQx", "PlateQy", "PlateRz", "SolidNx", "SolidNy", "SolidNz", "SolidTxy", "SolidTxz", "SolidTyz", "SpecElementRx", "SpecElementRy", "SpecElementRz", "SpecElementRux", "SpecElementRuy", "SpecElementRuz", "Spec_58_59_Qz", "Spec_58_59_Ny", "Spec_58_59_Qx", "Spec_310_N", "Spec_310_My", "Spec_310_Qz", "Spec_310_Mx", "Spec_310_Mz", "Spec_310_Qy", "Spec_264_N", "Spec_264_Qz", "Spec_264_Qy" };


      // перечень наименований семейств конечных элементов (LiraElementFamilyEnum)
      string[] ElementFamilyes = { "Undefined", "Bar", "Plate", "Solid", "SpecElement", "Spec_58_5", "Spec_310", "Spec_264" };


      for (int i = 0; i < LoadCombinations.Count; i++)
      {
        LiraLoadCombinationInfo LoadCase = LoadCombinations.Item[i];

        // вывод номера загружения
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n\n###########################################################");
        Console.WriteLine("\tLoad case number: " + LoadCase.Number + $" {LoadCase.Name}\n\n");
        Console.ResetColor();


        // номер РСН
        int LC = LoadCombinations.Item[i].Number;


        foreach (var el in elements)
        {
          // номер элемента
          int Element = el.ID;

          // вывод на консоль номера конечного элемента
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine("\n\t\tElement number: " + Element);
          Console.ResetColor();

          // вывод на консоль номера загружения
          Console.WriteLine("Load combination number: " + LC);

          // получение семейства (вида) конечного элемента
          int ElementFamily = (int)Response.GetFamily(Element);

          // вывод на консоль названия семейства, к которому принадлежит конечный элемент
          Console.WriteLine("Element family: " + ElementFamilyes[ElementFamily]);

          // получение усилий в конечном элементе
          LiraLongsArray ElementForces = Response.GetForces(Element);

          // вывод на консоль перечня усилий в конечном элементе
          Console.Write("Forces: ");

          for (int j = 0; j < ElementForces.Count; j++)
            Console.Write(StringElementForces[ElementForces.Item[j]] + " ");


          Console.WriteLine("\n------------------------------------");

          //Forces: PlateNx PlateNy PlateTxy PlateMx PlateMy PlateMxy PlateQx PlateQy PlateRz

          int SecCount = Response.GetSectionCount(Element);

          // вывод значений усилий в элементе Element от РСН LC для всех сечений
          // РСН расчетные
          int LS = (int)LiraLimitStateForcesEnum.kLiraLimitStateForces_UltimateFull;
          Console.WriteLine("РСН расчетные - Ultimate full");

          for (int CS = 1; CS <= SecCount; CS++)
          {
            Console.WriteLine("PlateNx: " + Response.GetPlateNx(Element, CS, LC, LS));
            Console.WriteLine("PlateNy: " + Response.GetPlateNy(Element, CS, LC, LS));
          }

          // РСН нормативные
          LS = (int)LiraLimitStateForcesEnum.kLiraLimitStateForces_ServiceabilityFull;
          Console.WriteLine("РСН нормативные - Serviceability full");
          for (int CS = 1; CS <= SecCount; CS++)
          {
            Console.WriteLine("PlateNx: " + Response.GetPlateNx(Element, CS, LC, LS));
            Console.WriteLine("PlateNy: " + Response.GetPlateNy(Element, CS, LC, LS));
          }

        }




      }
    }


    public static void LoadCombinationForces()
    {

      var Result = new LiraResultsAccess();

      // объявление и инициализация интерфейса Request (запрос)
      var Request = Result.CreateNewRequest(LiraRequestEnum.kLiraRequest_LoadCombinationForces) as LiraLoadCombinationForcesRequest;

      // объявление интерфейса Response (ответ)
      LiraLoadCombinationForcesResponse Response;

      // инициализация поля DocumentName (имя задачи)
      Request.DocumentName = "1_6";

      // заполнение массива Elements номерами конечных элементов
      Request.Elements.AddFromString("1-100");

      // инициализация номера таблицы РСН
      Request.LoadCombinationTable = 1;


      // инициализация массива, содержащего перечень предельных состояний внутренних усилий (РСН расчетные и РСН нормативные)

      // для получения всех предельных состояний необходимо оставить поле LoadCombinationLimitState пустым или перечислить все виды предельных состояний

      Request.LoadCombinationLimitState.Count = 2;



      // РСН расчетные

      Request.LoadCombinationLimitState.Item[0] = (int)LiraLimitStateForcesEnum.kLiraLimitStateForces_UltimateFull;



      // РСН нормативные
      Request.LoadCombinationLimitState.Item[1] = (int)LiraLimitStateForcesEnum.kLiraLimitStateForces_ServiceabilityFull;


      // обработка запроса
      try
      {
        Response = Result.LoadCombinationForces(Request);
      }
      catch (Exception e)
      {
        Console.WriteLine("Error: " + e.HResult);
        return;
      }

      // получение данных о всех загружениях
      //LiraLoadCaseInfos LoadCases = Response.LoadCases;

      // получение данных о всех РСН
      LiraLoadCombinationInfos LoadCombinations = Response.LoadCombinations;




      for (int i = 0; i < LoadCombinations.Count; i++)
      {
        LiraLoadCombinationInfo LoadCase = LoadCombinations.Item[i];

        // вывод номера загружения
        Console.WriteLine("Load case number: " + LoadCase.Number + $" {LoadCase.Name}");

        //// вывод количества форм
        //Console.WriteLine("Number of dynamic mode shapes: " + LoadCase.DynamicModeShapeCount);

        //// вывод количества составляющих
        //Console.WriteLine("Number of deformed shapes: " + LoadCase.DeformedShapeCount);

        //// вывод номеров составляющих
        //var DynamicModeShapesArray = new LiraLongsArray();
        //DynamicModeShapesArray = LoadCase.DeformedShapes;
        //Console.Write("Deformed shapes: ");
        //for (int j = 0; j < DynamicModeShapesArray.Count; j++)
        //  Console.Write(DynamicModeShapesArray.Item[j] + " ");

        //Console.WriteLine("\n------------------------------------");
      }

      // номер загружения
      //int LC = LoadCases.Item[0].Number;

      // номер РСН
      int LC = LoadCombinations.Item[0].Number;

      // номер элемента
      int Element = 93;

      // вывод на консоль номера конечного элемента
      Console.WriteLine("Element number: " + Element);

      // вывод на консоль номера загружения
      Console.WriteLine("Load combination number: " + LC);

      // перечень наименований семейств конечных элементов (LiraElementFamilyEnum)
      string[] ElementFamilyes = { "Undefined", "Bar", "Plate", "Solid", "SpecElement", "Spec_58_5", "Spec_310", "Spec_264" };

      // получение семейства (вида) конечного элемента
      int ElementFamily = (int)Response.GetFamily(Element);

      // вывод на консоль названия семейства, к которому принадлежит конечный элемент
      Console.WriteLine("Element family: " + ElementFamilyes[ElementFamily]);


      // инициализация перечня внутренних усилий (LiraForceEnum)
      string[] StringElementForces = { "Undefined", "BarN", "BarMx", "BarMy", "BarQz", "BarMz", "BarQy", "BarRy", "BarRz", "BarBw", "BarTw", "PlateNx", "PlateNy", "PlateNz", "PlateTxy", "PlateTxz", "PlateMx", "PlateMy", "PlateMxy", "PlateQx", "PlateQy", "PlateRz", "SolidNx", "SolidNy", "SolidNz", "SolidTxy", "SolidTxz", "SolidTyz", "SpecElementRx", "SpecElementRy", "SpecElementRz", "SpecElementRux", "SpecElementRuy", "SpecElementRuz", "Spec_58_59_Qz", "Spec_58_59_Ny", "Spec_58_59_Qx", "Spec_310_N", "Spec_310_My", "Spec_310_Qz", "Spec_310_Mx", "Spec_310_Mz", "Spec_310_Qy", "Spec_264_N", "Spec_264_Qz", "Spec_264_Qy" };



      // получение усилий в конечном элементе
      LiraLongsArray ElementForces = Response.GetForces(Element);

      // вывод на консоль перечня усилий в конечном элементе
      Console.Write("Forces: ");

      for (int i = 0; i < ElementForces.Count; i++)
        Console.Write(StringElementForces[ElementForces.Item[i]] + " ");


      Console.WriteLine("\n------------------------------------");

      //Forces: PlateNx PlateNy PlateTxy PlateMx PlateMy PlateMxy PlateQx PlateQy PlateRz

      int SecCount = Response.GetSectionCount(Element);

      // вывод значений усилий в элементе Element от РСН LC для всех сечений
      // РСН расчетные
      int LS = (int)LiraLimitStateForcesEnum.kLiraLimitStateForces_UltimateFull;
      Console.WriteLine("РСН расчетные - Ultimate full");

      for (int CS = 1; CS <= SecCount; CS++)
      {
        Console.WriteLine("PlateNx: " + Response.GetPlateNy(Element, CS, LC, LS));
      }

      // РСН нормативные
      LS = (int)LiraLimitStateForcesEnum.kLiraLimitStateForces_ServiceabilityFull;
      Console.WriteLine("РСН нормативные - Serviceability full");
      for (int CS = 1; CS <= SecCount; CS++)
      {
        Console.WriteLine("PlateNy: " + Response.GetPlateNy(Element, CS, LC, LS));
      }
      //// количество сечений в элементе
      //int SectionCount = Response.GetSectionCount(Element);

      //// вывод значений усилий в элементе Element от загружения LC для всех сечений
      //for (int i = 1; i <= SectionCount; i++)
      //{
      //  Console.WriteLine("Section: " + i);
      //  Console.WriteLine("BarN: " + Response.GetBarN(Element, i, LC));
      //  Console.WriteLine("BarMx: " + Response.GetBarMx(Element, i, LC));
      //  Console.WriteLine("BarMy: " + Response.GetBarMy(Element, i, LC));
      //  Console.WriteLine("BarQz: " + Response.GetBarQz(Element, i, LC));
      //  Console.WriteLine("BarMz: " + Response.GetBarMz(Element, i, LC));
      //  Console.WriteLine("BarQy: " + Response.GetBarQy(Element, i, LC));

      //  Console.WriteLine("");

      //}



    }

    public static void GetNodeDisplacements()
    {
      var Result = new LiraResultsAccess();
      // объявление и инициализация интерфейса Request (запрос)
      var Request = Result.CreateNewRequest(LiraRequestEnum.kLiraRequest_LoadCaseDisplacement)
      as LiraLoadCaseDisplacementsRequest;

      // объявление интерфейса Response (ответ)
      LiraLoadCaseDisplacementsResponse Response;

      // инициализация поля DocumentName (имя задачи)
      Request.DocumentName = "1_6";

      // заполнение массива Nodes номерами узлов, перемещения которых мы хотим получить
      Request.Nodes.AddFromString("1-6");

      Response = Result.LoadCaseDisplacements(Request);

      // номер загружения, от которого мы хотим получить перемещения в узлах
      int lc = 1;
      for (int i = 0; i < Request.Nodes.Count; i++)
      {
        double X, Y, Z;
        int node = Request.Nodes.Item[i];
        // получение данных о перемещении узла node от загружения lc
        X = Math.Round(Response.GetNodeX(node, lc) * 1000, 4, MidpointRounding.AwayFromZero);
        Y = Math.Round(Response.GetNodeY(node, lc) * 1000, 4, MidpointRounding.AwayFromZero);
        Z = Math.Round(Response.GetNodeZ(node, lc) * 1000, 4, MidpointRounding.AwayFromZero);
        // вывод результатов на консоль
        Console.WriteLine("Node: " + node);
        Console.WriteLine("X: " + X);
        Console.WriteLine("Y: " + Y);
        Console.WriteLine("Z: " + Z);
        Console.WriteLine();
      }
    }
  }
}