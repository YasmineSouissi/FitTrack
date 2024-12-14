namespace Projet.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }  // Assurez-vous que cette propriété existe
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public float CurrentWeight { get; set; }
        public float GoalWeight { get; set; }
        public int CoachId { get; set; }
        public float Height { get; set; }
        public int DailyCaloriesGoal { get; set; }
        public string Gender { get; set; }
        public string? ProfileImage { get; internal set; }
    }

}
