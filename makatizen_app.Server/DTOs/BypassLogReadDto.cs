namespace makatizen_app.Server.DTOs
{
    public class BypassLogReadDto
    {
        public int Id { get; set; }
        public int PersonID { get; set; }
        public string StepName { get; set; }
        public string ReasonCode { get; set; }
        public string ReasonDetails { get; set; }
        public DateTime DateBypassed { get; set; }
    }
}