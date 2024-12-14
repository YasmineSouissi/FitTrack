using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.IO;
using System;
using Microsoft.Extensions.Logging;

namespace Projet.Controllers
{
    public class CompleteProfileController : Controller
    {
        private readonly string _connectionString = "Server=localhost;Port=3306;Database=Projet;User=root;Password=;";
        private readonly ILogger<CompleteProfileController> _logger;

        // Injection de dépendance du logger
        public CompleteProfileController(ILogger<CompleteProfileController> logger)
        {
            _logger = logger;
        }

        // Action GET pour afficher la page de profil
        [HttpGet]
        public IActionResult CompleteProfile(int id)
        {
            ViewBag.UserId = id;  // Passe l'ID de l'utilisateur à la vue
            return View(); // Charge la vue CompleteProfile.cshtml
        }

        // Action POST pour traiter la soumission du profil
        [HttpPost]
        public IActionResult CompleteProfile(int id, float currentWeight, float goalWeight, float height, int dailyCaloriesGoal, string gender, IFormFile profileImage)
        {
            // Journalisation des informations avant de les enregistrer
            _logger.LogInformation("Données avant insertion dans la base de données:");
            _logger.LogInformation($"ID utilisateur: {id}");
            _logger.LogInformation($"Poids actuel: {currentWeight}");
            _logger.LogInformation($"Poids cible: {goalWeight}");
            _logger.LogInformation($"Taille: {height}");
            _logger.LogInformation($"Objectif calories quotidiennes: {dailyCaloriesGoal}");
            _logger.LogInformation($"Genre: {gender}");

            if (profileImage != null && profileImage.Length > 0)
            {
                // Sauvegarder l'image
                string imagePath = SaveProfileImage(profileImage);
                // Journalisation du chemin de l'image
                _logger.LogInformation($"Image de profil sauvegardée à: {imagePath}");

                // Mettre à jour les informations dans la base de données
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "UPDATE user SET current_weight = @CurrentWeight, goal_weight = @GoalWeight, height = @Height, daily_calories_goal = @DailyCaloriesGoal, gender = @Gender, profile_image = @ImagePath WHERE id_user = @UserId";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CurrentWeight", currentWeight);
                        command.Parameters.AddWithValue("@GoalWeight", goalWeight);
                        command.Parameters.AddWithValue("@Height", height);
                        command.Parameters.AddWithValue("@DailyCaloriesGoal", dailyCaloriesGoal);
                        command.Parameters.AddWithValue("@Gender", gender);
                        command.Parameters.AddWithValue("@ImagePath", imagePath);
                        command.Parameters.AddWithValue("@UserId", id);

                        int rowsAffected = command.ExecuteNonQuery();

                        // Journalisation du nombre de lignes affectées
                        _logger.LogInformation($"{rowsAffected} ligne(s) mise(s) à jour.");
                    }
                }
            }
            else
            {
                // Si aucun fichier n'est téléchargé, vous pouvez soit afficher un message d'erreur ou procéder sans image
                ViewBag.ErrorMessage = "Veuillez télécharger une image de profil.";
                return View(); // Retourne la vue avec un message d'erreur
            }

            // Rediriger vers la page de connexion après sauvegarde
            return RedirectToAction("Login", "Login");
        }

        // Méthode pour sauvegarder l'image de profil
        private string SaveProfileImage(IFormFile profileImage)
        {
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            // Crée le dossier si nécessaire
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + profileImage.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            // Sauvegarde l'image sur le disque
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                profileImage.CopyTo(fileStream);
            }

            return "/uploads/" + uniqueFileName; // Retourne le chemin relatif de l'image
        }
    }
}
