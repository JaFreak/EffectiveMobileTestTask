// создаем файл
using System.Collections.Generic;
using System.Globalization;

string path = Directory.GetCurrentDirectory();
string inputFileNameAndPath = path + "\\initial_data.txt";
string logFileNameAndPath = path + "\\_deliveryLog.txt";
string finalFileNameAndPath = path + "\\_deliveryOrder.txt";

//переменные для фильтрации и сортировки заказов
string districtFilter = null;
int orderNumber = 0;
double orderWeight = 0;
DateTime orderTime = new DateTime();

Random rnd = new Random();

string district0 = "downTown";
string district1 = "centre";
string district2 = "suburbia";
string district3 = "industrial";
var startTime = DateTime.Today;
List<Order> ordersList = new List<Order>(1000);
List<Order> ordersListForWork = new List<Order>(ordersList.Capacity);
List<Order> ordersSorted = new List<Order>(ordersList.Capacity);
GenerateLogFile(logFileNameAndPath, "Программа запущена");
Console.WriteLine("Выберите действие \n1 Генерировать файл с заказами\n2 Выбор готового файла с заказами" +
    "\nЛюбое другое значение для выхода");
int f = int.Parse(Console.ReadLine());
switch (f)
{
    case 1:        
        //генерируем файл с заказами
        GenerateInputFile(inputFileNameAndPath);
        break;
    case 2:

        Console.WriteLine("Введите путь к текстовому документу с заказами");
        //inputFileNameAndPath = string.Format("@{0}",Console.ReadLine());
        inputFileNameAndPath = Console.ReadLine();
        string logMes = "Выбран файл с заказами. Расположение файла:" + inputFileNameAndPath;
        GenerateLogFile(logFileNameAndPath, logMes);
        break;
    default:
        Console.WriteLine("Отмена");
        GenerateLogFile(logFileNameAndPath, "Выход из программы \n");
        return;
}

Console.WriteLine("Выберите действие \n1 Ввод параметров фильтрации\n2 Выбор файлов с параметрами фильтрации" +
    "\nЛюбое другое значение для выхода");
int i = int.Parse(Console.ReadLine());
DateTime timeStarFilter = DateTime.Now;
DateTime timeEndFilter = timeStarFilter.AddMinutes(30);
switch (i)
{
    case 1:
        GenerateLogFile(logFileNameAndPath, "Начало ввода параметров фильтрации");
        Console.WriteLine("Ввод параметров фильтрации");
        // вывод вариантов для выбора района        
        ChooseDistrict(out districtFilter);

        //запрос начала отсчета времени для сортировки заказов
        string timeFormat = "HH:mm:ss";
        Console.WriteLine("Введите время первого заказа в формате 00:00:00");
        string timeFromConcole = Console.ReadLine();
        if (DateTime.TryParseExact(timeFromConcole, timeFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out timeStarFilter))
        {
            timeEndFilter = timeStarFilter.AddMinutes(30);
            Console.WriteLine("Введено время {0}", timeStarFilter);
        }
        else
        {
            Console.WriteLine("Не верный ввод, программа завершила работу");
            string logMes0 = "Ввод параметров фильтрации завершен с ошибкой. " + "Выбран район: " + districtFilter + ". Время введено некорректно: " + timeFromConcole + "\n";
            GenerateLogFile(logFileNameAndPath, logMes0);
            break;
        }
        string logMes = "Ввод параметров фильтрации завершен. " + "Выбран район: " + districtFilter + ". Введено время: " + timeStarFilter;
        GenerateLogFile(logFileNameAndPath, logMes);
        //чтение содержимого текстового файла с заказами
        OrderExtractor(inputFileNameAndPath, districtFilter, timeStarFilter, timeEndFilter, ordersListForWork);        
        return;
    case 2:
        Console.WriteLine("Введите путь к текстовому документу с параметрами фильтрации заказов (название_района HH:mm:ss)");
        string filterFileNameAndPath = Console.ReadLine();
        string logMes2 = "Выбран файл с параметрами фильтрации заказов. Расположение файла:" + filterFileNameAndPath;
        GenerateLogFile(logFileNameAndPath, logMes2);
        string districtFilterFromFile = null;
        using (StreamReader reader = new StreamReader(filterFileNameAndPath))
        {
            string? line;
            line = reader.ReadLine(); //считываем строку из файла

            string[] lineSplitFilter = line.Split(' ');
            districtFilterFromFile = lineSplitFilter[0];
            Console.WriteLine("Выбран район {0}", districtFilterFromFile);

            string timeFormat2 = "HH:mm:ss";
            if (DateTime.TryParseExact(lineSplitFilter[1], timeFormat2, CultureInfo.CurrentCulture, DateTimeStyles.None, out timeStarFilter))
            {
                timeEndFilter = timeStarFilter.AddMinutes(30);
                Console.WriteLine("Введено время {0}", timeStarFilter);
                string logMes3 = "Ввод параметров фильтрации завершен." + "Выбран район " + districtFilterFromFile + "Введено время: " + timeStarFilter;
                GenerateLogFile(logFileNameAndPath, logMes3);
            }
            else
            {
                Console.WriteLine("Время указано в неправильном формате, укажите время в формате HH:mm:ss. Программа завершила работу");
                string logMes4 = "Ввод параметров фильтрации завершен с ошибкой." + "Выбран район " + districtFilterFromFile + "Время введено некорректно: " + lineSplitFilter[1] + "\n";
                GenerateLogFile(logFileNameAndPath, logMes4);
                return;
            }
            reader.Close();
        }

        OrderExtractor(inputFileNameAndPath, districtFilterFromFile, timeStarFilter, timeEndFilter, ordersListForWork);
        return;
    default:
        Console.WriteLine("Отмена");
        GenerateLogFile(logFileNameAndPath, "Выход из программы.\n");
        return;
}


void GenerateInputFile(string pathToFile)
{
    using (StreamWriter writer = new StreamWriter(pathToFile, false))
    {
        for (int l = 0; l < ordersList.Capacity; l++)
        {
            int number = l + 1;
            string nameForOrder = "order" + number.ToString();
            double weight = 0.1 + rnd.NextDouble() * (10.67 - 0.1);
            weight = Math.Round(weight, 3);
            int districtNumber = rnd.Next(0, 3);
            var newTime = startTime.AddSeconds(rnd.Next(0, 86400));
            string districtName = null;
            if (districtNumber == 0)
                districtName = district0;
            else if (districtNumber == 1)
                districtName = district1;
            else if (districtNumber == 2)
                districtName = district2;
            else if (districtNumber == 3)
                districtName = district3;
            Order newOrder = new Order();
            ordersList.Insert(l, newOrder);
            ordersList[l].orderName = nameForOrder;
            ordersList[l].number = number;
            ordersList[l].weight = weight;
            ordersList[l].district = districtName;
            ordersList[l].orderTime = newTime;

            writer.WriteLine("{0} {1} {2} {3}", ordersList[l].number.ToString(), ordersList[l].weight.ToString(),
              ordersList[l].district.ToString(), ordersList[l].orderTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        writer.Close();
    }
    GenerateLogFile(logFileNameAndPath, "Создан файл с заказами");
    Console.WriteLine("Файл создан");
}
void ChooseDistrict(out string districtName)
{
    Console.WriteLine("Выберите район города \n1 downTown\n2 centre\n3 suburbia\n4 industrial\nЛюбое другое значение для выхода");
    int value = int.Parse(Console.ReadLine());
    switch (value)
    {
        case 1:
            districtName = "downTown";
            Console.WriteLine("Выбран район {0}", districtName);
            break;
        case 2:
            districtName = "centre";
            Console.WriteLine("Выбран район {0}", districtName);
            break;
        case 3:
            districtName = "suburbia";
            Console.WriteLine("Выбран район {0}", districtName);
            break;
        case 4:
            districtName = "industrial";
            Console.WriteLine("Выбран район {0}", districtName);
            break;
        default:
            districtName = null;
            Console.WriteLine("Отмена");
            GenerateLogFile(logFileNameAndPath, "Район не задан. Выход из программы");
            return;
    }
}

void OrderExtractor(string fileNameAndPath, string districtName, DateTime start, DateTime end, List<Order> filteredOrders)
{
    using (StreamReader reader = new StreamReader(fileNameAndPath))
    {
        string? line;
        int o = 0;
        while ((line = reader.ReadLine()) != null) //считываем строки из файла
        {
            string[] lineSplit = line.Split(' '); //делим одну строку на набор строк по словам
            if (lineSplit[2] == districtName)     //зная структуру файла с заказами, проверяем относится ли заказ к нужному району
            {
                string[] timeValue = new string[] { lineSplit[3], lineSplit[4] };
                string timeFromFile = string.Join(" ", timeValue);
                if (DateTime.TryParseExact(timeFromFile, "yyyy-MM-dd HH:mm:ss",
                    CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime orderTime)) //дату и время разделило, объеденям снова
                {
                    if (orderTime >= start && orderTime <= end) //проверяем попадает ли заказ в нужный временной интервал
                    {
                        //если попадает создаем объект класса order и присваиваем ему свойства
                        Order order = new Order();
                        if (int.TryParse(lineSplit[0], out int orderNumber))
                            order.number = orderNumber;
                        if (double.TryParse(lineSplit[1], out double orderWeight))
                            order.weight = orderWeight;

                        order.district = districtName;
                        order.orderTime = orderTime;
                        //складываем объекты в список
                        filteredOrders.Insert(o, order);
                        o++;
                    }
                    else
                        continue;
                }
                else
                    continue;
            }
            else
                continue;
        }
        reader.Close();
    }
    filteredOrders.Sort((x, y) => DateTime.Compare(x.orderTime, y.orderTime));
    using (StreamWriter writer2 = new StreamWriter(finalFileNameAndPath, false))
    {
        foreach (Order o in filteredOrders)
        {
            Console.WriteLine("{0} {1} {2} {3}", o.number.ToString(), o.weight.ToString(),
              o.district.ToString(), o.orderTime.ToString("yyyy-MM-dd HH:mm:ss"));

            writer2.WriteLine("{0} {1} {2} {3}", o.number.ToString(), o.weight.ToString(),
          o.district.ToString(), o.orderTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        writer2.Close();
    }
    string logMes = "Файл с результатом выборки сформирован. Отобрано заказов: "+ filteredOrders.Count.ToString()+". Путь к файлу: " + finalFileNameAndPath + ". Программа завершила работу.\n";
    GenerateLogFile(logFileNameAndPath, logMes);
}

void GenerateLogFile(string pathToFile, string message)
{
    using (StreamWriter logWriter = new StreamWriter(pathToFile, true))
    {
        logWriter.WriteLine("Время записи: {0}     Событие: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message);
        logWriter.Close();
    }
}

class Order
{
    public string orderName { get; set; }
    public int number { get; set; }
    public double weight { get; set; }
    public string district { get; set; }
    public DateTime orderTime { get; set; }
}
