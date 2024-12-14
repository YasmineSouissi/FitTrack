using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Projet.Models;

namespace Projet.Controllers
{
    public class MembersController : Controller
    {
        private readonly string _connectionString = "Server=localhost;Port=3306;Database=Projet;User=root;Password=;";

        public IActionResult Index()

        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var user = GetUserById((int)userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var members_list = GetMembersFromDatabase();
            var members = new Members
            {
                Users = members_list,
                User= user
            };
            return View(members);
        }

        private User GetUserById(int userId)
        {
            User user = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT username, email, profile_image, role FROM user WHERE id_user = @UserId";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                Username = reader.GetString("username"),
                                Email = reader.GetString("email"),
                                ProfileImage = reader.IsDBNull(reader.GetOrdinal("profile_image")) ? null : reader.GetString("profile_image"),
                                Role = reader.GetString("role")

                            };
                        }
                    }
                }
            }

            return user;
        }
        private List<User> GetMembersFromDatabase()
        {
            var members = new List<User>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT  username, email, phone, role FROM user ";
                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        members.Add(new User
                        {
                            Username = reader.GetString("username"),
                            Email = reader.GetString("email"),
                            Phone = reader.GetString("phone"),
                            Role = reader.GetString("role")
                        });
                    }
                }
            }

            return members;
        }
    }
}
