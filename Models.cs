using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OlympiadProject
{
    public class Student
    {
        [Key] public int Id { get; set; }
        public string FullName { get; set; }
        public string GroupName { get; set; }
    }

    public class Subject
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
        public bool IsProfile { get; set; }
    }

    public class Grade
    {
        [Key] public int Id { get; set; }
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public int Score { get; set; }

        // Навигационные свойства для удобной работы
        public virtual Student Student { get; set; }
        public virtual Subject Subject { get; set; }
    }
}