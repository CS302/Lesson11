using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkersLibrary
{


    public abstract class Worker : IComparable
    {
        private string name;
        private int age;
        private Int64 snn;

        public Int64 Snn
        {
            get { return snn; }
            set { snn = value; }
        }
        public static int count;
        protected double salary;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public int Age
        {
            get { return age; }
            set
            {
                if (value < 0)
                {
                    Console.WriteLine("Неверно задан возраст!");
                }
                else
                {
                    age = value;
                }
            }
        }

        abstract public int GetBonus();

        public virtual void Print()
        {
            Console.WriteLine("Имя: " + name);
            Console.WriteLine("Возраст: " + age);
            Console.WriteLine("ИНН: " + snn);
            Console.WriteLine("Премия: " + GetBonus());
        }

        public static void PrintWorkers(Worker[] workers)
        {
            for (int i = 0; i < workers.GetLength(0); i++)
            {
                workers[i].Print();
            }
        }

        public Worker(string name, int age, Int64 snn)
        {
            this.name = name;
            this.Age = age;
            this.snn = snn;
            count++;
            salary = 20000;
        }

        public Worker(string name, int age)
            : this(name, age, 0)
        {

        }
        static Worker()
        {
            count = 0;
        }
        public Worker()
        { }

        public int CompareTo(object obj)
        {
            if (this.GetBonus() < (obj as Worker).GetBonus())
            {
                return -1;
            }
            if (this.GetBonus() == (obj as Worker).GetBonus())
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }





}
