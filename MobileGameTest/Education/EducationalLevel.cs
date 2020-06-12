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
    public class EducationalLevel
    {
        public string name;
        public string theory;

        public List<Task> TheoryTasks;
        public List<Task> MathTasks;
        public List<Task> CalculationTasks;

        public static Random rnd = new Random();

        public EducationalLevel()
        {
        }

        public static Task GenerateSumTask()
        {
            var task = new Task();


            return task;
        }
    }
}