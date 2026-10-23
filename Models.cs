using System.ComponentModel.DataAnnotations;

namespace OlympiadProject
{
    public class Student
    {
        [Key] public int Id { get; set; }
        public string FullName { get; set; }
        public string GroupName { get; set; }
        public string Faculty { get; set; }
        public string Email { get; set; }
    }

    public class Subject
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
        public string Teacher { get; set; }
    }

    public class Olympiad
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
        public string Level { get; set; }
        public double PassingScore { get; set; }
    }

    public class OlympiadSubject
    {
        [Key] public int Id { get; set; }
        public int OlympiadId { get; set; }
        public int SubjectId { get; set; }
        public bool IsProfile { get; set; }

        public virtual Olympiad Olympiad { get; set; }
        public virtual Subject Subject { get; set; }
    }

    public class Grade
    {
        [Key] public int Id { get; set; }
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public int Score { get; set; }
        public string Date { get; set; }

        public virtual Student Student { get; set; }
        public virtual Subject Subject { get; set; }
    }
}