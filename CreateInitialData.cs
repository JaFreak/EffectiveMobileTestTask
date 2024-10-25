// создаем файл
using System.Collections.Generic;
using System.Globalization;

string path = Directory.GetCurrentDirectory();
string fileNameAndPath = path + "\\initial_data.txt";

//нужен рандомайзер
Random rnd = new Random();
// создать коллекцию с номерами заказов
string district0 = "downTown";
string district1 = "centre";
string district2 = "suburbia";
string district3 = "industrial";
var startTime = DateTime.Today;
List<Order> ordersList = new List<Order>(100);

//генерируем файл с заказами
using (StreamWriter writer = new StreamWriter(fileNameAndPath, false))
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
Console.WriteLine("Файл создан");

//переменные для фильтрации и сортировки заказов
string districtFilter = null;
List<Order> ordersListForWork = new List<Order>(ordersList.Count);
int orderNumber = 0;
double orderWeight = 0;
DateTime orderTime = new DateTime();

Console.WriteLine("Выберите действие \n1 Ввод параметров фильтрации\n2 Расчет\n3 Сохранение\n4 выход");

int i = int.Parse(Console.ReadLine());
DateTime timeStarFilter = DateTime.Now;
DateTime timeEndFilter = timeStarFilter.AddMinutes(30);
switch (i)
{
    case 1:
        Console.WriteLine("Ввод");
        // вывод вариантов для выбора района
        Console.WriteLine("Выберите район города \n1 downTown\n2 centre\n3 suburbia\n4 industrial\nЛюбое другое значение для выхода");
        int d = int.Parse(Console.ReadLine());
        ChooseDistrict(d, out districtFilter);

        //запрос начала отсчета времени для сортировки заказов
        string timeFormat = "HH:mm:ss";
        Console.WriteLine("Введите время первого заказа в формате 00:00:00");
        if (DateTime.TryParseExact(Console.ReadLine(), timeFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out timeStarFilter))
        {
            timeEndFilter = timeStarFilter.AddMinutes(30);
            Console.WriteLine("Введёно время {0}", timeStarFilter);
        }
        else
        {
            Console.WriteLine("Не верный ввод, программа завершила работу");
            break;
        }

        //чтение содержимого текстового файла с заказами
        OrderExtractor(fileNameAndPath, districtFilter, timeStarFilter, timeEndFilter, ref ordersListForWork);
        //using (StreamReader reader = new StreamReader(fileNameAndPath))
        //{
        //    string? line;
        //    int o = 0;
        //    while ((line = await reader.ReadLineAsync()) != null) //считываем строки из файла
        //    {
        //        string[] lineSplit = line.Split(' '); //делим одну строку на набор строк по словам
        //        if (lineSplit[2] == districtFilter)     //зная структуру файла с заказами, проверяем относится ли заказ к нужному району
        //        {
        //            string[] timeValue = new string[] { lineSplit[3], lineSplit[4] };
        //            string timeFromFile = string.Join(" ", timeValue);
        //            if (DateTime.TryParseExact(timeFromFile, "yyyy-MM-dd HH:mm:ss",
        //                CultureInfo.CurrentCulture, DateTimeStyles.None, out orderTime)) //дату и время разделило, объеденям снова
        //            {
        //                if (orderTime >= timeStarFilter && orderTime <= timeEndFilter) //проверяем попадает ли заказ в нужный временной интервал
        //                {
        //                    //если попадает создаем объект класса order и присваиваем ему свойства
        //                    Order order = new Order();
        //                    if (int.TryParse(lineSplit[0], out orderNumber))
        //                        order.number = orderNumber;
        //                    if (double.TryParse(lineSplit[1], out orderWeight))
        //                        order.weight = orderWeight;
        //
        //                    order.district = districtFilter;
        //                    order.orderTime = orderTime;
        //                    //складываем объекты в список
        //                    ordersListForWork.Insert(o, order);
        //                    o++;
        //                    Console.WriteLine(line);
        //                }
        //            }
        //            else
        //                continue;
        //        }
        //        else
        //            continue;
        //    }
        //    reader.Close();
        //}
        break;
    case 2:
        Console.WriteLine("Расчет");
        break;
    case 3:
        Console.WriteLine("Сохранение");
        break;
    case 4:
        Console.WriteLine("выход");
        break;
    default:
        Console.WriteLine("Отмена");
        return;
}





void ChooseDistrict(int value, out string districtName)
{
    switch (value)
    {
        case 1:
            districtName = "downTown";
            Console.WriteLine("Выбран район {0}", districtFilter);
            break;
        case 2:
            districtName = "centre";
            Console.WriteLine("Выбран район {0}", districtFilter);
            break;
        case 3:
            districtName = "suburbia";
            Console.WriteLine("Выбран район {0}", districtFilter);
            break;
        case 4:
            districtName = "industrial";
            Console.WriteLine("Выбран район {0}", districtFilter);
            break;
        default:
            districtName = null;
            Console.WriteLine("Отмена");
            return;
    }
}

void OrderExtractor (string fileNameAndPath,string districtName, DateTime start, DateTime end, ref List<Order> filteredOrders) 
{
    using (StreamReader reader = new StreamReader(fileNameAndPath))
    {
        string? line;
        int o = 0;
        while ((line = reader.ReadLine()) != null) //считываем строки из файла
        {
            string[] lineSplit = line.Split(' '); //делим одну строку на набор строк по словам
            if (lineSplit[2] == districtFilter)     //зная структуру файла с заказами, проверяем относится ли заказ к нужному району
            {
                string[] timeValue = new string[] { lineSplit[3], lineSplit[4] };
                string timeFromFile = string.Join(" ", timeValue);
                if (DateTime.TryParseExact(timeFromFile, "yyyy-MM-dd HH:mm:ss",
                    CultureInfo.CurrentCulture, DateTimeStyles.None, out orderTime)) //дату и время разделило, объеденям снова
                {
                    if (orderTime >= start && orderTime <= end) //проверяем попадает ли заказ в нужный временной интервал
                    {
                        //если попадает создаем объект класса order и присваиваем ему свойства
                        Order order = new Order();
                        if (int.TryParse(lineSplit[0], out orderNumber))
                            order.number = orderNumber;
                        if (double.TryParse(lineSplit[1], out orderWeight))
                            order.weight = orderWeight;

                        order.district = districtFilter;
                        order.orderTime = orderTime;
                        //складываем объекты в список
                        filteredOrders.Insert(o, order);
                        o++;
                        Console.WriteLine(line);
                    }
                }
                else
                    continue;
            }
            else
                continue;
        }
        reader.Close();
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
