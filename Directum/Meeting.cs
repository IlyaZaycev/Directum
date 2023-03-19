using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualBasic;

namespace Directum
{
    internal class Meeting
    {
        private int _id;
        private string? _name = string.Empty;
        private DateTime _startMeeting;
        private DateTime _endMeeting;
        private DateTime _notification;

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public string? Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new Exception("Вы ничего не ввели");
                _name = value;
            }
        }

        public DateTime StartMeeting
        {
            get => _startMeeting;
            set
            {
                try
                {
                    _startMeeting = Convert.ToDateTime(value);
                }
                catch
                {
                    throw new Exception("Вы ничего не ввели");
                }
            }
        }

        public DateTime EndMeeting
        {
            get => _endMeeting;
            set
            {
                try
                {
                    _endMeeting = Convert.ToDateTime(value);
                }
                catch
                {
                    throw new Exception("Вы ничего не ввели");
                }
            }
        }

        public DateTime Notification
        {
            get => _notification;
            set
            {
                try
                {
                    _notification = Convert.ToDateTime(value);
                }
                catch
                {
                    throw new Exception("Вы ничего не ввели");
                }
            }
        }

        public Meeting(int id, string? name, DateTime startMeeting, DateTime endMeeting, DateTime notification)
        {
            Id = id;
            Name = name;
            StartMeeting = startMeeting;
            EndMeeting = endMeeting;
            Notification = notification;
        }
    }
}