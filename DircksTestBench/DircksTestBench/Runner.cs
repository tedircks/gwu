using System;
using System.Collections.Generic;
using System.Text;

namespace DircksTestBench
{
    public static class Runner
    {
        #region Preppers
        static List<string> FirstNames = new List<string>() { "John", "Bob", "Tom", "Eric", "Steven", "Maria", "Catherine", "Jane", "Lauren", "Julia" }; static List<string> GradeLevels = new List<string>() { "Freshman", "Sophomore", "Junior", "Senior", };
        struct Student
        {
            public string Name { get; set; }
            public float GPA { get; set; }
            public string GradeLevel { get; set; }
            public void Print() => Console.WriteLine($"Name: { Name } - GPA: { GPA } - Grade Level: { GradeLevel }"); 
        }

        class StudentV2
        {
            public string Name { get; set; }
            public float GPA { get; set; }
            public string GradeLevel { get; set; }
            public void Print() => Console.WriteLine($"Name: { Name } - GPA: { GPA } - Grade Level: { GradeLevel }");
        }
        #endregion

        /// <summary>
        /// This is fucking stupid. Create an array of student structs and print out their information.
        /// Using Random, generate random doubles to access non determinant GPAs and grade levels.
        /// </summary>
        public static void Go13()
        {
            Student[] students = new Student[10];
            // Random object to get random GradeLevels or tando create random GPAs        
            Random rand = new Random();
            for (int i = 0; i < 10; i++)
            {
                students[i] = new Student()
                {
                    Name = FirstNames[i],
                    // 4 = max, 1 = min                
                    GPA = float.Parse((rand.NextDouble() * (4 - 1) + 1).ToString()),
                    // random index of the GradeLevels list
                    GradeLevel = GradeLevels[rand.Next(0, 4)]
                };
            }

            foreach (var student in students) student.Print();
        }

        /// <summary>
        /// Literally the same fucking thing as Go13 with classes instead of structs. 
        /// This class is a joke. OO Code duplication FTW.
        /// </summary>
        public static void Go14()
        {
            StudentV2[] students = new StudentV2[10];
            // Random object to get random GradeLevels or tando create random GPAs        
            Random rand = new Random();
            for (int i = 0; i < 10; i++)
            {
                students[i] = new StudentV2()
                {
                    Name = FirstNames[i],
                    // 4 = max, 1 = min                
                    GPA = float.Parse((rand.NextDouble() * (4 - 1) + 1).ToString()),
                    // random index of the GradeLevels list
                    GradeLevel = GradeLevels[rand.Next(0, 4)]
                };
            }

            foreach (var student in students) student.Print();
        }
    }
}
