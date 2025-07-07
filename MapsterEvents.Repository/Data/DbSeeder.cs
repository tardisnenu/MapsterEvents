using MapsterEvents.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MapsterEvents.Repository.Data
{
    /// <summary>
    /// Veritabanı seed data sınıfı - Enterprise grade
    /// </summary>
    public static class DbSeeder
    {
        private static readonly IPasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        /// <summary>
        /// Seed data oluşturur
        /// </summary>
        /// <param name="context">Veritabanı context</param>
        /// <param name="logger">Logger (opsiyonel)</param>
        /// <returns>Task</returns>
        public static async Task SeedAsync(MapsterEventsDbContext context, ILogger? logger = null)
        {
            try
            {
                // Migration-based database creation (Production ready)
                await context.Database.MigrateAsync();
                logger?.LogInformation("Database migration completed successfully");

                // Eğer veri varsa seed işlemi yapma, ancak ImageUrl'leri güncelle
                if (await context.Categories.AnyAsync())
                {
                    logger?.LogInformation("Seed data already exists, updating ImageUrls for existing events");
                    await UpdateEventImageUrls(context, logger);
                    return;
                }

                logger?.LogInformation("Starting database seeding...");

            // Kategoriler
            var categories = new List<Category>
            {
                new Category
                {
                    Name = "Teknoloji",
                    Description = "Teknoloji ve yazılım geliştirme etkinlikleri"
                },
                new Category
                {
                    Name = "Eğitim",
                    Description = "Eğitim ve kişisel gelişim etkinlikleri"
                },
                new Category
                {
                    Name = "Spor",
                    Description = "Spor ve fiziksel aktivite etkinlikleri"
                },
                new Category
                {
                    Name = "Sanat",
                    Description = "Sanat ve kültür etkinlikleri"
                },
                new Category
                {
                    Name = "İş & Kariyer",
                    Description = "İş dünyası ve kariyer geliştirme etkinlikleri"
                }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            // Test kullanıcıları - Güvenli şifre hash'leme ile
            var users = new List<User>
            {
                CreateSecureUser("Admin User", "admin@mapsterevents.com", "Admin123!"),
                CreateSecureUser("Test Kullanıcısı", "test@example.com", "Test123!"),
                CreateSecureUser("Ahmet Yılmaz", "ahmet@example.com", "Ahmet123!")
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

            // Örnek etkinlikler
            var events = new List<Event>
            {
                new Event
                {
                    Title = ".NET 8 ve Modern Web Geliştirme",
                    Description = "ASP.NET Core 8.0 ile modern web uygulaması geliştirme teknikleri üzerine kapsamlı bir workshop. Entity Framework Core, SignalR ve Blazor konularını kapsayacaktır.",
                    Date = DateTime.UtcNow.AddDays(7),
                    Location = "İstanbul Teknik Üniversitesi Konferans Salonu",
                    CategoryId = 1, // Teknoloji
                    OrganizerId = 1, // Admin User
                    ImageUrl = "/images/NET 8 ve Modern Web.png"
                },
                new Event
                {
                    Title = "Angular ve TypeScript Masterclass",
                    Description = "Angular framework'ü ile profesyonel frontend geliştirme. RxJS, NgRx ve Angular Material konularını içeren ileri seviye eğitim.",
                    Date = DateTime.UtcNow.AddDays(14),
                    Location = "Boğaziçi Üniversitesi Güney Kampüs",
                    CategoryId = 1, // Teknoloji
                    OrganizerId = 2, // Test Kullanıcısı
                    ImageUrl = "/images/Angular ve TypeScript.png"
                },
                new Event
                {
                    Title = "Yazılım Geliştirmede SOLID Prensipleri",
                    Description = "Temiz kod yazma teknikleri ve SOLID prensiplerinin pratikte uygulanması. Design patterns ve refactoring teknikleri dahil.",
                    Date = DateTime.UtcNow.AddDays(21),
                    Location = "ODTÜ Teknokent Konferans Salonu",
                    CategoryId = 2, // Eğitim
                    OrganizerId = 3, // Ahmet Yılmaz
                    ImageUrl = "/images/Yazılım Geliştirmede SOLID.png"
                },
                new Event
                {
                    Title = "Startup Dünyasına Giriş",
                    Description = "Girişimcilik ekosistemi, iş modeli geliştirme ve yatırım alma süreçleri hakkında kapsamlı bir seminer.",
                    Date = DateTime.UtcNow.AddDays(10),
                    Location = "İTÜ Çekirdek İnovasyon Merkezi",
                    CategoryId = 5, // İş & Kariyer
                    OrganizerId = 1, // Admin User
                    ImageUrl = "/images/Startup Dünyasına.png"
                },
                new Event
                {
                    Title = "Açık Kaynak Kodlu Yazılım Geliştirme",
                    Description = "GitHub, GitLab kullanımı ve açık kaynak projelere katkıda bulunma yöntemleri. Open source ekosisteminde nasıl yer alınır?",
                    Date = DateTime.UtcNow.AddDays(5),
                    Location = "Hacettepe Üniversitesi Bilgisayar Mühendisliği",
                    CategoryId = 1, // Teknoloji
                    OrganizerId = 2, // Test Kullanıcısı
                    ImageUrl = "/images/Açık Kaynak Kodlu.png"
                },
                new Event
                {
                    Title = "Proje Yönetimi ve Agile Metodolojiler",
                    Description = "Scrum, Kanban ve diğer agile metodolojiler ile etkili proje yönetimi teknikleri. Sertifikalı eğitim programı.",
                    Date = DateTime.UtcNow.AddDays(28),
                    Location = "Sabancı Üniversitesi Minerva Sarayı",
                    CategoryId = 5, // İş & Kariyer
                    OrganizerId = 3, // Ahmet Yılmaz
                    ImageUrl = "/images/Proje Yönetimi ve Agile.png"
                }
            };

            await context.Events.AddRangeAsync(events);
            await context.SaveChangesAsync();

            // Örnek kayıtlar
            var registrations = new List<Registration>
            {
                new Registration
                {
                    EventId = 1,
                    UserId = 2,
                    RegistrationDate = DateTime.UtcNow.AddDays(-1)
                },
                new Registration
                {
                    EventId = 1,
                    UserId = 3,
                    RegistrationDate = DateTime.UtcNow.AddDays(-2)
                },
                new Registration
                {
                    EventId = 2,
                    UserId = 1,
                    RegistrationDate = DateTime.UtcNow.AddDays(-1)
                },
                new Registration
                {
                    EventId = 3,
                    UserId = 1,
                    RegistrationDate = DateTime.UtcNow.AddDays(-3)
                },
                new Registration
                {
                    EventId = 3,
                    UserId = 2,
                    RegistrationDate = DateTime.UtcNow.AddDays(-2)
                }
            };

            await context.Registrations.AddRangeAsync(registrations);
            await context.SaveChangesAsync();
            
            logger?.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }

        /// <summary>
        /// Güvenli kullanıcı oluşturmak için yardımcı metod
        /// </summary>
        /// <param name="fullName">Kullanıcı adı</param>
        /// <param name="email">E-posta</param>
        /// <param name="password">Şifre</param>
        /// <returns>Güvenli kullanıcı</returns>
        private static User CreateSecureUser(string fullName, string email, string password)
        {
            var user = new User
            {
                FullName = fullName,
                Email = email
            };

            // ASP.NET Core Identity PasswordHasher kullanarak güvenli hash
            user.PasswordHash = _passwordHasher.HashPassword(user, password);
            
            // Modern approach: Salt artık hash içinde gömülü, ayrı salt field'ı kullanılmıyor
            user.PasswordSalt = string.Empty;
            
            return user;
        }

        /// <summary>
        /// Mevcut etkinliklerin ImageUrl değerlerini günceller
        /// </summary>
        /// <param name="context">Veritabanı context</param>
        /// <param name="logger">Logger (opsiyonel)</param>
        /// <returns>Task</returns>
        private static async Task UpdateEventImageUrls(MapsterEventsDbContext context, ILogger? logger = null)
        {
            try
            {
                var events = await context.Events.ToListAsync();
                
                foreach (var eventItem in events)
                {
                    if (string.IsNullOrEmpty(eventItem.ImageUrl))
                    {
                        // Etkinlik başlığına göre doğru resmi ata
                        switch (eventItem.Title)
                        {
                            case ".NET 8 ve Modern Web Geliştirme":
                                eventItem.ImageUrl = "/images/NET 8 ve Modern Web.png";
                                break;
                            case "Angular ve TypeScript Masterclass":
                                eventItem.ImageUrl = "/images/Angular ve TypeScript.png";
                                break;
                            case "Yazılım Geliştirmede SOLID Prensipleri":
                                eventItem.ImageUrl = "/images/Yazılım Geliştirmede SOLID.png";
                                break;
                            case "Startup Dünyasına Giriş":
                                eventItem.ImageUrl = "/images/Startup Dünyasına.png";
                                break;
                            case "Açık Kaynak Kodlu Yazılım Geliştirme":
                                eventItem.ImageUrl = "/images/Açık Kaynak Kodlu.png";
                                break;
                            case "Proje Yönetimi ve Agile Metodolojiler":
                                eventItem.ImageUrl = "/images/Proje Yönetimi ve Agile.png";
                                break;
                            default:
                                eventItem.ImageUrl = "/images/NET 8 ve Modern Web.png"; // Default fallback
                                break;
                        }
                        
                        logger?.LogInformation($"Updated ImageUrl for event: {eventItem.Title}");
                    }
                }
                
                await context.SaveChangesAsync();
                logger?.LogInformation("Event ImageUrls updated successfully");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error updating event ImageUrls");
                throw;
            }
        }
    }
}