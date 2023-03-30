using HomeBar.Models.Dtos;
using System.Text;
using System.Text.Json;

public class Program
{
    private const string apiUrl = "https://localhost:7182";
    private const string registerEntryPoint = "/api/auth/register";
    private const string loginEntryPoint = "/api/auth/login";
    private const string logoutEntryPoint = "/api/auth/logout";

    private static readonly HttpClient httpClient = new HttpClient();

    public static async Task Main(string[] args)
    {
        string token = null;

        try
        {
            while (true)
            {
                MainMenu();
                var opt = Console.ReadLine();

                switch (Int16.Parse(opt))
                {
                    case 1:
                        await RegisterAsync();
                        break;
                    case 2:
                        token = await LoginAsync();
                        break;
                    case 3:
                        await LogoutAsync(token);
                        token = null;
                        break;
                    case 4:
                        Exit();
                        return;
                    default:
                        InvalidOption();
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static void InvalidOption()
    {
        Console.WriteLine("Invalid option selected.");
    }

    // Displays the main menu options
    private static void MainMenu()
    {
        Console.WriteLine("Welcome to Home Bar App!");
        Console.WriteLine("Please select an action:");
        Console.WriteLine();
        Console.WriteLine("1. Register a new user");
        Console.WriteLine("2. Login");
        Console.WriteLine("3. Logout");
        Console.WriteLine("4. Exit");
    }

    // Reads a password from the console without displaying it
    private static string ReadPassword()
    {
        string password = "";
        ConsoleKeyInfo key;
        DateTime lastKeyTime = DateTime.Now;

        do
        {
            key = Console.ReadKey(true);

            // If the key is not a special key
            if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
            {
                password += key.KeyChar;
                Console.Write("*");
                lastKeyTime = DateTime.Now;
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password.Substring(0, password.Length - 1);
                Console.Write("\b \b");
            }
            else if ((DateTime.Now - lastKeyTime).TotalSeconds >= 1)
            {
                Console.Write("*");
            }
        }
        while (key.Key != ConsoleKey.Enter);

        Console.WriteLine();

        return password;
    }

    // Registers a new user via the API
    private static async Task RegisterAsync()
    {
        Console.Write("Please enter your name: ");
        string name = Console.ReadLine();
        Console.Write("Please enter your email: ");
        string email = Console.ReadLine();
        Console.Write("Please enter your password: ");
        string password = ReadPassword();

        var registerDto = new RegisterDto
        {
            Name = name,
            Email = email,
            Password = password
        };

        var json = JsonSerializer.Serialize(registerDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(apiUrl + registerEntryPoint, content);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Registration successful");
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Registration failed: {error}");
        }
    }

    // Logs in a user via the API
    public static async Task<string> LoginAsync()
    {
        Console.Write("Please enter your email: ");
        string email = Console.ReadLine();
        Console.Write("Please enter your password: ");
        string password = ReadPassword();

        var loginDto = new LoginDto
        {
            Email = email,
            Password = password
        };

        var json = JsonSerializer.Serialize(loginDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(apiUrl + loginEntryPoint, content);

        if (response.IsSuccessStatusCode)
        {
            var token = response.Headers.GetValues("Authorization").FirstOrDefault()?.Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("Invalid token received");
            }
            Console.WriteLine("\nLogin successful\n\n");
            return token;
        }
        else
        {
            Console.WriteLine("Login failed. Please check your email and password.");
            return null;
        }
    }

    public static async Task LogoutAsync(string? token)
    {
        if (token != null)
        {
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await httpClient.PostAsync(apiUrl + logoutEntryPoint, null);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("\nLogout successful\n\n");
            }
            else
            {
                Console.WriteLine("\nLogout failed\n\n");
            }
        }
        else
        {
            Console.WriteLine("You need to login first.");
        }
    }

    private static void Exit()
    {
        Console.WriteLine("Goodbye");
    }
}