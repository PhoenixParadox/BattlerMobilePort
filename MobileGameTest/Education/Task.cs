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

namespace MobileGameTest.Education
{
    public enum GeneratedTaskType
    {
        None,
        Sum,
        MovementSimple,
        NumberTask
    }

    public enum TaskType
    {
        Theory,
        Math,
        Calc
    }

    public class Task
    {
        public string description;
        public string answer;
        public TaskType type;
        public bool isGenerated;
        public int points;
        public bool isSolved;

        public GeneratedTaskType generatedType;
        public Func<int, int, int, int, int> solution;
        public int[] parameters;


        public static Random rnd = new Random();
        public static Task Generate(GeneratedTaskType type)
        {
            Task res = null;
            switch (type)
            {
                case (GeneratedTaskType.Sum):
                    res = GenerateSumTask();
                    break;
                case (GeneratedTaskType.MovementSimple):
                    res = GenerateSimpleMovementTask();
                    break;
                case (GeneratedTaskType.NumberTask):
                    res = GenerateNumberTask();
                    break;
            }
            return res;
        }

        #region check generated tasks
        public static bool CheckGeneratedTask(Task task, string answer)
        {
            switch (task.generatedType)
            {
                case (GeneratedTaskType.Sum):
                    return CheckSumTask(task, answer);
                    break;
            }
            return false;
        }

        private static bool CheckSumTask(Task task, string answer)
        {
            var res = task.solution(task.parameters[0], task.parameters[1], task.parameters[2], task.parameters[3]);
            try
            {
                return res == Int32.Parse(answer);
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region generate tasks
        private static Task GenerateSumTask()
        {
            var task = new Task();
            task.parameters = new int[4] { rnd.Next(1, 401), rnd.Next(1, 51), rnd.Next(1, 101), rnd.Next(1, 201) };
            if (rnd.Next(1, 11) > 5)
            {
                task.description = "Посчитайте выражение и найдите сумму\nдесятков и сотен результата\n" + $"{task.parameters[0]} + {task.parameters[1]} - {task.parameters[2]} + {task.parameters[3]}";
                task.solution = (p1, p2, p3, p4) =>
                {
                    var res = Math.Abs(p1 + p2 - p3 + p4);
                    return res / 100 + (res / 10) % 10;

                };
            }
            else
            {
                task.description = "Посчитайте выражение и найдите сумму\nдесятков и единиц результата\n" + $"{task.parameters[0]} - {task.parameters[1]} + {task.parameters[2]} - {task.parameters[3]}";
                task.solution = (p1, p2, p3, p4) =>
                {
                    var res = Math.Abs(p1 - p2 + p3 - p4);
                    return res % 10 + (res / 10) % 10;
                };
            }
            task.isGenerated = true;
            task.points = 50;
            task.type = TaskType.Calc;
            task.generatedType = GeneratedTaskType.Sum;
            task.answer = task.solution(task.parameters[0], task.parameters[1], task.parameters[2], task.parameters[3]).ToString();
            return task;
        }

        private static Task GenerateSimpleMovementTask()
        {
            var task = new Task();
            // speed, direction, position, time
            task.parameters = new int[4] { rnd.Next(1, 10), rnd.Next(1, 3), rnd.Next(1, 100), rnd.Next(1, 10) };
            if (task.parameters[0] * task.parameters[3] > task.parameters[2])
            {
                task.description = $"Турист идет по шоссе со скоростью {task.parameters[0]} км/ч.\n(Движется в сторону увеличения км на столбах)\nСейчас он находится у километрового столба\nс отметкой {task.parameters[2]} км.\nУ столба с какой отметкой он будет через {task.parameters[3]} ч ? ";
                task.solution = (p1, p2, p3, p4) =>
                {
                    return p1 * p4 + p3;
                };
            }
            else
            {
                if (task.parameters[1] > 1)
                {
                    task.description = $"Турист идет по шоссе со скоростью {task.parameters[0]} км/ч.\n(Движется в сторону увеличения км на столбах)\nСейчас он находится у километрового столба\nс отметкой {task.parameters[2]} км.\nУ столба с какой отметкой он будет через {task.parameters[3]} ч ? ";
                }
                else
                {
                    task.description = $"Турист идет по шоссе со скоростью {task.parameters[0]} км/ч.\n(Движется в сторону уменьшения км на столбах)\nСейчас он находится у километрового столба\nс отметкой {task.parameters[2]} км.\nУ столба с какой отметкой он будет через {task.parameters[3]} ч ? ";
                }
                task.solution = (p1, p2, p3, p4) =>
                {
                    return (p2 > 1) ? p1 * p4 + p3 : p3 - p1 * p4;
                };
            }
            task.isGenerated = true;
            task.points = 70;
            task.type = TaskType.Math;
            task.generatedType = GeneratedTaskType.MovementSimple;
            task.answer = task.solution(task.parameters[0], task.parameters[1], task.parameters[2], task.parameters[3]).ToString();
            return task;
        }

        private static Task GenerateNumberTask()
        {
            var task = new Task();
            task.parameters = new int[4] { rnd.Next(100000, 999999), rnd.Next(1, 5), rnd.Next(1, 11), rnd.Next(1, 11) };
            task.description = $"Охарактеризовать число {task.parameters[0]} по разрядам\nи записать ";
            switch (task.parameters[1])
            {
                case (1):
                    task.description += "сумму старшего и\nмладшего разрядов.";
                    task.solution = (p1, p2, p3, p4) => p1 % 10 + p1 / 100000;
                    break;
                case (2):
                    task.description += "сумму сотен и сотен тысяч.";
                    task.solution = (p1, p2, p3, p4) => (p1 / 100) % 10 + (p1 / 100000) % 10;
                    break;
                case (3):
                    task.description += "произведение второго и\nчетвертого разрядов.";
                    task.solution = (p1, p2, p3, p4) => ((p1 / 10) % 10) * ((p1 / 1000) % 10);
                    break;
                case (4):
                    task.description += "произведение третьего и\nстаршего разряда";
                    task.solution = (p1, p2, p3, p4) => (p1 / 100000) * ((p1 / 100) % 10);
                    break;                        
            }
            task.isGenerated = true;
            task.points = 70;
            task.type = TaskType.Math;
            task.generatedType = GeneratedTaskType.NumberTask;
            task.answer = task.solution(task.parameters[0], task.parameters[1], task.parameters[2], task.parameters[3]).ToString();
            return task;
        }
        #endregion
    }
}