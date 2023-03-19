using System.Text;

namespace Directum
{
    internal class Manager
    {
        private List<Meeting> _meetings = new();

        public string GetMeetings()
        {
            if (_meetings.Count == 0) return "У вас не заплонированы встречи";
            var output = new StringBuilder();
            _meetings = _meetings.OrderBy(meet => meet.StartMeeting).ToList();
            DateTime checkedMonth = default; //для сравнения 
            var indexOfNextMeeting = 0;
            if (_meetings.Count > 0)
                checkedMonth = _meetings.FirstOrDefault()!.StartMeeting.Date;

            for (var month = 0; month <= 12; month++)
            {
                if (month != checkedMonth.Month) continue;
                var matchMeeting = 0;
                if (_meetings.Count > 0)
                    matchMeeting = _meetings.Find(date => Convert.ToInt32(date.StartMeeting.Month) == month)!.StartMeeting
                        .Month;
                if (month == matchMeeting)
                {
                    output.Append($"{checkedMonth.ToString("Y")}\n");

                    foreach (var meeting in _meetings.Where(meeting => meeting.StartMeeting.Date.Month == month))
                    {
                        output.Append(
                            $"Название встречи:{meeting.Name}, Дата начала встречи:{meeting.StartMeeting.TimeOfDay}, Дата окончания встречи:{meeting.EndMeeting.TimeOfDay}\n");
                        indexOfNextMeeting = meeting.Id;
                    }

                    if (_meetings.Count <= 1) continue;
                    if (indexOfNextMeeting + 1 < _meetings.Count) continue;
                    indexOfNextMeeting = _meetings.Count;
                    checkedMonth = _meetings[indexOfNextMeeting - 1].StartMeeting.Date;
                }
            }

            return output.ToString();
        }

        public string ScheduleForOneDay(string day)
        {
            var sb = new StringBuilder();
            if (day.Length== 0) return "Вы ничего не ввели";
            if (!DateTime.TryParse(day, out _)) return "Введите дату в нужном формате";
            var meetingsList = _meetings.Where(meet => meet.StartMeeting.Date == Convert.ToDateTime(day)).ToList();
            if (meetingsList.Count == 0) return "У вас не заплонированы встречи";
            foreach (var meeting in meetingsList)
            {
                sb.Append(
                    $"Название встречи:{meeting.Name}, Дата начала встречи:{meeting.StartMeeting.TimeOfDay}, Дата окончания встречи:{meeting.EndMeeting.TimeOfDay}\n");
            }
            return sb.ToString();
        }
        public string AddMeeting(string? name, DateTime startMeeting, DateTime endMeeting,
            DateTime notification)
        {
            if (startMeeting < DateTime.Now)
                return "Вы не можете заплонировать новое мероприятие на прошедшее время";
            if (notification < DateTime.Now)
                return $"Пожалуйста выберите другое время для оповещения";
            var listTodaysMeetings = _meetings.Where(meeting => meeting.StartMeeting.Day == startMeeting.Day).ToList();
            if (listTodaysMeetings.Any(meeting =>
                    meeting.StartMeeting >= startMeeting && meeting.StartMeeting <= endMeeting))
                return "На данное время запланировано другое мероприятие";
            _meetings.Add(new Meeting(_meetings.Count + 1, name, startMeeting, endMeeting, notification));
            return "Встреча заплонирована.";
        }

        public string RemoveMeeting(int id)
        {
            _meetings.RemoveAt(id - 1);
            return "Встреча удалена.";
        }

        public string ChangeMeeting(int id, string? name, DateTime startMeeting, DateTime endMeeting,
            DateTime notification)
        {
            if (_meetings.Count <= 0)
                return "У вас не запланированы никакие мероприятия";
            if (_meetings.FindIndex(x => x.Id == id) != id)
                return "Вы ошбились в написании индекса или такой записи не существует";
            _meetings[id].Name = name!;
            _meetings[id].StartMeeting = startMeeting;
            _meetings[id].EndMeeting = endMeeting;
            _meetings[id].Notification = notification;
            return "Данные о встрече изменены.";
        }

        public string GetInfo()
        {
            return
                $"Комманды:help - вывод всех комманд\nadd - добавление новой записи\nchange - изменение записи\ndelete - удаление записи\nexport - экспортировать мероприятия в текстовый документ\ninformation - просмотр всех записей\n";
        }

        public string ExportToTXT()
        {
            var path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\Meetings.txt";
            using var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            {
                var sr = new StreamReader(fs);
                using var sw = new StreamWriter(fs);
                {
                    sr.ReadToEnd();
                    fs.SetLength(0);
                    sw.Write(GetMeetings());
                }
            }
            return $"Экспорт файлов завершен по следующему пути: {path}.";
        }

        public bool CheckDate(string startMeeting, string endMeeting,
            string notification)
        {
            return DateTime.TryParse(startMeeting, out _) && DateTime.TryParse(endMeeting, out _) &&
                   DateTime.TryParse(notification, out _);
        }
        public async void MeetingAlert()
        {
            if (_meetings.Count <= 0) return;
            var meetings = GetSortedListByNotification();
            while (true)
            {
                var meet = meetings.FirstOrDefault()!.Notification;
                var time = DateTime.Now;
                if (time == meet)
                    Console.WriteLine($"Скоро начнется следующая встреча");
            }
        }

        private List<Meeting> GetSortedListByNotification()
        {
            return _meetings.OrderBy(meet => meet.Notification).ToList();
        }
    }
}