using Bogus;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace BirthdayAttack
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AppDbContext db = new AppDbContext();
            //db.Users.AddRange(GetUsers());
            //db.SaveChanges();
            //foreach (var item in db.Users)
            //{
            //    Console.WriteLine($"{item.Login}, {item.Password}");
            //}

            Random random = new Random();
            int attempts = 0;
            bool found = false;

            while (!found)
            {
                // Генерация случайной строки
                string randomString = GenerateRandomString(random, 12);

                // Вычисление MD5 хэша
                string hash = ComputeMd5Hash(randomString);

                Console.WriteLine("Пароль: " + randomString);
                Console.WriteLine("Хэш: " + hash);
                Console.WriteLine("__________________");
                attempts++;
                
                // Проверка, совпадает ли хэш с целевым
                var u = db.Users.FirstOrDefault(x => x.Password == hash);
                if (u != null)
                {
                    Console.WriteLine("Совпадение найдено!");
                    Console.WriteLine("Логин: " + u.Login);
                    Console.WriteLine("Пароль: " + randomString);
                    Console.WriteLine("Хэш: " + hash);
                    Console.WriteLine("Количество попыток: " + attempts);
                    found = true;
                }
            }
        }
        static string GenerateRandomString(Random random, int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.!#№$";
            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }
            return result.ToString();
        }

        // Метод для вычисления MD5 хэша
        static string ComputeMd5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Конвертация байтов в строку
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        static User[] GetUsers()
        {
            Faker faker = new Faker();
            var fakeUsers = new Faker<User>()
                .RuleFor(u => u.Id, f => f.IndexFaker)
                .RuleFor(u => u.Login, f => f.Internet.UserName())
                .RuleFor(u => u.Password, f => f.Internet.Password());
            fakeUsers.Generate();
            var users = fakeUsers.Generate(1000000);

            foreach (var user in users)
            {
                user.Password = ComputeMd5Hash(user.Password);
            }

            return users.ToArray();
        }
    }
}
