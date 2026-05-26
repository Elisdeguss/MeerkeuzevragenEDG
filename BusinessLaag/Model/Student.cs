namespace BusinessLaag.Model
{
    public class Student
    {
        public Student(int id, string naam)
        {
            Id = id;
            Naam = naam;
            AlleResultaten = new List<TestResultaat>();
        }

        public int Id { get; set; }
        public string Naam { get; set; }


        public List<TestResultaat> AlleResultaten { get; set; }


        


    }
}
