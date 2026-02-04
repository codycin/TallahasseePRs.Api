namespace TallahasseePRs.Api.Models
{
    public class Lift
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Category { get; set; } = "Power";
        public string DefaultUnit { get; set; } = "lb";
    }
}
