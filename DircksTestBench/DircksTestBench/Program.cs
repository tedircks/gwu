using System;
using System.Collections.Generic;
class Program
{
    static List<string> FirstNames = new List<string>() { "John", "Bob", "Tom", "Eric", "Steven", "Maria", "Catherine", "Jane", "Lauren", "Julia" }; static List<string> GradeLevels = new List<string>() { "Freshman", "Sophomore", "Junior", "Senior", };
    class Student
    {
        public string Name { get; set; }
        public float GPA { get; set; }
        public string GradeLevel { get; set; }
        public void Print() => Console.WriteLine($"Name: { Name } - GPA: { GPA } - Grade Level: { GradeLevel }");

    }
    static void Main(string[] args)
    {
        // array of Student structs        
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
}