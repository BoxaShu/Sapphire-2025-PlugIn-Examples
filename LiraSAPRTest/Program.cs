// See https://aka.ms/new-console-template for more information

// подключение библиотеки LiraFemRes (LiraSAPR 2026)
//using LiraFemRes;

// подключение библиотеки LiraSapphireRes (LiraSAPR 2025)
using LiraSapphire;
using LiraSapphireRes;
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
      GetSelectedElem();
      //GetElementGeom();

    }

    private static void GetElementGeom()
    {


    }

    public static void GetSelectedElem()
    {
      // Запускаем ВИЗОР
      LiraApplication App = new LiraApplication();
      // Создаём в нём новую расчётную схему:
      LiraDocument Doc = App.ActiveDocument;

      object pNumbers=null; //Тут будут лежать номера выбранных в Лире элементов

      Doc.GetSelection(ref pNumbers, LiraSelEnum.kLiraSel_Els);

      //LiraTable Els = Doc.AllTables.Item[LiraTableEnum.kLiraTable_Elements_TypeAndNumbersOfNodes] as LiraTable;
      LiraTable Els = Doc.AllTables.CreateNewItem(LiraTableEnum.kLiraTable_Elements_TypeAndNodes) as LiraTable;


      object data = null;
      Els.GetContents(ref data);

      // Явное приведение к двумерному массиву int
      object[,] array = (object[,])data;

      // Доступ к элементам
      object value0 = array[92, 0]; // 2
      object value1 = array[92, 1]; // 2
      object value2 = array[92, 2]; // 2


      //var el = data[93 - 1];
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
        LiraLoadCombinationInfo  LoadCase = LoadCombinations.Item[i];

        // вывод номера загружения
        Console.WriteLine("Load case number: " + LoadCase.Number+ $" {LoadCase.Name}");

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
        Console.WriteLine("PlateNx: " + Response.GetPlateNy(Element, CS, LC, LS));
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