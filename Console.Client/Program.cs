namespace ConsoleClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync("https://localhost:7091/api/students");

            Console.WriteLine(response);
            //if (response.IsSuccessStatusCode)
            //{
            //    var studentsJson = await response.Content.ReadAsStringAsync();
            //    Console.WriteLine("Students: " + studentsJson);
            //}
            //else
            //{
            //    Console.WriteLine("Error: " + response.StatusCode);
            //}
        }
    }
}
