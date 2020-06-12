using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MobileGameTest.Education;

namespace MobileGameTest.Data
{
    public class EducationData
    {
        private static EducationData instance;
        public static EducationData Instance
        {
            get
            {
                if (instance == null)
                    instance = new EducationData();
                return instance;
            }
        }

        private EducationData()
        {
            Load();
        }

        private void Load()
        {
            Levels = new List<EducationalLevel>();
            var lvl1 = new EducationalLevel()
            {
                name = "Уровень 1: Натуральные числа",
                theory = "Натуральные числа - числа 1,2,3,4,5 и т.д., используе-мые при счете.\nПервым числом натурального ряда является число 1, а последнего числа - нет, так как это бесконечный ряд чисел.\nКаждое следующее число больше предыдущего на единицу. Все натуральные числа записываются с помощью специальных знаков, которые называются цифрами.\nТаких знаков десять: 0,1,2,3,4,5,6,7,8,9. В зависимости от занимаемого места в числе каждая цифра может иметь разное значение,\nнапример: цифра 5 в числе 859 обозначает число пятьдесят, в числе 529 - пятьсот, а в числе 5078 - пять тысяч.\nМесто, занимаемое цифрой в записи числа, называется разрядом. Если считать справа налево,\nто первое место в записи числа называют разрядом единиц. Второе - разрядом десятков.\nТретье - разрядом сотен и т.д. Например: 6 345 - пять единиц,  четыре десятка,\nтри сотни и 6 тысяч. Такая запись называется десятичной.\nДесять единиц каждого разряда составляют одну единицу следующего разряда.",
                TheoryTasks = new List<Task>()
                {
                    new Task()
                    {
                        description = "theory task 1",
                        answer = "1",
                        isGenerated = false,
                        type = TaskType.Theory,
                        points = 30
                    },
                    new Task()
                    {
                        description = "theory task 2",
                        answer = "1",
                        isGenerated = false,
                        type = TaskType.Theory
                    },
                    new Task()
                    {
                        description = "theory task 3",
                        answer = "1",
                        isGenerated = false,
                        type = TaskType.Theory
                    },
                    new Task()
                    {
                        description = "theory task 4",
                        answer = "1",
                        isGenerated = false,
                        type = TaskType.Theory
                    },
                    new Task()
                    {
                        description = "theory task 5",
                        answer = "1",
                        isGenerated = false,
                        type = TaskType.Theory
                    }
                },
                MathTasks = new List<Task>()
                {
                    Task.Generate(GeneratedTaskType.NumberTask),
                    Task.Generate(GeneratedTaskType.MovementSimple)
                },
                CalculationTasks = new List<Task>()
                {
                   Task.Generate(GeneratedTaskType.Sum),
                   new Task()
                   {
                       description = "calc task 2",
                       answer = "1",
                       isGenerated = false,
                       type = TaskType.Calc
                   },
                   new Task()
                   {
                       description = "calc task 3",
                       answer = "1",
                       isGenerated = false,
                       type = TaskType.Calc
                   }
                }
            };
            Levels.Add(lvl1);
        }

        public List<EducationalLevel> Levels;
        public int currentLevel = 0;

        public EducationalLevel CurrentLevel { get { return Levels[currentLevel]; } }
    }
}