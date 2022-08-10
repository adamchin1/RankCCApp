using Microsoft.AspNetCore.Mvc;


namespace RankCCApp.Controllers
{
    public class ReadWriteController : Controller
    {
        public string[] Read(string textFile)
        {
            string path = ".\\Data\\" + textFile;
            string[] lines = System.IO.File.ReadAllLines(path);
            return lines;
        }
        public void Write(string textFile, string input)
        {
            string path = ".\\Data\\" + textFile;
            using StreamWriter file = new StreamWriter(path, append: true);
            file.WriteLineAsync(input);
        }
        public void Delete(string textFile, string input)
        {
            string path = ".\\Data\\" + textFile;
            string[] lines = System.IO.File.ReadAllLines(path);
            string[] values;
            for (int i = 0; i < lines.Length; i++)
            {
                values = lines[i].Split("||");
                if (input == values[0].ToUpper())
                    lines = lines.Except(new string[] { lines[i] }).ToArray(); // remove item from array that equals input
            }
            System.IO.File.WriteAllLines(path, lines);
        }
    }
}
