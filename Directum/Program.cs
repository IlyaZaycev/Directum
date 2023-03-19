using System.ComponentModel;
using System.Net.Mime;
using System.Text;

namespace Directum
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var manager = new Manager();
            manager.GetMeetings();
            var bw = new BackgroundWorker();
            Console.WriteLine("Для получения справки введите 'help'");
            while (true)
            {
                switch (Console.ReadLine()!.ToLower().Trim())
                {
                    case "add":
                    {
                        Console.WriteLine(
                            "Введите данные о новой встрече следующим образом:\nНазвание встречи,дата начала встречи(1.01.2023 00:00:00),дата окончания встречи(1:01.2023 00:00:00), дату напоминания(1.01.2023 00:00:00).");
                        var str = Console.ReadLine()?.Trim();
                        if (string.IsNullOrWhiteSpace(str))
                        {
                            Console.WriteLine("Вы ничего не ввели");
                            break;
                        }

                        string?[] s = str.Split(',');
                        if (s.Length != 4)
                        {
                            Console.WriteLine(
                                "Вы ввели данные в неправильном виде, пожалуйста повторите операцию еще раз");
                            break;
                        }

                        if (!manager.CheckDate(s[1], s[2], s[3]))
                        {
                            Console.WriteLine("Вы ввели неверную дату");
                            break;
                        }

                        Console.WriteLine(manager.AddMeeting(s[0]!.Trim(), Convert.ToDateTime(s[1]!.Trim()),
                            Convert.ToDateTime(s[2]!.Trim()),
                            Convert.ToDateTime(s[3]!.Trim())));
                        
                        break;
                    }
                    case "change":
                    {
                        Console.WriteLine(manager.GetMeetings());
                        if (manager.GetMeetings().Equals("У вас не заплонированы встречи"))
                            break;
                        Console.WriteLine(
                            "Выберите элемент который вы хотите изменить(его номер), если хотите вернуться назад то введите 'back'");
                        var changeId = Console.ReadLine()?.Trim();
                        if (changeId == "back")
                            break;
                        Console.WriteLine(
                            "Введите новые данные о новой встрече следующим образом:\nНазвание встречи,дата начала встречи(1.01.2023 00:00:00),дата окончания встречи(1:01.2023 00:00:00), дату напоминания(1.01.2023 00:00:00)");
                        var str = Console.ReadLine()?.Trim();
                        if (string.IsNullOrWhiteSpace(str))
                        {
                            Console.WriteLine("Вы ничего не ввели");
                            break;
                        }

                        string?[] s = str.Split(',');
                        if (s.Length != 4)
                        {
                            Console.WriteLine(
                                "Вы ввели данные в неправильном виде, пожалуйста повторите операцию еще раз");
                            break;
                        }

                        if (manager.CheckDate(s[1], s[2], s[3]))
                            Console.WriteLine("Вы ввели неверную дату");
                        manager.ChangeMeeting(Convert.ToInt32(changeId) - 1, s[0]!.Trim(),
                            Convert.ToDateTime(s[1]!.Trim()),
                            Convert.ToDateTime(s[2]!.Trim()),
                            Convert.ToDateTime(s[3]!.Trim()));
                        break;
                    }
                    case "delete":
                        Console.WriteLine(manager.GetMeetings());
                        if (manager.GetMeetings().Equals("У вас не заплонированы встречи"))
                            break;
                        Console.WriteLine(
                            "Выберите элемент который вы хотите удалить(его номер), если хотите вернуться назад то введите 'back'");
                        var deleteId = Console.ReadLine().Trim();
                        if (deleteId == "back")
                            break;
                        Console.WriteLine(manager.RemoveMeeting(Convert.ToInt32(deleteId?.Trim())));
                        break;
                    case "export":
                        Console.WriteLine(manager.ExportToTXT());
                        break;
                    case "information":
                        Console.WriteLine("Для того чтобы посмотреть все расписание введите 'all' или введите день в следующем формате(1.01.2023). Если хотите вернуться назад то введите 'back'");
                        var info = Console.ReadLine().Trim();
                        if(info == "back")
                            break;
                        Console.WriteLine(info == "all" ? manager.GetMeetings() : manager.ScheduleForOneDay(info));
                        break;
                    case "help":
                        Console.WriteLine(manager.GetInfo());
                        break;
                }

                if (!bw.IsBusy)
                {
                    bw.DoWork += (_, _) => manager.MeetingAlert();
                    bw.RunWorkerAsync();
                }
            }
        }
    }
}