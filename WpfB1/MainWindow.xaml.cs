using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace WpfB1
{
    public partial class MainWindow : Window
    {

        LineContext db = new LineContext();
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // гарантируем, что база данных создана
            db.Database.EnsureCreated();
            // загружаем данные из БД
            db.Lines.Load();
            // и устанавливаем данные в качестве контекста
            //DataContext = db.Lines.Local.ToObservableCollection();
        }


        private void GenerateFiles_Click(object sender, RoutedEventArgs e)
        {
            string directoryPath = @"C:\GeneratedFiles";
            Directory.CreateDirectory(directoryPath);

            Random random = new Random();
            DateTime startDate = DateTime.Now.AddYears(-5);

            for (int fileIndex = 1; fileIndex <= 100; fileIndex++)
            {
                string filePath = Path.Combine(directoryPath, $"File{fileIndex}.txt");

                using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    for (int lineIndex = 1; lineIndex <= 1000; lineIndex++)
                    {
                        DateTime randomDate = startDate.AddDays(random.Next((DateTime.Now - startDate).Days));
                        string randomLatinString = GenerateRandomString(random, 10, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
                        string randomCyrillicString = GenerateRandomString(random, 10, "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя");
                        int randomEvenNumber = random.Next(1, 50000000) * 2;
                        double randomDouble = Math.Round(1 + (random.NextDouble() * 19), 8);

                        string line = $"{randomDate:dd.MM.yyyy} || {randomLatinString} || {randomCyrillicString} || {randomEvenNumber} || {randomDouble}";

                        writer.WriteLine(line);
                    }
                }
            }

            MessageBox.Show("All files have been generated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CombineFiles_Click(object sender, RoutedEventArgs e)
        {
            string directoryPath = @"C:\GeneratedFiles";
            string outputFilePath = Path.Combine(directoryPath, "CombinedFile.txt");
            string substringToRemove = SubstringTextBox.Text;

            int removedLinesCount = 0;

            if (substringToRemove == "")
            {
                substringToRemove = "-";
            }

            try
            {
                using (FileStream fs = new FileStream(outputFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                    {
                        foreach (string file in Directory.GetFiles(directoryPath, "File*.txt"))
                        {
                            foreach (string line in File.ReadLines(file, Encoding.UTF8))
                            {
                                if (!line.Contains(substringToRemove))
                                {
                                    writer.WriteLine(line);
                                }
                                else
                                {
                                    removedLinesCount++;
                                }
                            }
                        }
                    }
                }
                MessageBox.Show($"Combining completed. {removedLinesCount} lines were removed.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (IOException ioEx)
            {
                MessageBox.Show($"File is being used by another process: {ioEx.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private string GenerateRandomString(Random random, int length, string chars)
        {
            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }
            return result.ToString();
        }

        private void SubstringTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            PlaceholderTextBlock.Visibility = string.IsNullOrEmpty(SubstringTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private List<Line> ReadLinesFromFile(string filePath)
        {
            var lines = new List<Line>();
            foreach (var line in File.ReadLines(filePath, Encoding.UTF8))
            {
                var parts = line.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(part => part.Trim())
                                .ToArray();

                if (parts.Length == 5)
                {
                    var lineObject = new Line
                    {
                        Date = DateTime.ParseExact(parts[0], "dd.MM.yyyy", CultureInfo.InvariantCulture),
                        LatinString = parts[1],
                        CyrillicString = parts[2],
                        EvenNumber = int.Parse(parts[3]),
                        DecimalNumber = double.Parse(parts[4], CultureInfo.InvariantCulture)
                    };
                    lines.Add(lineObject);
                }
            }

            return lines;
        }

        private void ImportData_Click(object sender, RoutedEventArgs e)
        {
            string combinedFilePath = @"C:\GeneratedFiles\CombinedFile.txt";
            var lines = ReadLinesFromFile(combinedFilePath);
            SaveLinesToDatabase(lines);

            MessageBox.Show($"{lines.Count} lines have been imported to the database.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SaveLinesToDatabase(List<Line> lines)
        {
            try
            {
                int totalLines = lines.Count;
                int batchSize = 100;
                int importedLines = 0;

                for (int i = 0; i < totalLines; i += batchSize)
                {
                    var batch = lines.Skip(i).Take(batchSize).ToList();
                    db.Lines.AddRange(batch);
                    db.SaveChanges();

                    importedLines += batch.Count;

                    ProgressBar.Value = (double)importedLines / totalLines * 100;
                    ImportProgressText.Text = $"Импортировано строк: {importedLines}/{totalLines}";

                    System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(delegate { }));
                }
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show(ex.InnerException?.Message);
            }
        }
    }
}